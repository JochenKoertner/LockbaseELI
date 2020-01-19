
#ifndef SESSION_LIST_H
#define SESSION_LIST_H

typedef struct node {
    int session_id;
    int last_session_id;
    char * sUserList;
    char * sSystem;
    char * sExtData;
    struct node * next;
} node_t;

node_t * new_session(node_t ** head, const char * sUserList, const char * sSystem, const char * sExtData);
node_t * find_session(node_t * head, int session_id);
node_t * find_system(node_t * head, const char * sSystem);
void remove_session(node_t ** head, int session_id);

#endif //SESSION_LIST_H
