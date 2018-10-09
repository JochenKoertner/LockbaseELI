#include <stdio.h>
#include <string.h>

#include "library.h"

#include "MQTTClient.h"

#define ADDRESS     "tcp://localhost:1883"
#define CLIENT_ID    "Alice"
#define TOPIC       "heartbeat"
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
    const char* retCode = ELICreate("lic", "0.9", myCallBack );
    printf("ELICreate() => %s\n\n", retCode);

    // dump the driver-info to console
    printf("%s\n",ELIDriverInfo());

    // dump product-info to console
    printf("%s\n",ELIProductInfo("productid"));

    // dump system-info to console
    printf("%s\n",ELISystemInfo("users"));

    // call ELIDriverUI
    ELIDriverUI( "sessionID", "SID");

    // open connection to hardware (in this case MQTT broker)
    const char* session = ELIOpen("UserList", TOPIC, CLIENT_ID);
    printf("Session : %s\n", session);

    // close connection to hardware (i.e. MQTT broker disconnect)
    printf("%s\n", ELIClose(session));

    ELIDestroy();
    return 0;
}