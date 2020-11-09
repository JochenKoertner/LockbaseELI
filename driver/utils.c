#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "utils.h"

char *session_id_to_string(int session_id)
{
	size_t needed = snprintf(NULL, 0, "%08X", session_id);
	char *buffer = malloc(needed + 1);
	sprintf(buffer, "%08X", session_id);
	return buffer;
}

char *job_id_to_string(int job_id)
{
	size_t needed = snprintf(NULL, 0, "%08X", job_id);
	char *buffer = malloc(needed + 1);
	sprintf(buffer, "%08X", job_id);
	return buffer;
}

int string_to_session_id(const char *sSessID)
{
	return (int)strtol(sSessID, NULL, 16);
}

char *topic_replyTo(const char *topic, const char *clientId)
{
	size_t needed = snprintf(NULL, 0, "%s/%s/result", topic, clientId);
	char *buffer = malloc(needed + 1);
	sprintf(buffer, "%s/%s/result", topic, clientId);
	return buffer;
}

char *formatUrl(const char *protocol, const char *host, long port)
{
	static char address[100];
	sprintf(address, "%s://%s:%ld", protocol, host ? host : "localhost", port);
	return address;
}

char *create_event_payload(const char *sText)
{
	const char *fmt = u8"{text: '%s'}";
	size_t needed = snprintf(NULL, 0, fmt, sText);
	char *buffer = malloc(needed + 1);
	sprintf(buffer, fmt, sText);
	buffer[needed + 1] = 0;
	return buffer;
}

#ifdef WIN32
char *strndup(const char *str, unsigned int chars)
{
	char *buffer;
	int n;

	buffer = (char *)malloc(chars + 1);
	if (buffer)
	{
		for (n = 0; ((n < chars) && (str[n] != 0)); n++)
			buffer[n] = str[n];
		buffer[n] = 0;
	}

	return buffer;
}
#endif

void parse_payload(const char *json, char **text)
{

	int i;
	int r;
	jsmn_parser p;
	jsmntok_t t[128]; /* We expect no more than 128 tokens */

	jsmn_init(&p);
	r = jsmn_parse(&p, json, strlen(json), t, sizeof(t) / sizeof(t[0]));
	if (r < 0)
	{
		printf("Failed to parse JSON: %d\n", r);
	}

	/* Assume the top-level element is an object */
	if (r < 1 || t[0].type != JSMN_OBJECT)
	{
		printf("Object expected\n");
	}

	/* Loop over all keys of the root object */
	for (i = 1; i < r; i++)
	{
		if (jsoneq(json, &t[i], "text") == 0)
		{
			*text = strndup(json + t[i + 1].start, t[i + 1].end - t[i + 1].start);
			i++;
		}
	}
}

char *string_alloc(const char *source, int len)
{
	return strndup(source, len);
}