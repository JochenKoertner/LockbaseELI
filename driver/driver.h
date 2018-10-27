//
// Created by Stefan Böther on 27.10.18.
//
#include "MQTTClient.h"
#include "library.h"
#include "session_list.h"

#ifndef LBELI_CLI_DRIVER_H
#define LBELI_CLI_DRIVER_H

typedef struct driver_info {
    ELIDrv2App  callback;
    MQTTClient client;
    node_t * sessions;
    char * config;
    char * host;
    long port;
} driver_info_t;


//extern MQTTClient client;
//extern node_t * sessions;
extern driver_info_t * driverInfo;

driver_info_t * new_driver(ELIDrv2App callBack);
void free_driver(driver_info_t * driver);

#endif //LBELI_CLI_DRIVER_H
