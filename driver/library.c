

#include <stdio.h>
#include <string.h>

#include "MQTTClient.h"
#include "library.h"

/*
 * Global variable for holding the function pointer for the callback from driver
 */
ELIDrv2App  globalCallback;

#define ADDRESS     "tcp://localhost:1883"
#define CLIENT_ID   "Alice"

#define TOPIC       "channel"
#define PAYLOAD     "The Darkside of the moon (Pink Flyod)"
#define QOS         2
#define TIMEOUT     10000L


MQTTClient client;
MQTTClient_message pubmsg = MQTTClient_message_initializer;
MQTTClient_deliveryToken token;

int mqtt_connect() {
    MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;

    conn_opts.keepAliveInterval = 20;
    conn_opts.cleansession = 1;
    int rc ;

    if ((rc = MQTTClient_connect(client, &conn_opts)) != MQTTCLIENT_SUCCESS) // TODO: Hier kommt er nicht zurück??
    {
        printf("Failed to connect, return code %d\n", rc);
        return rc;
    }
    return 0;
}

void mqtt_disconnect() {
    MQTTClient_disconnect(client, 10000);
}

int mqtt_publish(const char* payload) {
    pubmsg.payload = PAYLOAD;
    pubmsg.payloadlen = (int) strlen(PAYLOAD);
    pubmsg.qos = QOS;
    pubmsg.retained = 0;
    int rc;
    if ((rc = MQTTClient_publishMessage(client, TOPIC, &pubmsg, &token)) != MQTTCLIENT_SUCCESS) {
        printf("Failed to publish, return code %d\n", rc);
        return rc;
    }
    printf("Waiting for up to %d seconds for publication of %s\n"
           "on topic %s for client with ClientID: %s\n",
           (int) (TIMEOUT / 1000), PAYLOAD, TOPIC, CLIENT_ID);
    rc = MQTTClient_waitForCompletion(client, token, TIMEOUT);
    printf("Message with delivery token %d delivered\n", token);
    return rc;

}




/*
 *  LbwELI() is the constructor of the interface object, which is required to use the interface. It expects the
 *  application to identify itself by its licence and LbwELI revision number, which allows the driver to support
 *  different interface revisions and adjust its services to different types of installations (e.g. factory, locksmith
 *  or end user installation). Secondly the constructor expects a callback function pointer to launch the driver's
 *  responses to job requests (s.b.).
 */

const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback ) {
    printf("Lizenz: %s Revision: %s", sLic, sLbwELIRev);
    globalCallback = callback;

    printf("MQTTClient_create (start)\n");

    //int rc = MQTTClient_create(&client, ADDRESS, CLIENTID,
    //                       MQTTCLIENT_PERSISTENCE_NONE, NULL);
    //printf("MQTTClient_create (%d)\n", rc);
    return "EOK";
}

void ELIDestroy() {
    // MQTT free
    //MQTTClient_destroy(&client);
}




/*
 * To retrieve global information about the driver the application will call the interface function DriverInfo()
 *
 * The function returns a text pointer containing definitions for the following variables (s.b.).
 * The returned text must be available for the application at least until the next call of an interface function.
 * The information returned by DriverInfo() is expected to be unchanged within the same driver revision.
 */

const char* ELIDriverInfo() {

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

void ELIDriverUI(const char* SessID, const char* SID) {
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

const char* ELIProductInfo( const char* sProductID ) {

    return
            u8"ProgrammingTarget=0\n" // required
            u8"DeviceCapacity=INT\n"  // required
            u8"TimePeriodCapacity=INT\n"  // required
            u8"EventTypes =[ID:EventID0],[ID:Class],[LTL:Description0];" // required
            u8"[ID:EventID1],[ID:Class],[LTL:Description1];\n"

            // the following values are optional
            u8"OnlineSystem=0\n"
            u8"DefaultAccess=0\n"
            u8"AccessByNmbOfLockings=0\n"
            u8"AccessByFloatingPeriod=0\n"
            u8"TimePeriodRecurrence=[ID:RecIntID1],[ID:RecIntID2]\n"
            u8"EventUpdateInterval=[INT:60]\n"
            u8"AccessUpdateInterval=[INT:120]\n"
            ;
}

const char* ELISystemInfo( const char* sUsers ) {
    // send message to broker
    //connect();
    //publish(PAYLOAD);
    // wait for response (PAYLOAD)
    //disconnect();
    return
            u8"[ID:Sys1],[ID:ProductID],[TXT:Name],[ACLR],['0':disable|'1':enable]\n"
            u8"[ID:Sys2],[ID:ProductID],[TXT:Name],[ACLR],['0':disable|'1':enable]\n"
            u8",[ID:Product1],,[ACLR]\n"
            u8",[ID:Product2],,[ACLR]\n";
}