#ifndef MQTT_LIB_LIBRARY_H
#define MQTT_LIB_LIBRARY_H

#include <stdio.h>

typedef int (*ELIDrv2App)( const char* sSessID, int nJob, const char* sJob);

const char* ELIDriverInfo();
const char* ELIProductInfo( const char* sProductID );
const char* ELISystemInfo( const char* sUsers );

void  ELIDriverUI(const char* SessID, const char* SID);

const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback);
void ELIDestroy();

const char* ELIOpen( const char* sUserList, const char* sSystem, const char* sExtData);
const char* ELIClose( const char* sSessID );

int ELIApp2Drv( const char* sSessID, int nJob, const char* sJob);

#endif