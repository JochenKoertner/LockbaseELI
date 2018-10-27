#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "library.h"
#include "jsmn/jsmn.h"

#define CLIENT_ID    "Alice"
#define SYSTEM       "channel"
#define TIMEOUT     10000L

int myCallBack( const char* sSessID, int nJob, const char* sJob) {
    printf("myCallBack: Session '%s' Job #%i ('%s') \n", sSessID, nJob, sJob);
    return 42;
}

const char* getfield(const char* line, int num)
{
    static char _line[200];
    strcpy(&_line, line);

    const char* tok;
    for (tok = strtok(_line, ",");
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
    const char* retCode = ELICreate("lic", LbwELI_VERSION, myCallBack );
    printf("ELICreate(...) => '%s'\n", retCode);
    if (strcmp(retCode, "EOK") != 0) {
        return -1;
    }

    // dump the driver-info to console
    // printf("%s\n",ELIDriverInfo());

    // dump product-info to console
    // printf("%s\n",ELIProductInfo("productid"));

    // dump system-info to console
    // printf("%s\n",ELISystemInfo("users"));

    // call ELIDriverUI
    // ELIDriverUI( "sessionID", "SID");

    // open connection to hardware (in this case MQTT broker)
    const char* csv = ELIOpen("UserList", SYSTEM, CLIENT_ID);
    const char* errorCode = getfield(csv, 1);
    if (strcmp(errorCode, "EOK") == 0) {
        const char* session = getfield(csv, 2);
        printf("ELIOpen(...) => '%s' (%s)\n", retCode, session);

        int rc = ELIApp2Drv( session, 4711, "Job");

        // close connection to hardware (i.e. MQTT broker disconnect)
        printf("ELIClose('%s') => '%s'\n", session, ELIClose(session));
    }
    else 
        printf("[%s]\n", errorCode);
    
    //return 0;
    // split csv
    // retrieve first & second element 
    // first => EOK
    // second => sessionId 

    // approach Regular Expressions or spliting bei , ?


    // destroy the driver interface
    ELIDestroy();
    printf("ELIDestroy()\n");
    return 0;
}