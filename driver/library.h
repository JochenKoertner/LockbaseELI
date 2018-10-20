#ifndef LBELI_LIBRARY_H
#define LBELI_LIBRARY_H

#include <stdio.h>

#if defined (WIN32)
    #if defined(LbELI_EXPORTS)
        #define LBELI_EXPORT __declspec(dllexport)
    #else
        #define  LBELI_EXPORT __declspec(dllimport)
    #endif
#else
    #define LBELI_EXPORT
#endif

typedef int (*ELIDrv2App)( const char* sSessID, int nJob, const char* sJob);

LBELI_EXPORT const char* ELIDriverInfo();
LBELI_EXPORT const char* ELIProductInfo( const char* sProductID );
LBELI_EXPORT const char* ELISystemInfo( const char* sUsers );

LBELI_EXPORT void  ELIDriverUI(const char* SessID, const char* SID);

LBELI_EXPORT const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback);
LBELI_EXPORT void ELIDestroy();

LBELI_EXPORT const char* ELIOpen( const char* sUserList, const char* sSystem, const char* sExtData);
LBELI_EXPORT const char* ELIClose( const char* sSessID );

LBELI_EXPORT int ELIApp2Drv( const char* sSessID, int nJob, const char* sJob);

#endif