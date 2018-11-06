// setup and release driver structure

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

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

void parseProductInfo(const char* json, const char* sProductID, char** productInfo) {
    free(driverInfo->productInfo);

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
        if (jsoneq(json, &t[i], "productInfos") == 0 && t[i + 1].type == JSMN_OBJECT) {
            {
                printf("von bis %i .. %i\n ", t[i + 1].start, t[i + 1].end);
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