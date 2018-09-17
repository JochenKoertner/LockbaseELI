#ifndef MQTT_LIB_LIBRARY_H
#define MQTT_LIB_LIBRARY_H


#include <stdio.h>


typedef int (*getNextValue)(void);
typedef void (*FunctionPtr)();
typedef int (*Drv2App)( const char* sSessID, int nJob, const char* sJob);


const char* DriverInfo();
const char* ProductInfo( const char* sProductID );

const char* SystemInfo( const char* sUsers );

const char* Open( const char* sUserList, const char* sSystem, const char* sExtData);
const char* Close( const char* sSessID );

int App2Drv( const char* sSessID, int nJob, const char* sJob);

void LbwELI( const char* sLic, const char* sLbwELIRev, Drv2App callback);

void LbELIFinit();

#endif