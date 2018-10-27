

#ifndef UTILS_H
#define UTILS_H

#include "jsmn/jsmn.h"

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

#endif //UTILS_H
