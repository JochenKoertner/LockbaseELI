#ifndef LBELI_LIBRARY_H
#define LBELI_LIBRARY_H

#include <stdio.h>

#if defined (WIN32)
    #if defined(LbwELI_KMGmbH_EXPORTS)
        #define LBELI_EXPORT __declspec(dllexport)
    #else
        #define  LBELI_EXPORT __declspec(dllimport)
    #endif
#else
    #define LBELI_EXPORT
#endif


#define LbwELI_VERSION     "0.4.1848"

typedef int (*ELIDrv2App)( const char* sSysID, const char* sJobID, const char* sJobData);

LBELI_EXPORT const char* ELIDriverInfo();
LBELI_EXPORT const char* ELIProductInfo( const char* sProductID );
LBELI_EXPORT const char* ELISystemInfo( const char* sUsers );

LBELI_EXPORT void  ELIDriverUI(const char* SessID, const char* SID);

LBELI_EXPORT const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback);
LBELI_EXPORT void ELIDestroy();

LBELI_EXPORT const char* ELIOpen( const char* sUserList, const char* sSysID, const char* sExtData);
LBELI_EXPORT const char* ELIClose( const char* sSysID, const char* sSessID );

LBELI_EXPORT int ELIApp2Drv( const char* sSysID, const char *sJobID, const char* sJobData);

#endif