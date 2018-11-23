// setup and release driver structure

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <stdbool.h>

#include "driver.h"
#include "utils.h"

void parseConfigFile(const char* json, char** host, long* port) {

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);
    if (json == NULL) return;

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
        if (jsoneq(json, &t[i], "host") == 0) {
            *host = strndup(json + t[i+1].start, t[i+1].end-t[i+1].start);
            i++;
        } else if (jsoneq(json, &t[i], "port") == 0) {
            *port = strtol(json + t[i+1].start, NULL, 10);
            i++;
        }
    }
}


char* concatStrings(char* source, const char* appendStr, char separator) {

    if (source == NULL)
        return strdup(appendStr);
    else {
        int lenSource = strlen(source);
        int lenAppend = strlen(appendStr);
        char* result = malloc(lenSource+1+lenAppend+1);
        strncpy(result, source, lenSource);
        result[lenSource] = separator;
        result[lenSource+1] = 0;
        strncpy(result+(lenSource+1), appendStr, lenAppend+1);
        free(source);
        return result;
    }
}

char* getStringField(const char* keyName, jsmntok_t* token, const char* json) {
    char* result=malloc(strlen(keyName)+3+(token->end-token->start)+1);
    sprintf(result, "%s='%.*s'", keyName, token->end-token->start, json + token->start);
    return result;
}

char* getIntField(const char* keyName, jsmntok_t* token, const char* json) {
    char* result=malloc(strlen(keyName)+1+(token->end-token->start)+1);
    sprintf(result, "%s=%.*s", keyName, token->end-token->start, json + token->start);
    return result;
}

char* getBoolField(const char* keyName, jsmntok_t* token, const char* json) {
    char* result=malloc(strlen(keyName)+4+1);
    int flag = json[token->start] == 't' || json[token->start] == 'T';
    sprintf(result, "%s='%i'", keyName, flag);
    return result;
}

char* getIdArrayField(const char* keyName, jsmntok_t* token, const char* json) {

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);

    r = jsmn_parse(&p, json + token->start, token->end-token->start, t, sizeof(t)/sizeof(t[0]));
    if (r < 0) {
        printf("Failed to parse JSON: %d\n", r);
    }

    char* list = NULL;
    for (i = 1; i < r; i++) {
        if (t[i].type == JSMN_STRING) {
            {
                int len=t[i].end - t[i].start;
                char* value = malloc(len + 1);
                strncpy(value, json + token->start + t[i].start, len);
                list = concatStrings(list, value, ',');
                free(value);
            }
        }
    }

    char* result = concatStrings(strdup(keyName), list, '=');
    free(list);
    return result;
}

char* getEventArray(jsmntok_t* token, const char* json) {

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);

    r = jsmn_parse(&p, json + token->start, token->end-token->start, t, sizeof(t)/sizeof(t[0]));
    if (r < 0) {
        printf("Failed to parse JSON: %d\n", r);
    }

    char* list = NULL;
    for (i = 1; i < r; i++) {
        if (t[i].type == JSMN_STRING) {
            {
                int len=t[i].end - t[i].start;
                char* value = malloc(len + 1);
                strncpy(value, json + token->start + t[i].start, len);
                list = concatStrings(list, value, ',');
                free(value);
            }
        }
    }

    return list;
}

char* getEventTypesField(const char* keyName, jsmntok_t* token, const char* json) {

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);

    r = jsmn_parse(&p, json + token->start, token->end-token->start, t, sizeof(t)/sizeof(t[0]));
    if (r < 0) {
        printf("Failed to parse JSON: %d\n", r);
    }

    char* list = NULL;
    for (i = 1; i < r; i++) {
        if (t[i].type == JSMN_ARRAY) {
            {
                int start = i+1;
                int end = i + t[i].size;

                char* items = NULL;

                for( i = start; i <= end; i++) {
                    if (t[i].type == JSMN_STRING)
                    {
                        int len=t[i].end - t[i].start;
                        char* value = string_alloc(json + token->start + t[i].start, len);
                        items = concatStrings(items, value, ',');
                        free(value);
                    }
                }

                list = concatStrings(list, items, ';');
                free(items);

                i = end;
            }
        }
    }

    char* result = concatStrings(strdup(keyName), list, '=');
    free(list);
    return result;
}


void parseProductInfo(const char* json, const char* sProductID, char** productInfo) {
    free(*productInfo);

    int i;
    int r;
    jsmn_parser p;
    jsmntok_t t[128]; /* We expect no more than 128 tokens */

    jsmn_init(&p);
    if (json == NULL) return;

    r = jsmn_parse(&p, json, strlen(json), t, sizeof(t)/sizeof(t[0]));
    if (r < 0) {
        printf("Failed to parse JSON: %d\n", r);
    }

    /* Assume the top-level element is an object */
    if (r < 1 || t[0].type != JSMN_OBJECT) {
        printf("Object expected\n");
    }

    int found = false;
    /* Loop over all keys of the root object */
    for (i = 1; i < r; i++) {
        if (jsoneq(json, &t[i], "productInfos") == 0 && t[i + 1].type == JSMN_OBJECT) {
            {
                found = true;
                break;
            }
        }
    }

    if (found) {
        const char* pi = json + t[i + 1].start;
        r = jsmn_parse(&p, pi, t[i+1].end - t[i+1].end, t, sizeof(t)/sizeof(t[0]));
        if (r < 0) {
            printf("Failed to parse JSON: %d\n", r);
        }

        found = false;
        for (i = 1; i < r; i++) {
            if (jsoneq(json, &t[i], sProductID) == 0 && t[i + 1].type == JSMN_OBJECT) {
                {
                    found = true;
                    printf("von bis %i .. %i\n ", t[i + 1].start, t[i + 1].end);
                    break;
                }
            }
        }

        if (found) {
            const char* pii = json + t[i + 1].start;
            r = jsmn_parse(&p, pii, t[i+1].end - t[i+1].end, t, sizeof(t)/sizeof(t[0]));
            if (r < 0) {
                printf("Failed to parse JSON: %d\n", r);
            }

            const char* PRODUCT_NAME = "ProductName";
            const char* PROGRAMMING_TARGET = "ProgrammingTarget";
            const char* DEVICE_CAPACITY = "DeviceCapacity";
            const char* TIMEPERIOD_CAPACITY = "TimePeriodCapacity";
            const char* ONLINE_SYSTEM = "OnlineSystem";
            const char* DEFAULT_ACCESS = "DefaultAccess";

            const char* ACCESS_BY_NMBOFLOCKINGS = "AccessByNmbOfLockings";
            const char* ACCESS_BY_FLOATINGPERIOD = "AccessByFloatingPeriod";

            const char* EVENT_UPDATE_INTERVAL = "EventUpdateInterval";
            const char* ACCESS_UPDATE_INTERVAL = "AccessUpdateInterval";

            const char* TIMEPERIOD_RECURRENCE = "TimePeriodRecurrence";
            const char* EVENT_TYPES = "EventTypes";

            /* Loop over all keys of the root object */
            for (i = 1; i < r; i++) {
                if (jsoneq(json, &t[i], PRODUCT_NAME) == 0) {
                    char* productName = getStringField(PRODUCT_NAME, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, productName, NEWLINE);
                    free(productName);
                    i++;
                } else if (jsoneq(json, &t[i], PROGRAMMING_TARGET) == 0) {
                    char* programmingTarget = getStringField(PROGRAMMING_TARGET, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, programmingTarget, NEWLINE);
                    free(programmingTarget);
                    i++;
                } else if (jsoneq(json, &t[i], DEVICE_CAPACITY) == 0) {
                    char* deviceCapacity = getIntField(DEVICE_CAPACITY, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, deviceCapacity, NEWLINE);
                    free(deviceCapacity);
                    i++;
                } else if (jsoneq(json, &t[i], TIMEPERIOD_CAPACITY) == 0) {
                    char* timePeriodCapacity = getIntField(TIMEPERIOD_CAPACITY, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, timePeriodCapacity, NEWLINE);
                    free(timePeriodCapacity);
                    i++;
                } else if (jsoneq(json, &t[i], ONLINE_SYSTEM) == 0) {
                    char* onlineSystem = getBoolField(ONLINE_SYSTEM, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, onlineSystem, NEWLINE);
                    free(onlineSystem);
                    i++;
                } else if (jsoneq(json, &t[i], DEFAULT_ACCESS) == 0) {
                    char* defaultAccess = getBoolField(DEFAULT_ACCESS, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, defaultAccess, NEWLINE);
                    free(defaultAccess);
                    i++;
                } else if (jsoneq(json, &t[i], EVENT_UPDATE_INTERVAL) == 0) {
                    char* eventUpdateInterval = getIntField(EVENT_UPDATE_INTERVAL, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, eventUpdateInterval, NEWLINE);
                    free(eventUpdateInterval);
                    i++;
                } else if (jsoneq(json, &t[i], ACCESS_UPDATE_INTERVAL) == 0) {
                    char* accessUpdateInterval = getIntField(ACCESS_UPDATE_INTERVAL, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, accessUpdateInterval, NEWLINE);
                    free(accessUpdateInterval);
                    i++;
                } else if (jsoneq(json, &t[i], ACCESS_BY_NMBOFLOCKINGS) == 0) {
                    char* accessByNmbOfLockings = getBoolField(ACCESS_BY_NMBOFLOCKINGS, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, accessByNmbOfLockings, NEWLINE);
                    free(accessByNmbOfLockings);
                    i++;
                } else if (jsoneq(json, &t[i], ACCESS_BY_FLOATINGPERIOD) == 0) {
                    char* accessByFloatingPeriod = getBoolField(ACCESS_BY_FLOATINGPERIOD, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, accessByFloatingPeriod, NEWLINE);
                    free(accessByFloatingPeriod);
                    i++;
                } else if (jsoneq(json, &t[i], TIMEPERIOD_RECURRENCE) == 0) {
                    char* timePeriodRecurrence = getIdArrayField(TIMEPERIOD_RECURRENCE, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, timePeriodRecurrence, NEWLINE);
                    free(timePeriodRecurrence);
                    i++;
                } else if (jsoneq(json, &t[i], EVENT_TYPES) == 0) {
                    char* eventTypes = getEventTypesField(TIMEPERIOD_RECURRENCE, &t[i+1], json);
                    *productInfo = concatStrings(*productInfo, eventTypes, NEWLINE);
                    free(eventTypes);
                    i++;
                }
            }
        }
    }
}

void parseSystemInfo(const char* json, char** systemInfo) {

}

// read config file (json format)
char* readFile(const char* file) {
    FILE *fp = fopen(file, "rb");
    if ( fp != NULL )
    {
        char *buf;

        unsigned long fileLen = 0;
        unsigned long bytesRead = 0;

        fseek(fp, 0, SEEK_END);
        fileLen = ftell(fp);
        fseek(fp, 0, SEEK_SET);
        buf=(char *)malloc(fileLen);
        bytesRead = (int)fread(buf, sizeof(char), fileLen, fp);
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

driver_info_t * driverInfo = NULL;

driver_info_t * new_driver(ELIDrv2App callBack) {
    // seed random number generator with clock
    srand(time(0));

    driver_info_t * new_driver;
    new_driver = malloc(sizeof(driver_info_t));

    new_driver->callback = callBack;
    new_driver->sessions = NULL;
    new_driver->config = readFile("../config.json");
    new_driver->port = 1883;
    new_driver->host = NULL;
    new_driver->productInfo = NULL;
    new_driver->systemInfo = NULL;


    if (new_driver->config != NULL) {
        parseConfigFile(new_driver->config, &new_driver->host, &new_driver->port);
    }
    else {
        new_driver->host = strdup("localhost");
    }

    return new_driver;
}

void free_driver(driver_info_t * driver) {
    free(driver->config);
    free(driver->host);
    free(driver->productInfo);
    free(driver->systemInfo);
    free(driver);
}