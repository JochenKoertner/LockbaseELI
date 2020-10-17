#include <stdio.h>
#include <stdbool.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>

#include "MQTTClient.h"


#include "library.h"
#include "utils.h"
#include "session_list.h"
#include "driver.h"

#define CLIENT_ID		"Alice"

#define TIMEOUT				1000L

#define QoS_FireAndForget	0
#define QoS_AtLeastOnce		1
#define QoS_ExactlyOnce		2


#define RESPONSE_TOPIC		"respond"

int mqtt_create(const char* serverURI) {
	return MQTTClient_create(&driverInfo->client, serverURI, CLIENT_ID, MQTTCLIENT_PERSISTENCE_NONE, NULL);
}

void mqtt_destroy() {
	MQTTClient_destroy(&driverInfo->client);
}

int mqtt_connect() {
	MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;
	conn_opts.keepAliveInterval = 20;
	conn_opts.cleansession = 1;
	int rc ;

	if ((rc = MQTTClient_connect(driverInfo->client, &conn_opts)) != MQTTCLIENT_SUCCESS) // TODO: Hier kommt er nicht zurück??
	{
		printf("Failed to connect, return code %d\n", rc);
		return rc;
	}
	return rc;
}

int mqtt_disconnect() {
	return MQTTClient_disconnect(driverInfo->client, 1000);
}

int mqtt_publish(const char* topic, const char* payload, int qos) {

	MQTTClient_deliveryToken token;

	MQTTClient_message pubmsg = MQTTClient_message_initializer;
	pubmsg.payload = (char*)payload;
	pubmsg.payloadlen = (int) strlen(payload);
	pubmsg.qos = qos;
	pubmsg.retained = 0;

	int rc = MQTTClient_publishMessage(driverInfo->client, topic, &pubmsg, &token);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to publish, return code %d\n", rc);
		return rc;
	}

	if (qos == QoS_FireAndForget)
		return rc;

	printf("Waiting for up to %d seconds for publication of %s\n"
		   "on topic %s for client with ClientID: %s\n",
		   (int) (TIMEOUT / 1000), "sss", topic, CLIENT_ID);
	rc = MQTTClient_waitForCompletion(driverInfo->client, token, TIMEOUT);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Message publish timed out, return code %d\n", rc);
		return rc;
	}
	return token;
}

int mqtt_subscribe(const char* topic, int qos) {
	int rc = MQTTClient_subscribe(driverInfo->client, topic, qos);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to subscribe '%s' return code %d\n", topic, rc);
	}
	return rc;
}

int mqtt_unsubscribe(const char* topic) {
	int rc = MQTTClient_unsubscribe(driverInfo->client, topic);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to unsubscribe return code %d\n", rc);
	}
	return rc;
}

int mqtt_receive_msg(const char* topic, int timeout, char** payload) {
	char* topicName = NULL;
	int topicLen;
	MQTTClient_message* msg = NULL;

	int rc = mqtt_subscribe(topic, QoS_ExactlyOnce);
	if (rc != MQTTCLIENT_SUCCESS) {
		return rc;
	}

	if ((rc = MQTTClient_receive(driverInfo->client, &topicName, &topicLen, &msg, timeout)) != MQTTCLIENT_SUCCESS) {
		mqtt_unsubscribe(topic);
		printf("Failed to receive, return code %d\n", rc);
		return rc;
	}

	if (topicName) {
		if (msg != NULL) {
			*payload = strndup((char*)(msg->payload), msg->payloadlen);
			// printf("Message received on topic %s is %.*s", topicName, msg->payloadlen, (char*)(msg->payload));
			MQTTClient_freeMessage(&msg);
		}
		MQTTClient_free(topicName);
	};

	rc = mqtt_unsubscribe(topic);
	if (rc != MQTTCLIENT_SUCCESS)
		return rc;

	return MQTTCLIENT_SUCCESS;
}



/*
*  LbwELI() is the constructor of the interface object, which is required to use the interface. It expects the
*  application to identify itself by its licence and LbwELI revision number, which allows the driver to support
*  different interface revisions and adjust its services to different types of installations (e.g. factory, locksmith
*  or end user installation). Secondly the constructor expects a callback function pointer to launch the driver's
*  responses to job requests (s.b.).
*  In case the constructor supports the requested interface revision, it simply returns EOK. In case it does not,
*  it returns EREV, followed by the best interface it supports:
*       [ID:Error],[TXT:DrvELIRev]
*   This allows the application to 'downgrade' to an older drivers capabilities. If the driver cannot start
*   for ny other reason, the constructor returns EUNKNOWN.
*/

LBELI_EXPORT const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2App callback ) {
	if (strcmp(sLbwELIRev, LbwELI_VERSION) != 0) {
		return u8"EREV,"LbwELI_VERSION;
	}

	driverInfo = new_driver(callback);

	const char* url = strcasecmp(sLic,"vbox") == 0 
		? "tcp://10.0.2.2:1883" : formatUrl("tcp", driverInfo->host, driverInfo->port);
	int ret = mqtt_create(url);
	if (ret != MQTTCLIENT_SUCCESS) {
		return "EUNKNOWN";
	}

	return u8"OK,"LbwELI_VERSION;
}

/*
* ELIDestroy() is the destructor of the interface object, it will be called on application termination or when
* the application detaches from the driver for other reasons. The application will call the destructor anyway,
* even if the constructor did not returned EOK.
*/
LBELI_EXPORT void ELIDestroy() {
	mqtt_destroy();

	free_driver(driverInfo);
	driverInfo = NULL;
}

/*
* To retrieve global information about the driver the application will call the interface function DriverInfo()
*
* The function returns a text pointer containing definitions for the following variables (s.b.).
* The returned text must be available for the application at least until the next call of an interface function.
* The information returned by DriverInfo() is expected to be unchanged within the same driver revision.
*/

LBELI_EXPORT const char* ELIDriverInfo() {
	parseDriverInfo(driverInfo->config, &driverInfo->driverInfo);
	return driverInfo->driverInfo;
}

/*
* ELIDriverUI() will receive the application's current session id and the id of the current active system as
* parameters (s.b.). This variable is optional, by default no driver provided user interface will be supported.
*/

LBELI_EXPORT void ELIDriverUI(const char* SessID, const char* SID) {
	printf("__v__ELIDriverUI()\n");
	printf("Start Browser  SessID='%s'  SID='%s'\n", SessID, SID);
}

/*
* The device capabilities and limitations are regarded to be product dependent.
* They will be requested by a call to the ProductInfo() method
*
* The function returns a text pointer containing definitions for the following variables (s.b.).
* The returned text must be available for the application at least until the next call of an interface function.
* The information returned by ProductInfo() is expected to be unchanged within the same driver revision.
*/

LBELI_EXPORT const char* ELIProductInfo( const char* sProductID ) {
	parseProductInfo(driverInfo->config, sProductID, &driverInfo->productInfo);
	return driverInfo->productInfo;
}

LBELI_EXPORT const char* ELISystemInfo( const char* sUsers ) {
	parseSystemInfo(driverInfo->config, &driverInfo->systemInfo);
	return driverInfo->systemInfo;
}

LBELI_EXPORT const char* ELIOpen( const char* sUserList, const char* sSysID, const char* sExtData) {
	printf("__v__ELIOpen(%s)\n",sSysID);
	// connect to the broker
	int rc = mqtt_connect();
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("mqtt_connect() => %i\n", rc);
		return "EUNKNOWN,,,,0";
	}

	node_t * node = find_system(driverInfo->sessions, sSysID);
	bool isNewSession = !node;
	if (isNewSession)
	{
		printf("system %s unknown\n", sSysID);
		node = new_session(&driverInfo->sessions, sUserList, sSysID, sExtData);
		printf("create new session for '%s' \n", sSysID);
	}
	else {
		update_session(node, sUserList, sExtData);
	}

	char* sSessID = session_id_to_string(node->session_id);
	char* message = create_event_payload("ELIOpen", sSessID, "OPEN,sSystem,sExtData", RESPONSE_TOPIC);
	rc = mqtt_publish(sSysID, message, QoS_FireAndForget);
	free(message);
	if (rc != MQTTCLIENT_SUCCESS) {
		free(sSessID);
		return "EUNKNOWN,,,,0";
	}
	
	const char* sessionId = isNewSession ? "" : sSessID;

	printf("__^__ELIOpen(%s)\n",sSessID);
	static char buf[100];
	sprintf(buf, "%s,%s,ACLR,%s,%i", "OK", node->sSystem, sessionId, node->state);
	free(sSessID);
	return buf;
}

/* 
* By using the Close() function the application signals the end of a session to the driver. 
* The function expects the system id and a session id defined by the application. The driver 
* must store the session id with the system and return it in the next call of ELIOpen() for 
* this system (s.a.).The ELIClose() function returns a CSV record of the following 
* form:[ID:Error],[B64:ExtData]
*
* In case the function succeeds, the error field contains 'OK' and is (optionally) followed
* by a binarydata block to store with the system in the application's database. In case an 
* error occurs the errorfield contains one of the defined error codes (see 'Data Exchange, 
* Jobs and Statements, Error Codes') and the following fields are omitted.
*/

LBELI_EXPORT const char* ELIClose( const char* sSysID, const char* sSessID ) {
	printf("__v__ELIClose(%s)\n",sSessID);
	int session_id = string_to_session_id(sSessID);

	node_t * node = find_system(driverInfo->sessions, sSysID);
	if (!node)
	{
		printf("system %s unknown\n", sSysID);
		return "EUNKNOWN";
	}
	node->last_session_id = session_id;

	// node_t * node = find_session(driverInfo->sessions, session_id);
	// if (!node)
	// {
	// 	printf("session %s unknown\n", sSessID);
	// 	return "EUNKNOWN";
	// }

	char* sessionID = session_id_to_string(node->session_id);

	char* message = create_event_payload("ELIClose", sessionID, "CLOSE,session", RESPONSE_TOPIC);
	int rc = mqtt_publish(node->sSystem, message, QoS_FireAndForget);
	free(message);
	free(sessionID);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("not publish to %s retcode %d \n", node->sSystem, rc);
		return "ECONNECTION";
	}

	// remove the session from the list
	// remove_session(&driverInfo->sessions, session_id);

	// disconnet from mqtt broker
	int ret = mqtt_disconnect();
	if (ret != MQTTCLIENT_SUCCESS) {
		printf("mqtt_disconnect() => %i\n", ret);
		return "EUNKNOWN";
	}
	printf("__^__ELIClose(%s)\n",sSessID);
	return "OK";
}

LBELI_EXPORT int ELIApp2Drv( const char* sSysID, const char *sJobID, const char* sJobData) {
	node_t * node = find_system(driverInfo->sessions, sSysID);
	if (!node)
	{
		printf("system %s unknown\n", sSysID);
		return -1;
	}

	int session_id = node->session_id;

	char* sSessionID = session_id_to_string(session_id);

	char* message = create_event_payload("ELIApp2Drv", sSessionID, sJobData, RESPONSE_TOPIC);
	int rc = mqtt_publish(node->sSystem, message, QoS_FireAndForget);
	free(message);
	free(sSessionID);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("not publish to %s retcode %d \n", node->sSystem, rc);
		return -1;
	}


	//  "response" cames separatly and asynchron via ELIDrv2App
	char* payload = NULL;
	rc = mqtt_receive_msg(RESPONSE_TOPIC, 100L, &payload);

	if (payload) {
		char* sessionId = NULL;
		char* text = NULL;
		parse_payload(payload, &sessionId, &text);
		if (driverInfo->callback != NULL)
		{
			driverInfo->callback(sSysID, sJobID, text);
		}

		free(sessionId);
		free(text);
		free(payload);
	}
	else
	{
		printf("no response\n");
	}
	return 0;
}