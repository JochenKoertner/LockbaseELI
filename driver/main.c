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

const char* getField(const char* line, int num)
{
    static char _result[100];
    char* prev = line;
    int len=0;
    while (num > 0) {
        if (prev != NULL) {
            char *separator = strchr(prev, ',');
            if (separator != NULL)
            {
                len = separator - prev;
                if (num == 1) {
                    strncpy(_result, prev, len);
                }
                prev = separator + 1;
                if (num == 1) {
                    return _result;
                }
            }
            else
            {
                if (num == 1)
                {
                    strcpy(_result, prev);
                    return _result;
                }
                prev = NULL;
            }
        }
        num--;
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
    const char* errorCode = getField(csv, 1);
    if (strcmp(errorCode, "EOK") == 0) {
        const char* session = getField(csv, 2);

        printf("ELIOpen(...) => '%s' (%s)\n", retCode, session);

        ELIApp2Drv( session, 4711, "LD");

        /* >> DK,[ID:KID1],[TXT:Name1],,[B64:ExtData] DK,[ID:KID2],[TXT:Name2],,[B64:ExtData]
        DL,[ID:LID1],[TXT:Name1],,[B64:ExtData] DL,[ID:LID2],[TXT:Name2],,[B64:ExtData]
        ... AT,[ID:TID1],0-5,20120508T105000Z/153040,20120608T105000Z/173040 AT,[ID:TID2],,20120509T220000Z/3600,20120510T220000Z/3600
        ... AK,[ID:KID1],[ID:TID1],[LID1],[LID2],[LID3],... AK,[ID:KID1],[ID:TID2],[LID3],[LID5],[LID7],...
        12/14 Körtner & Muth GmbH, LbwELI – Lockbase Electronic Lock Interface Rev. 0.1.1837
        AK,[ID:KID2],[ID:TID1],[LID1],[LID3],[LID4],... ...
        LDR,OK
        */

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