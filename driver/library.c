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

#define CLIENT_ID		"driver"

#define TIMEOUT 5000L

#define QoS_FireAndForget	0
#define QoS_AtLeastOnce		1
#define QoS_ExactlyOnce		2

#define QoS 				QoS_AtLeastOnce

int mqtt_create(const char* serverURI) {
	MQTTClient_createOptions createOpts = MQTTClient_createOptions_initializer;
	createOpts.MQTTVersion = MQTTVERSION_5;
	int rc = MQTTClient_createWithOptions(&driverInfo->client, serverURI,
										  CLIENT_ID, MQTTCLIENT_PERSISTENCE_NONE, NULL, &createOpts);

	if (rc != MQTTCLIENT_SUCCESS)
	{
		MQTTClient_destroy(&driverInfo->client);
	}
	return rc;
}

void mqtt_destroy() {
	MQTTClient_destroy(&driverInfo->client);
}

int mqtt_connect() {
	MQTTClient_connectOptions conn_opts = MQTTClient_connectOptions_initializer;
	conn_opts.keepAliveInterval = 0;
	conn_opts.cleansession = false;
	conn_opts.cleanstart = true; 
	conn_opts.MQTTVersion = MQTTVERSION_5;

	MQTTResponse response = MQTTResponse_initializer;

	MQTTProperties props = MQTTProperties_initializer;

	response = MQTTClient_connect5(driverInfo->client, &conn_opts, &props, NULL);
	int rc = response.reasonCode;

	MQTTProperties_free(&props);
	if (rc != MQTTCLIENT_SUCCESS)
	{
		printf("Failed to connect, return code %d\n", rc);
		return rc;
	}
	return rc;
}

int mqtt_disconnect() {
	return MQTTClient_disconnect(driverInfo->client, 1000);
}

int mqtt_publish(const char* topic, const char* payload, int qos, const char* correlationId, const char* replyTo) {

	MQTTClient_deliveryToken token;

	MQTTClient_message pubmsg = MQTTClient_message_initializer;
	pubmsg.payload = (char*)payload;
	pubmsg.payloadlen = (int)strlen(payload) + 1;

	if (strlen(payload) < 20)	// padding up small messages
	{
		size_t needed = snprintf(NULL, 0, "%-20s", payload);
		char  *buffer = malloc(needed+1);
		sprintf(buffer, "%-20s", payload);

		pubmsg.payload = buffer;
		pubmsg.payloadlen = needed;
	}

	pubmsg.qos = qos;
	pubmsg.retained = false;

	MQTTProperty property;
	property.identifier = MQTTPROPERTY_CODE_RESPONSE_TOPIC;
	property.value.data.data = replyTo;
	property.value.data.len = (int)strlen(property.value.data.data);
	MQTTProperties_add(&pubmsg.properties, &property);

	property.identifier = MQTTPROPERTY_CODE_CORRELATION_DATA;
	property.value.data.data = correlationId;
	property.value.data.len = (int)strlen(property.value.data.data);
	MQTTProperties_add(&pubmsg.properties, &property);

	MQTTResponse resp;

	resp = MQTTClient_publishMessage5(driverInfo->client, topic, &pubmsg, &token);
	int rc = resp.reasonCode;
	
	if (strlen(payload) < 20)
	{
		free(pubmsg.payload);
	}

	MQTTProperties_free(&pubmsg.properties);

	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to publish, return code %d\n", rc);
		return rc;
	}

	if (qos == QoS_FireAndForget)
		return rc;

	rc = MQTTClient_waitForCompletion(driverInfo->client, token, TIMEOUT);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Message publish timed out, return code %d\n", rc);
		return rc;
	}
	return MQTTCLIENT_SUCCESS;
}

int mqtt_subscribe(const char* topic, int qos) {
	MQTTSubscribe_options subopts = MQTTSubscribe_options_initializer;
	subopts.retainAsPublished = false;
	subopts.noLocal = true;

	MQTTProperties props = MQTTProperties_initializer;
	int rc = MQTTClient_subscribe5(driverInfo->client, topic, qos, &subopts, &props).reasonCode;
	MQTTProperties_free(&props);
	if (rc != qos) {
		printf("Failed to subscribe '%s' return code %d\n", topic, rc);
	}
	return MQTTCLIENT_SUCCESS;
}

int mqtt_unsubscribe(const char* topic) {
	MQTTProperties props = MQTTProperties_initializer;
	int rc = MQTTClient_unsubscribe5(driverInfo->client, topic, &props).reasonCode;
	MQTTProperties_free(&props);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to unsubscribe return code %d\n", rc);
	}
	return rc;
}

int mqtt_receive_msg(const char *topic, int timeout, char **payload, char **correlationId)
{
	char* topicName = NULL;
	int topicLen;
	MQTTClient_message* msg = NULL;

	int rc = MQTTClient_receive(driverInfo->client, &topicName, &topicLen, &msg, timeout);
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("Failed to receive, return code %d\n", rc);
		return rc;
	}

	if (topicName)
	{
		if (msg != NULL) {
			*payload = strndup((char*)(msg->payload), msg->payloadlen);

			if (MQTTProperties_hasProperty(&msg->properties, MQTTPROPERTY_CODE_CORRELATION_DATA))
			{
				MQTTProperty *prop = MQTTProperties_getProperty(&msg->properties, MQTTPROPERTY_CODE_CORRELATION_DATA);
				*correlationId = strndup((char *)(prop->value.data.data), prop->value.data.len);
			}
			MQTTClient_freeMessage(&msg);

		}
		MQTTClient_free(topicName);
	};

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
	char *replyTo = topic_replyTo(node->sSystem, CLIENT_ID);
	
	rc = mqtt_subscribe(replyTo, QoS);

	if (rc != MQTTCLIENT_SUCCESS) {
		printf("no possible to subscripe to '%s' \n", replyTo);
		return "EUNKNOWN,,,,0";
	}
	free(replyTo);

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

	char *replyTo = topic_replyTo(node->sSystem, CLIENT_ID);
	int rc = mqtt_unsubscribe(replyTo);
	free(replyTo);
	free(sessionID);
	
	if (rc != MQTTCLIENT_SUCCESS)
	{
		printf("mqtt_unsubscripe() => %i\n", rc);
		return "EUNKNOWN";
	}	

	// disconnet from mqtt broker
	rc = mqtt_disconnect();
	if (rc != MQTTCLIENT_SUCCESS) {
		printf("mqtt_disconnect() => %i\n", rc);
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

	char *sSessionID = session_id_to_string(node->session_id);
	char *replyTo = topic_replyTo(node->sSystem, sSessionID);
	int rc = mqtt_publish(node->sSystem, sJobData, QoS, sJobID, replyTo);
	free(sSessionID);

	if (rc != MQTTCLIENT_SUCCESS) {
		printf("not publish to %s retcode %d \n", node->sSystem, rc);
		return -1;
	}

	//  "response" cames separatly and asynchron via ELIDrv2App
	char* payload = NULL;
	char *correlationId = NULL;
	rc = mqtt_receive_msg(replyTo, 100L, &payload, &correlationId);
	free(replyTo);

	if (payload)
	{
		if (driverInfo->callback != NULL)
		{
			driverInfo->callback(sSysID, (correlationId) ? correlationId : sJobID, payload);
		}
		free(payload);
	}

	return 0;
}