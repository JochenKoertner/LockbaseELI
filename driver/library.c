#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>

#include "MQTTClient.h"


#include "library.h"
#include "utils.h"
#include "session_list.h"
#include "driver.h"

#define ADDRESS     "tcp://localhost:1883"
// #define ADDRESS     "tcp://10.0.2.2:1883"
#define CLIENT_ID   "Alice"

#define TIMEOUT     1000L

#define QoS_FireAndForget   0
#define QoS_AtLeastOnce     1
#define QoS_ExactlyOnce     2



int mqtt_create(const char* serverURI) {
    return MQTTClient_create(&driverInfo->client, serverURI, CLIENT_ID, MQTTCLIENT_PERSISTENCE_NONE, NULL);
}

void mqtt_destroy() {
    MQTTClient_destroy(&driverInfo->client);
}

int mqtt_connect() {
    MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;
    conn_opts.keepAliveInterval = 20;
    conn_opts.cleansession = 1;
    int rc ;

    if ((rc = MQTTClient_connect(driverInfo->client, &conn_opts)) != MQTTCLIENT_SUCCESS) // TODO: Hier kommt er nicht zurück??
    {
        printf("Failed to connect, return code %d\n", rc);
        return rc;
    }
    return rc;
}

int mqtt_disconnect() {
    return MQTTClient_disconnect(driverInfo->client, 1000);
}

int mqtt_publish(const char* topic, const char* payload, int qos) {

    MQTTClient_deliveryToken token;

    MQTTClient_message pubmsg = MQTTClient_message_initializer;
    pubmsg.payload = (char*)payload;
    pubmsg.payloadlen = (int) strlen(payload);
    pubmsg.qos = qos;
    pubmsg.retained = 0;
    int rc;
    if ((rc = MQTTClient_publishMessage(driverInfo->client, topic, &pubmsg, &token)) != MQTTCLIENT_SUCCESS) {
        printf("Failed to publish, return code %d\n", rc);
        return rc;
    }

    if (qos == QoS_FireAndForget)
        return rc;

    printf("Waiting for up to %d seconds for publication of %s\n"
           "on topic %s for client with ClientID: %s\n",
           (int) (TIMEOUT / 1000), "sss", topic, CLIENT_ID);
    rc = MQTTClient_waitForCompletion(driverInfo->client, token, TIMEOUT);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("Message publish timed out, return code %d\n", rc);
        return rc;
    }
    return token;
}

int mqtt_subscribe(const char* topic, int qos) {
    int rc = MQTTClient_subscribe(driverInfo->client, topic, qos);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("Failed to subscribe '%s' return code %d\n", topic, rc);
    }
    return rc;
}

int mqtt_unsubscribe(const char* topic) {
    int rc = MQTTClient_unsubscribe(driverInfo->client, topic);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("Failed to unsubscribe return code %d\n", rc);
    }
    return rc;
}

int mqtt_receive_msg(const char* topic, int timeout, char** payload) {
    char* topicName = NULL;
    int topicLen;
    MQTTClient_message* msg = NULL;

    int rc = mqtt_subscribe(topic, QoS_ExactlyOnce);
    if (rc != MQTTCLIENT_SUCCESS) {
        return rc;
    }

    if ((rc = MQTTClient_receive(driverInfo->client, &topicName, &topicLen, &msg, timeout)) != MQTTCLIENT_SUCCESS) {
        mqtt_unsubscribe(topic);
        printf("Failed to receive, return code %d\n", rc);
        return rc;
    }

    if (topicName) {

        if (msg != NULL) {
            *payload = strndup((char*)(msg->payload), msg->payloadlen);
            // printf("Message received on topic %s is %.*s", topicName, msg->payloadlen, (char*)(msg->payload));
            MQTTClient_freeMessage(&msg);
        }
        MQTTClient_free(topicName);
    }
    else
        printf("No message received within timeout period\n");

    rc = mqtt_unsubscribe(topic);
    if (rc != MQTTCLIENT_SUCCESS)
        return rc;

    return MQTTCLIENT_SUCCESS;
}



/*
*  LbwELI() is the constructor of the interface object, which is required to use the interface. It expects the
*  application to identify itself by its licence and LbwELI revision number, which allows the driver to support
*  different interface revisions and adjust its services to different types of installations (e.g. factory, locksmith
*  or end user installation). Secondly the constructor expects a callback function pointer to launch the driver's
*  responses to job requests (s.b.).
*  In case the constructor supports the requested interface revision, it simply returns EOK. In case it does not,
*  it returns EREV, followed by the best interface it supports:
*       [ID:Error],[TXT:DrvELIRev]
*   This allows the application to 'downgrade' to an older drivers capabilities. If the driver cannot start
*   for ny other reason, the constructor returns EUNKNOWN.
*/

LBELI_EXPORT const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback ) {
    if (strcmp(sLbwELIRev, LbwELI_VERSION) != 0) {
        return u8"EREV,"
                LbwELI_VERSION;
    }

    driverInfo = new_driver(callback);
    int ret = mqtt_create(formatUrl("tcp", driverInfo->host, driverInfo->port));
    if (ret != MQTTCLIENT_SUCCESS) {
        return "EUNKNOWN";
    }

    /* check requested revision
    if (sLbwELIRev > "1.2") {
    return "EREV\n"
            u8"[ID:Error],[TXT:DrvELIRev]";
    }*/



    return "EOK";
}

/*
* ELIDestroy() is the destructor of the interface object, it will be called on application termination or when
* the application detaches from the driver for other reasons. The application will call the destructor anyway,
* even if the constructor did not returned EOK.
*/
LBELI_EXPORT void ELIDestroy() {
    mqtt_destroy();

    free_driver(driverInfo);
    driverInfo = NULL;
}

/*
* To retrieve global information about the driver the application will call the interface function DriverInfo()
*
* The function returns a text pointer containing definitions for the following variables (s.b.).
* The returned text must be available for the application at least until the next call of an interface function.
* The information returned by DriverInfo() is expected to be unchanged within the same driver revision.
*/

LBELI_EXPORT const char* ELIDriverInfo() {

    return u8"DriverRevision=0.9\n"         // required
        u8"LbwELIRevision=0.1.1837\n"    // required
        u8"Manufacturer = KMGmbH,\"de:Körtner & Muth GmbH\"\n" // required
        u8"Products = [ID:product 1],[LTL:product name 1];[ID: product 2],[LTL:product name 2]\n" // required
        u8"DriverAuthor = [LTL:CaptainFuture]\n" // optional
        u8"DriverCopyright = [LTL:Copyright CaptainFuture (c) 2018]\n" // optional
        u8"DriverUI = [LTL:Start DemoApp]\n" // optional
            ;
}

/*
* ELIDriverUI() will receive the application's current session id and the id of the current active system as
* parameters (s.b.). This variable is optional, by default no driver provided user interface will be supported.
*/

LBELI_EXPORT void ELIDriverUI(const char* SessID, const char* SID) {
    printf("Start Browser  SessID='%s'  SID='%s'\n", SessID, SID);
}

/*
* The device capabilities and limitations are regarded to be product dependent.
* They will be requested by a call to the ProductInfo() method
*
* The function returns a text pointer containing definitions for the following variables (s.b.).
* The returned text must be available for the application at least until the next call of an interface function.
* The information returned by ProductInfo() is expected to be unchanged within the same driver revision.
*/

LBELI_EXPORT const char* ELIProductInfo( const char* sProductID ) {

    parseProductInfo(driverInfo->config, sProductID, &driverInfo->productInfo);
    return driverInfo->productInfo;
}

LBELI_EXPORT const char* ELISystemInfo( const char* sUsers ) {

    char* pointer = NULL;
    parseSystemInfo(driverInfo->config, &pointer);
    return driverInfo->systemInfo;

    // send message to broker
    // connect();
    // publish(PAYLOAD);
    // wait for response (PAYLOAD)
    // disconnect();
    /*return
            u8"[ID:Sys1],[ID:ProductID],[TXT:Name],[ACLR],['0':disable|'1':enable]\n"
            u8"[ID:Sys2],[ID:ProductID],[TXT:Name],[ACLR],['0':disable|'1':enable]\n"
            u8",[ID:Product1],,[ACLR]\n"
            u8",[ID:Product2],,[ACLR]\n"; */
}

LBELI_EXPORT const char* ELIOpen( const char* sUserList, const char* sSysID, const char* sExtData) {

    // connect to the broker
    int rc = mqtt_connect();
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("mqtt_connect() => %i\n", rc);
        return "EUNKNOWN,,,,'0'";
    }

    node_t * node = new_session(&driverInfo->sessions, sUserList, sSysID, sExtData);
    const char* sSessID = session_id_to_string(node->session_id);
    const char* message = create_event_payload("ELIOpen", sSessID, "OPEN,sSystem,sExtData");
    rc = mqtt_publish(sSysID, message, QoS_FireAndForget);
    if (rc != MQTTCLIENT_SUCCESS) {
        return "EUNKNOWN,,,,'0'";
    }
    
    static char buf[100];
    sprintf(buf, "%s,%08X,ACLR,%08X,'1'", "EOK", node->session_id, node->last_session_id);
    return buf;
}

LBELI_EXPORT const char* ELIClose( const char* sSysID, const char* sSessID ) {

    int session_id = string_to_session_id(sSessID);

    node_t * node = find_session(driverInfo->sessions, session_id);
    if (!node)
    {
        printf("session %s unknown\n", sSessID);
        return "EUNKNOWN";
    }

    const char* message = create_event_payload("ELIClose", sSessID, "CLOSE,session");
    int rc = mqtt_publish(node->sSystem, message, QoS_FireAndForget);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("not publish to %s retcode %d \n", node->sSystem, rc);
        return "ECONNECTION";
    }

    // remove the session from the list
    remove_session(&driverInfo->sessions, session_id);

    // disconnet from mqtt broker
    int ret = mqtt_disconnect();
    if (ret != MQTTCLIENT_SUCCESS) {
        printf("mqtt_disconnect() => %i\n", ret);
        return "EUNKNOWN";
    }
    return "EOK";
}

LBELI_EXPORT int ELIApp2Drv( const char* sSysID, const char *sJobID, const char* sJobData) {

    int session_id = string_to_session_id(sSysID);

    node_t * node = find_session(driverInfo->sessions, session_id);
    if (!node)
    {
        printf("session %s unknown\n", sSysID);
        return -1;
    }

    const char* message = create_event_payload("ELIApp2Drv", sSysID, sJobData);
    int rc = mqtt_publish(node->sSystem, message, QoS_FireAndForget);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("not publish to %s retcode %d \n", node->sSystem, rc);
        return -1;
    }

    /*   "response" cames separatly and asynchron via ELIDrv2App
    char* payload = NULL;
    rc = mqtt_receive_msg("heartbeat", 5000L, &payload);
    if (rc != MQTTCLIENT_SUCCESS) {
        printf("nothing receive to retcode %d \n", rc);
    } else {
        char* sessionId = NULL;
        char* text = NULL;
        parse_payload(payload, &sessionId, &text);
        printf("Heartbeat sessionId '%s'\n", sessionId);
        printf("Heartbeat text '%s'\n", text);

        free(sessionId);
        free(text);
    };

    free(payload);
     */

    return 0;
}
