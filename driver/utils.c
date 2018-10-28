
#include <stdio.h>
#include <stdlib.h>
#include <string.h>


#include "utils.h"


char* session_id_to_string(int session_id) {
    static char buf[9];
    sprintf(buf, "%08X", session_id);
    return buf;
}

int string_to_session_id(const char* sSessID) {
    return (int)strtol(sSessID, NULL, 16);
}

char* formatUrl(const char* protocol, const char* host, long port) {
    static char address[100];
    sprintf(address, "%s://%s:%ld", protocol, host ? host : "localhost", port);
    return address;
}

char* create_event_payload(const char* eventName, const char* sSessID, const char* sText) {
    static char buf[200];
    sprintf(buf,
            u8"{ eventName: '%s', session_id : '%s', text : '%s' }",
            eventName, sSessID, sText);
    return buf;
}

#ifdef WIN32
char *strndup(char *str, int chars)
{
    char *buffer;
    int n;

    buffer = (char *) malloc(chars +1);
    if (buffer)
    {
        for (n = 0; ((n < chars) && (str[n] != 0)) ; n++) buffer[n] = str[n];
        buffer[n] = 0;
    }

    return buffer;
}
#endif

void parse_payload(const char* json, char** sessionId, char** text) {

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);
    r = jsmn_parse(&p, json, strlen(json), t, sizeof(t)/sizeof(t[0]));
    if (r < 0) {
        printf("Failed to parse JSON: %d\n", r);
    }

    /* Assume the top-level element is an object */
    if (r < 1 || t[0].type != JSMN_OBJECT) {
        printf("Object expected\n");
    }

    /* Loop over all keys of the root object */
    for (i = 1; i < r; i++) {
        if (jsoneq(json, &t[i], "session_id") == 0) {
            *sessionId = strndup(json + t[i+1].start, t[i+1].end-t[i+1].start);
            i++;
        } else if (jsoneq(json, &t[i], "text") == 0) {
            *text = strndup(json + t[i+1].start, t[i+1].end-t[i+1].start);
            i++;
        }
    }
}