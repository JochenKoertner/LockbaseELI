#include <stdio.h>
#include <string.h>

#include "library.h"

#include "MQTTClient.h"

#define ADDRESS     "tcp://localhost:1883"
#define CLIENTID    "Alice"
#define TOPIC       "channel"
#define PAYLOAD     "The dark side of the moon (Pink Flyod)"
#define QOS         2
#define TIMEOUT     10000L

int myCallBack( const char* sSessID, int nJob, const char* sJob) {

    printf("%s\n", sSessID);
    printf("%i\n", nJob);
    printf("%s\n", sJob);

    return 42;
}

int main() {

    // initialise driver interface and register a callback function
    LbwELI("lic", "0.9", myCallBack );

    // dump the driver-info to console
    printf("%s\n",DriverInfo());

    // dump product-info to console
    printf("%s\n",ProductInfo("productid"));

    // dump system-info to console
    //printf("%s\n",SystemInfo("users"));


    MQTTClient client;
    MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;
    MQTTClient_message pubmsg = MQTTClient_message_initializer;
    MQTTClient_deliveryToken token;
    int rc;

    rc = MQTTClient_create(&client, ADDRESS, CLIENTID,
                      MQTTCLIENT_PERSISTENCE_NONE, NULL);
    printf("%d\n", rc);
    conn_opts.keepAliveInterval = 20;
    conn_opts.cleansession = 1;
    conn_opts.MQTTVersion = MQTTVERSION_3_1_1;

    if ((rc = MQTTClient_connect(client, &conn_opts)) != MQTTCLIENT_SUCCESS)
    {
        printf("Failed to connect, return code %d\n", rc);
        return 99;
    }
    pubmsg.payload = PAYLOAD;
    pubmsg.payloadlen = (int)strlen(PAYLOAD);
    pubmsg.qos = QOS;
    pubmsg.retained = 0;
    if ((rc = MQTTClient_publishMessage(client, TOPIC, &pubmsg, &token)) != MQTTCLIENT_SUCCESS)
    {
        printf("Failed to publish, return code %d\n", rc);
        return 99;
    }
    printf("Waiting for up to %d seconds for publication of %s\n"
           "on topic %s for client with ClientID: %s\n",
           (int)(TIMEOUT/1000), PAYLOAD, TOPIC, CLIENTID);
    rc = MQTTClient_waitForCompletion(client, token, TIMEOUT);
    printf("Message with delivery token %d delivered\n", token);
    MQTTClient_disconnect(client, 10000);
    MQTTClient_destroy(&client);

    LbELIFinit();
    return 0;
}