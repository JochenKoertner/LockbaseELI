// https://www.learn-c.org/en/Linked_lists

// linked list that holds all sessions that given by ELIOpen

#include <stddef.h>
#include <stdlib.h>
#include <string.h>

#include "session_list.h"

node_t* new_session(node_t** head, const char * sUserList, const char* sSystem, const char* sExtData) {
    node_t* new_node;
    new_node = malloc(sizeof(node_t));

    new_node->session_id = rand();
    if (*head != NULL)
    {
        new_node->last_session_id = (*head)->session_id;
    }
    else
    {
        new_node->last_session_id = 0;
    }
    new_node->sUserList = strdup(sUserList);
    new_node->sSystem = strdup(sSystem);
    new_node->sExtData = strdup(sExtData);
    new_node->next = *head;
    *head = new_node;

    return new_node;
}

void free_session(node_t * node) {
    free(node->sUserList);
    free(node->sSystem);
    free(node->sExtData);
    free(node);
}

void pop_session(node_t ** head) {
    node_t* next_node = NULL;

    if (*head == NULL) {
        return;
    }

    next_node = (*head)->next;
    free_session(*head);
    *head = next_node;
}

node_t* find_session(node_t* head, int session_id ) {
    node_t* current = head;

    while (current != NULL) {
        if (current->session_id == session_id)
            return current;
        current = current->next;
    }

    return NULL;
}

node_t* find_system(node_t* head, const char* sSystem ) {
    node_t* current = head;

    while (current != NULL) {
        if (strcmp(current->sSystem, sSystem)==0)
            return current;
        current = current->next;
    }

    return NULL;
}

void remove_session(node_t** head, int session_id) {

    int i = 0;
    node_t* current = *head;
    node_t* temp_node = NULL;

    if (current->session_id == session_id) {
        pop_session(head);
        return;
    }

    while (current != NULL) {
        temp_node = current;
        current = current->next;
        if (current != NULL && current->session_id == session_id) {
            temp_node->next = current->next;
            free_session(current);
            return;
        }
    }
}