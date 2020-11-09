#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include "library.h"
#include "utils.h"
#include "driver.h"

#define CLIENT_ID "Alice"
#define SYSTEM "ELIDemo"
#define TIMEOUT 10000L


// Mit 'vbox' gehe ich zum MacOs Host
// Mit 'lic' gehe ich zum lokalen Mqtt Server

//#if defined (WIN32)
//	#define LICENCE  "vbox"
//#else
#define LICENCE "lic"
//#endif

// const char* sSysID, const char* sJobID, const char* sJobData
int myCallBack(const char *sSysID, const char *sJobID, const char *sJobData)
{
	printf("DRV2APP [%s] [%s] BEGIN\n", sSysID, sJobID);
    printf("%s\n", sJobData);
	printf("END\n");
    return 0;
}

#if defined(WIN32)
#include <conio.h>
#else
char getch()
{
    system("/bin/stty raw");
    int ch = getchar();
    system("/bin/stty cooked");
    return ch;
}
#endif

char* jobId = NULL; 

const char *getField(const char *line, int num)
{
    static char _result[100];
    const char *prev;
    prev = line;
    int len = 0;
    while (num > 0)
    {
        if (prev != NULL)
        {
            char *separator = strchr(prev, ',');
            if (separator != NULL)
            {
                len = separator - prev;
                if (num == 1)
                {
                    strncpy(_result, prev, len);
                }
                prev = separator + 1;
                if (num == 1)
                {
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

void send_initial_setup()
{
   ELIApp2Drv(SYSTEM, jobId,
	          "DK,000000hqvs1lo,103-1,,MTAzLTEsIEZlbmRlciwgS2xhdXMA\n"
               "DK,040000iavs1lo,104-1,,MTA0LTEsIEtpc3RsZXIsIFNhYmluZQA=\n"
               "DK,080000ijvs1lo,105-1,,MTA1LTEsIEtvaGwsIFVscmljaAA=\n"
               "DK,0c0000ml0c25o,203-1,,MjAzLTEsIFdhbHRlciwgSmVucwA=\n"
               "DK,0g0000ml0c25o,203-2,,MjAzLTIsIFdpbnRlciwgU2luYQA=\n"
               "DK,0k0000ml0c25o,203-3,,MjAzLTMsIFdvbmRyYXNjaGVrLCBWb2xrZXIA\n"
               "DK,1s0000l00nuiu,200-1,,MjAwLTEsIExlaW5rYW1wLCBTZWJhc3RpYW4A\n"
               "DK,200000l00nuiu,200-2,,MjAwLTIA\n"
               "DK,240000l00nuiu,201-1,,MjAxLTEsIE1lcnRlbnMsIE1hcnRpbmEA\n"
               "DK,280000l00nuiu,201-2,,MjAxLTIA\n"
               "DK,2c0000l00nuiu,202-1,,MjAyLTEsIFNpZG93LCBKYW5pbgA=\n"
               "DK,2g0000l00nuiu,202-2,,MjAyLTIA\n"
               "DK,7s0000l00nuiu,901-1,,OTAxLTEsIEJhcnRoYXVlciwgVGhvbWFzAA==\n"
               "DK,840000l00nuiu,900-1,,OTAwLTEsIEFocmVucywgQW5kcmVhAA==\n"
               "DK,8c0000l00nuiu,200-3,,MjAwLTMA\n"
               "DK,8g0000l00nuiu,200-4,,MjAwLTQA\n"
               "DL,000000t00nuiu,100,,MTAwLCBNZWV0aW5nIFJvb20sIEFkbWluaXN0cmF0aW9uAA==\n"
               "DL,040000t00nuiu,101,,MTAxLCBPZmZpY2UgQWhyZW5kcywgQWRtaW5pc3RyYXRpb24A\n"
               "DL,080000t00nuiu,102,,MTAyLCBPZmZpY2UgQmFydGhhdWVyLCBBZG1pbmlzdHJhdGlvbgA=\n"
               "DL,0c0000t00nuiu,103,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=\n"
               "DL,0g0000t00nuiu,104,,MTA0LCBPZmZpY2UgU2FsZXMgMSwgQWRtaW5pc3RyYXRpb24A\n"
               "DL,0k0000t00nuiu,105,,MTA1LCBPZmZpY2UgU2FsZXMgMiwgQWRtaW5pc3RyYXRpb24A\n"
               "DL,1c0000t00nuiu,200,,MjAwLCBTdGVlbCBSZXBvc2l0b3J5LCBQcm9kdWN0aW9uAA==\n"
               "DL,1g0000t00nuiu,201,,MjAxLCBQcm9kdWN0IFJlcG9zaXRvcnksIFByb2R1Y3Rpb24A\n"
               "DL,1k0000t00nuiu,202,,MjAyLCBPZmZpY2UgQXNzZW1ibHksIFByb2R1Y3Rpb24A\n"
               "DL,1s0000t00nuiu,204,,MjA0LCBXb3Jrc2hvcCBXZXN0LCBQcm9kdWN0aW9uAA==\n"
               "DL,200000t00nuiu,205,,MjA1LCBXb3Jrc2hvcCBTb3V0aCwgUHJvZHVjdGlvbgA=\n"
               "DL,240000t00nuiu,206,,MjA2LCBHYXRlIFdlc3QsIFByb2R1Y3Rpb24A\n"
               "DL,580000t00nuiu,Z1,,WjEsIEVudHJhbmNlIFdlc3QsIEFkbWluaXN0cmF0aW9uAA==\n"
               "AT,000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z\n"
               "AT,040002vn1g25o,,20190212T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T150000Z,20190401T060000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T140000Z,20191028T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200212T150000Z\n"
               "AT,080002uc1k25o,,20190212T050000Z/54000/DW/20190330T200000Z,20190331T040000Z/54000/DW/20191026T190000Z,20191027T050000Z/54000/DW/20200212T200000Z\n"
               "AT,0c0002ua0s25o,,20181231T230000Z/63072000\n"
               "AT,0g0002u31s25o,,20190212T200000Z/32400/DW/20190330T050000Z,20190330T200000Z/28800,20190331T190000Z/32400/DW/20191026T040000Z,20191026T190000Z/36000,20191027T200000Z/32400/DW/20200212T050000Z\n"
               "AK,000000hqvs1lo,000002oe1g25o,0c0000t00nuiu,580000t00nuiu\n"
               "AK,040000iavs1lo,000002oe1g25o,0g0000t00nuiu,580000t00nuiu\n"
               "AK,080000ijvs1lo,000002oe1g25o,0k0000t00nuiu,580000t00nuiu\n"
               "AK,0c0000ml0c25o,000002oe1g25o,580000t00nuiu\n"
               "AK,0c0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu\n"
               "AK,0g0000ml0c25o,000002oe1g25o,580000t00nuiu\n"
               "AK,0g0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu\n"
               "AK,0k0000ml0c25o,000002oe1g25o,580000t00nuiu\n"
               "AK,0k0000ml0c25o,040002vn1g25o,1s0000t00nuiu,200000t00nuiu\n"
               "AK,1s0000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,1s0000l00nuiu,040002vn1g25o,1c0000t00nuiu\n"
               "AK,200000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,200000l00nuiu,040002vn1g25o,1c0000t00nuiu\n"
               "AK,240000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,240000l00nuiu,040002vn1g25o,1g0000t00nuiu\n"
               "AK,280000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,280000l00nuiu,040002vn1g25o,1g0000t00nuiu\n"
               "AK,2c0000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,2c0000l00nuiu,040002vn1g25o,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu\n"
               "AK,2c0000l00nuiu,0g0002u31s25o,240000t00nuiu\n"
               "AK,2g0000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,2g0000l00nuiu,040002vn1g25o,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu\n"
               "AK,2g0000l00nuiu,0g0002u31s25o,240000t00nuiu\n"
               "AK,7s0000l00nuiu,080002uc1k25o,000000t00nuiu,040000t00nuiu,080000t00nuiu,0c0000t00nuiu,0g0000t00nuiu,0k0000t00nuiu,580000t00nuiu\n"
               "AK,840000l00nuiu,0c0002ua0s25o,000000t00nuiu,040000t00nuiu,080000t00nuiu,0c0000t00nuiu,0g0000t00nuiu,0k0000t00nuiu,1c0000t00nuiu,1g0000t00nuiu,1k0000t00nuiu,1s0000t00nuiu,200000t00nuiu,240000t00nuiu,580000t00nuiu\n"
               "AK,8c0000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,8c0000l00nuiu,040002vn1g25o,1c0000t00nuiu\n"
               "AK,8g0000l00nuiu,000002oe1g25o,580000t00nuiu\n"
               "AK,8g0000l00nuiu,040002vn1g25o,1c0000t00nuiu\n"); 
}

const char *createSession()
{
    int mySession = rand();
    printf("1st Session '%08X' \n\n", mySession);
    return session_id_to_string(mySession);
}

int main()
{
    printf("PID: '%d'\n", getpid());
	jobId = job_id_to_string(getpid());

    // initialise driver interface and register a callback function
    const char *retCode = ELICreate(LICENCE, LbwELI_VERSION, myCallBack);
    printf("ELICreate(...) => '%s'\n\n", retCode);
    if (strcmp(retCode, "OK,0.4") != 0)
    {
        return -1;
    }

    printf("MQTT on Host: '%s' Port: %ld\n", driverInfo->host, driverInfo->port);

    // dump the driver-info to console
    const char *driverInfo = ELIDriverInfo();
    printf("ELIDriverInfo() => \n%s\n\n", driverInfo);

    // dump product-info to console
    const char *productId = PRODUCT_ID;
    const char *productInfos = ELIProductInfo(productId);
    printf("ELIProductInfo('%s') => \n%s\n\n", productId, productInfos);

    // dump system-info to console
    const char *users = "users";
    const char *systemInfo = ELISystemInfo(users);
    printf("ELISystemInfo('%s') => \n%s\n\n", users, systemInfo);

    // call ELIDriverUI
    // ELIDriverUI( "sessionID", "SID");

    // First session
    const char *mySessionId = createSession();


	const char *csv = ELIOpen("jk", SYSTEM, CLIENT_ID);
	const char *errorCode = getField(csv, 1);
	if (strcmp(errorCode, "OK") == 0)
	{
		const char *tmp = getField(csv, 4); //TODO funktioniert nicht bei ACLR,,1 !!
		const char *session = tmp == errorCode ? "" : tmp;

		printf("ELIOpen(...) => '%s' (%s)\n", retCode, session);
	}
	else
	{
		printf("[%s]\n", errorCode);
	}

    printf("Press Any Key to Continue\n");
    printf(" - 'I' => Initsequence (DL,DK,AK,...)\n");
    printf(" - 'D' => List Data (LD)\n");
    printf(" - 'E' => List Events (LE)\n");
    printf(" - 'Q' => Quit\n");
    char ch = getch();
    while (ch != 'q' && ch != 'Q')
    {
		printf(".");
        // Init Sequence
        if ((ch == 'i') || (ch == 'I'))
        {
			send_initial_setup();
        }
        // List Data command
        else if ((ch == 'd') || (ch == 'D'))
        {
            printf("List Data (LD)\n");
            ELIApp2Drv(SYSTEM, jobId, "LD");
        }
        // List Events command
        else if ((ch == 'e') || (ch == 'E'))
        {
            printf("List Events (LE)\n");
            ELIApp2Drv(SYSTEM, jobId, "LE"); // 20200213T142758Z
        }
        else if ((ch == 'q') || (ch == 'Q'))
        {
        }
        ch = getch();
    }
    printf("Quit and Close\n");
    printf("ELIClose('%s','%s') => '%s'\n", SYSTEM, mySessionId, ELIClose(SYSTEM, mySessionId));

    // destroy the driver interface
    ELIDestroy();
    printf("ELIDestroy()\n");

	free(jobId);
    return 0;
}