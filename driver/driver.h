
#include "MQTTClient.h"
#include "library.h"
#include "session_list.h"

#ifndef DRIVER_H
#define DRIVER_H

typedef struct driver_info {
    ELIDrv2App  callback;
    MQTTClient client;
    node_t * sessions;
    char * config;
    char * host;
    long port;
} driver_info_t;

extern driver_info_t * driverInfo;

driver_info_t * new_driver(ELIDrv2App callBack);
void free_driver(driver_info_t * driver);

#endif //DRIVER_H
