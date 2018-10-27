#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "library.h"
#include "jsmn/jsmn.h"

#define CLIENT_ID    "Alice"
#define SYSTEM       "channel"
#define TIMEOUT     10000L

int myCallBack( const char* sSessID, int nJob, const char* sJob) {

    printf("%s\n", sSessID);
    printf("%i\n", nJob);
    printf("%s\n", sJob);

    return 42;
}

static const char *JSON_STRING =
	"{\"host\": \"localhost\", \"port\": 1883,\n }";

static int jsoneq(const char *json, jsmntok_t *tok, const char *s) {
	if (tok->type == JSMN_STRING && (int) strlen(s) == tok->end - tok->start &&
			strncmp(json + tok->start, s, tok->end - tok->start) == 0) {
		return 0;
	}
	return -1;
}

void parseConfigFile() {

    int i;
    int r;
    jsmn_parser p;
	jsmntok_t t[128]; /* We expect no more than 128 tokens */

	jsmn_init(&p);
	r = jsmn_parse(&p, JSON_STRING, strlen(JSON_STRING), t, sizeof(t)/sizeof(t[0]));
	if (r < 0) {
		printf("Failed to parse JSON: %d\n", r);
	}

    	/* Assume the top-level element is an object */
	if (r < 1 || t[0].type != JSMN_OBJECT) {
		printf("Object expected\n");
	}

	/* Loop over all keys of the root object */
	for (i = 1; i < r; i++) {
		if (jsoneq(JSON_STRING, &t[i], "host") == 0) {
			/* We may use strndup() to fetch string value */
			printf("- Host: %.*s\n", t[i+1].end-t[i+1].start,
					JSON_STRING + t[i+1].start);
			i++;
		} else if (jsoneq(JSON_STRING, &t[i], "port") == 0) {
			/* We may want to do strtol() here to get numeric value */
			printf("- port: %.*s\n", t[i+1].end-t[i+1].start,
					JSON_STRING + t[i+1].start);
			i++;
		} else {
			printf("Unexpected key: %.*s\n", t[i].end-t[i].start,
					JSON_STRING + t[i].start);
		}
	}

    printf("All ok %d Tokens found!\n", r);
}

char* readFile(const char* file) {
    FILE *fp = fopen(file, "rb");
	if ( fp != NULL )
	{
        char *buf;
        unsigned long fileLen = 0;
        unsigned long bytesRead = 0;

		fseek(fp, 0, SEEK_END);
		int fileLen = ftell(fp);
		fseek(fp, 0, SEEK_SET);
		buf=(char *)malloc(fileLen);
		bytesRead = (int)fread(buf, sizeof(char), fileLen, fp);
		*buffer = buf;
		*buflen = bytesRead;
		if ( bytesRead != fileLen )
		{
            free(buf);
            return NULL;
        }
		fclose(fp);
	    return buf;
	}
    return NULL;
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

    // read configuration from Json File
    parseConfigFile();
    return 0;

    // initialise driver interface and register a callback function
    const char* retCode = ELICreate("lic", LbwELI_VERSION, myCallBack );
    if (strcmp(retCode, "EOK") != 0) {
        printf("%s\n", retCode);
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
        printf("#3 %s\n", session);

        int rc = ELIApp2Drv( session, 4711, "Job");

        // close connection to hardware (i.e. MQTT broker disconnect)
        printf("%s\n", ELIClose(session));
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
    return 0;
}