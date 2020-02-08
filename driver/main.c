#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "library.h"
#include "utils.h"

#define CLIENT_ID	"Alice"
#define SYSTEM		"ELIDemo"
#define TIMEOUT		10000L

#define JOB_ID		"4711"

// const char* sSysID, const char* sJobID, const char* sJobData
int myCallBack( const char* sSysID, const char*  sJobID, const char* sJobData) {
	printf("myCallBack: Session '%s' Job #%s ('%s') \n", sSysID, sJobID, sJobData);
	return 42;
}

const char* getField(const char* line, int num)
{
	static char _result[100];
	const char* prev;
	prev = line;
	int len=0;
	while (num > 0) {
		if (prev != NULL) {
			char *separator = strchr(prev, ',');
			if (separator != NULL)
			{
				len = separator - prev;
				if (num == 1) {
					strncpy(_result, prev, len);
				}
				prev = separator + 1;
				if (num == 1) {
					return _result;
				}
			}
			else
			{
				if (num == 1)
				{
					strcpy(_result, prev);
					return _result;
				}
				prev = NULL;
			}
		}
		num--;
	}
	return NULL;
}

int main() {

	// initialise driver interface and register a callback function
	const char* retCode = ELICreate("lic", LbwELI_VERSION, myCallBack );
	printf("ELICreate(...) => '%s'\n\n", retCode);
	if (strcmp(retCode, "OK,0.4") != 0) {
		return -1;
	}

	// dump the driver-info to console
	const char* driverInfo = ELIDriverInfo();
	printf("ELIDriverInfo() => \n%s\n\n", driverInfo);

	// dump product-info to console
	const char* productId = PRODUCT_ID;
	const char* productInfos = ELIProductInfo(productId);
	printf("ELIProductInfo('%s') => \n%s\n\n", productId, productInfos);

	// dump system-info to console
	const char* users = "users";
	const char* systemInfo = ELISystemInfo(users);
	printf("ELISystemInfo('%s') => \n%s\n\n", users, systemInfo);

	// call ELIDriverUI
	// ELIDriverUI( "sessionID", "SID");


	int mySession = rand();
	printf("MySession '%08X' \n\n", mySession);

	// open connection to hardware (in this case MQTT broker)
	const char* csv = ELIOpen("jk", SYSTEM, CLIENT_ID);
	const char* errorCode = getField(csv, 1);
	if (strcmp(errorCode, "OK") == 0) {
		const char* session = getField(csv, 4);

		printf("ELIOpen(...) => '%s' (%s)\n", retCode, session);

		ELIApp2Drv( SYSTEM, JOB_ID, "DK,000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,040000iavs1lo,,,MTA0LTEsIEtpc3RsZXIsIFNhYmluZQA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,080000ijvs1lo,,,MTA1LTEsIEtvaGwsIFVscmljaAA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,0c0000ml0c25o,,,MjAzLTEsIFdhbHRlciwgSmVucwA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,0g0000ml0c25o,,,MjAzLTIsIFdpbnRlciwgU2luYQA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,0k0000ml0c25o,,,MjAzLTMsIFdvbmRyYXNjaGVrLCBWb2xrZXIA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,1s0000l00nuiu,,,MjAwLTEsIExlaW5rYW1wLCBTZWJhc3RpYW4A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,200000l00nuiu,,,MjAwLTIA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,240000l00nuiu,,,MjAxLTEsIE1lcnRlbnMsIE1hcnRpbmEA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,280000l00nuiu,,,MjAxLTIA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,2c0000l00nuiu,,,MjAyLTEsIFNpZG93LCBKYW5pbgA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,2g0000l00nuiu,,,MjAyLTIA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,7s0000l00nuiu,,,OTAxLTEsIEJhcnRoYXVlciwgVGhvbWFzAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,840000l00nuiu,,,OTAwLTEsIEFocmVucywgQW5kcmVhAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,8c0000l00nuiu,,,MjAwLTMA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DK,8g0000l00nuiu,,,MjAwLTQA");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,000000t00nuiu,,,MTAwLCBNZWV0aW5nIFJvb20sIEFkbWluaXN0cmF0aW9uAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,040000t00nuiu,,,MTAxLCBPZmZpY2UgQWhyZW5kcywgQWRtaW5pc3RyYXRpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,080000t00nuiu,,,MTAyLCBPZmZpY2UgQmFydGhhdWVyLCBBZG1pbmlzdHJhdGlvbgA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,0c0000t00nuiu,,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,0g0000t00nuiu,,,MTA0LCBPZmZpY2UgU2FsZXMgMSwgQWRtaW5pc3RyYXRpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,0k0000t00nuiu,,,MTA1LCBPZmZpY2UgU2FsZXMgMiwgQWRtaW5pc3RyYXRpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,1c0000t00nuiu,,,MjAwLCBTdGVlbCBSZXBvc2l0b3J5LCBQcm9kdWN0aW9uAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,1g0000t00nuiu,,,MjAxLCBQcm9kdWN0IFJlcG9zaXRvcnksIFByb2R1Y3Rpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,1k0000t00nuiu,,,MjAyLCBPZmZpY2UgQXNzZW1ibHksIFByb2R1Y3Rpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,1s0000t00nuiu,,,MjA0LCBXb3Jrc2hvcCBXZXN0LCBQcm9kdWN0aW9uAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,200000t00nuiu,,,MjA1LCBXb3Jrc2hvcCBTb3V0aCwgUHJvZHVjdGlvbgA=");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,240000t00nuiu,,,MjA2LCBHYXRlIFdlc3QsIFByb2R1Y3Rpb24A");
		ELIApp2Drv( SYSTEM, JOB_ID, "DL,580000t00nuiu,,,WjEsIEVudHJhbmNlIFdlc3QsIEFkbWluaXN0cmF0aW9uAA==");
		ELIApp2Drv( SYSTEM, JOB_ID, "AT,000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z");
		ELIApp2Drv( SYSTEM, JOB_ID, "AT,040002vn1g25o,,20190212T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T150000Z,20190401T060000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T140000Z,20191028T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200212T150000Z");
		ELIApp2Drv( SYSTEM, JOB_ID, "AT,080002uc1k25o,,20190212T050000Z/54000/DW/20190330T200000Z,20190331T040000Z/54000/DW/20191026T190000Z,20191027T050000Z/54000/DW/20200212T200000Z");
		ELIApp2Drv( SYSTEM, JOB_ID, "AT,0c0002ua0s25o,,20181231T230000Z/63072000");
		ELIApp2Drv( SYSTEM, JOB_ID, "AT,0g0002u31s25o,,20190212T200000Z/32400/DW/20190330T050000Z,20190330T200000Z/28800,20190331T190000Z/32400/DW/20191026T040000Z,20191026T190000Z/36000,20191027T200000Z/32400/DW/20200212T050000Z");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,000000hqvs1lo,000002oe1g25o,0c0000t00nuiu,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,040000iavs1lo,000002oe1g25o,0g0000t00nuiu,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,080000ijvs1lo,000002oe1g25o,0k0000t00nuiu,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0c0000ml0c25o,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0c0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0g0000ml0c25o,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0g0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0k0000ml0c25o,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,0k0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,1s0000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,1s0000l00nuiu,040002vn1g25o,1c0000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,200000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,200000l00nuiu,040002vn1g25o,1c0000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,240000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,240000l00nuiu,040002vn1g25o,1g0000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,280000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,280000l00nuiu,040002vn1g25o,1g0000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2c0000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2c0000l00nuiu,040002vn1g25o,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2c0000l00nuiu,0g0002u31s25o,240000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2g0000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2g0000l00nuiu,040002vn1g25o,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,2g0000l00nuiu,0g0002u31s25o,240000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,7s0000l00nuiu,080002uc1k25o,000000t00nuiu,040000t00nuiu,080000t00nuiu,0c0000t00nuiu,0g0000t00nuiu,0k0000t00nuiu,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,840000l00nuiu,0c0002ua0s25o,000000t00nuiu,040000t00nuiu,080000t00nuiu,0c0000t00nuiu,0g0000t00nuiu,0k0000t00nuiu,1c0000t00nuiu,1g0000t00nuiu,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu,240000t00nuiu,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,8c0000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,8c0000l00nuiu,040002vn1g25o,1c0000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,8g0000l00nuiu,000002oe1g25o,580000t00nuiu");
		ELIApp2Drv( SYSTEM, JOB_ID, "AK,8g0000l00nuiu,040002vn1g25o,1c0000t00nuiu");


		// close connection to hardware (i.e. MQTT broker disconnect)
		const char* mySessionId = session_id_to_string(mySession);
		printf("ELIClose('%s','%s') => '%s'\n", SYSTEM, mySessionId, ELIClose(SYSTEM, mySessionId));
	}
	else 
		printf("[%s]\n", errorCode);
	
	//return 0;
	// split csv
	// retrieve first & second element 
	// first => EOK
	// second => sessionId 

	// approach Regular Expressions or spliting bei , ?


	// destroy the driver interface
	ELIDestroy();
	printf("ELIDestroy()\n");
	return 0;
}