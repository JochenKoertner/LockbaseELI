

#include <stdio.h>
#include <string.h>

#include "MQTTClient.h"
#include "library.h"

/*
 * Global variable for holding the function pointer for the callback from driver
 */
Drv2App  globalCallback;

#define ADDRESS     "tcp://localhost:1883"
#define CLIENTID    "Alice"

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
           (int) (TIMEOUT / 1000), PAYLOAD, TOPIC, CLIENTID);
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

void LbwELI( const char* sLic, const char* sLbwELIRev, Drv2App callback ) {
    printf("Lizenz: %s Revision: %s", sLic, sLbwELIRev);
    globalCallback = callback;

    printf("MQTTClient_create (start)\n");

    //int rc = MQTTClient_create(&client, ADDRESS, CLIENTID,
    //                       MQTTCLIENT_PERSISTENCE_NONE, NULL);
    //printf("MQTTClient_create (%d)\n", rc);
}

void LbELIFinit() {
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

const char* DriverInfo() {

    return u8"DriverRevision=0.9\n"
           u8"LbwELIRevision=0.7\n"
           u8"Manufacturer = KMGmbH,\"de:Körtner & Muth GmbH\"\n"
           u8"Products = [ID:product 1],[LTL:product name 1];[ID: product 2],[LTL:product name 2]\n";
}

/*
 * The device capabilities and limitations are regarded to be product dependent.
 * They will be requested by a call to the ProductInfo() method
 *
 * The function returns a text pointer containing definitions for the following variables (s.b.).
 * The returned text must be available for the application at least until the next call of an interface function.
 * The information returned by ProductInfo() is expected to be unchanged within the same driver revision.
 */

const char* ProductInfo( const char* sProductID ) {

    return u8"ProgrammingTarget=0\n";
}

const char* SystemInfo( const char* sUsers ) {
    // send message to broker
    //connect();
    //publish(PAYLOAD);
    //disconnect();

    return u8"ProgrammingTarget=0\n";
}