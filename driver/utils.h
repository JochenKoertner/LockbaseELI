

#ifndef UTILS_H
#define UTILS_H

#define JSMN_HEADER
#include "jsmn/jsmn.h"

#define NEWLINE 0x0A

static int jsoneq(const char *json, jsmntok_t *tok, const char *s) {
    if (tok->type == JSMN_STRING && (int) strlen(s) == tok->end - tok->start &&
        strncmp(json + tok->start, s, tok->end - tok->start) == 0) {
        return 0;
    }
    return -1;
}

char* session_id_to_string(int session_id);
int string_to_session_id(const char* sSessID);
char* formatUrl(const char* protocol, const char* host, long port);

char* create_event_payload(const char* eventName, const char* sSessID, const char* sText);
void parse_payload(const char* json, char** sessionId, char** text);

char* string_alloc(const char* source, int len);


#ifdef WIN32
char *strndup(const char *str, unsigned int chars);
#endif

#endif //UTILS_H
