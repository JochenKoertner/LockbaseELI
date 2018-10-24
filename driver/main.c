#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "library.h"

#define CLIENT_ID    "Alice"
#define SYSTEM       "channel"
#define TIMEOUT     10000L

int myCallBack( const char* sSessID, int nJob, const char* sJob) {

    printf("%s\n", sSessID);
    printf("%i\n", nJob);
    printf("%s\n", sJob);

    return 42;
}

const char* getfield(char* line, int num)
{
    const char* tok;
    for (tok = strtok(line, ",");
            tok && *tok;
            tok = strtok(NULL, ","))
    {
        if (!--num)
            return tok;
    }
    return NULL;
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
    char* csv = ELIOpen("UserList", SYSTEM, CLIENT_ID);

    const char* errorCode = getfield(csv, 1);
    const char* session = getfield(csv, 2);
    printf("[%s]\n[%s]\n", csv, session);
    

    // split csv
    // retrieve first & second element 
    // first => EOK
    // second => sessionId 

    // approach Regular Expressions or spliting bei , ?


    int rc = ELIApp2Drv( session, 4711, "Job");

    // close connection to hardware (i.e. MQTT broker disconnect)
    printf("%s\n", ELIClose(session));

    // destroy the driver interface
    ELIDestroy();
    return 0;
}