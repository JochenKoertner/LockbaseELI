cmake_minimum_required(VERSION 3.12)

IF (WIN32)
    SET(LIBS_SYSTEM ws2_32)
ENDIF()

set(CMAKE_C_STANDARD 11)

## Build JSMN Library ----------------------------------------------
project(Jsmn C)

IF(WIN32)
  ADD_DEFINITIONS(-D_CRT_SECURE_NO_DEPRECATE -DWIN32_LEAN_AND_MEAN -MD)
ELSEIF(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  ADD_DEFINITIONS(-DOSX)
ENDIF()

set(PROJECT_SOURCE_DIR ${PROJECT_SOURCE_DIR}/jsmn)

add_library(${PROJECT_NAME} 
  ${PROJECT_SOURCE_DIR}/jsmn.c 
  ${PROJECT_SOURCE_DIR}/jsmn.h)

## Build PahoMqtt Library ------------------------------------------
project(PahoMqtt C)

IF(WIN32)
  ADD_DEFINITIONS(-D_CRT_SECURE_NO_DEPRECATE -DWIN32_LEAN_AND_MEAN -MD)
ELSEIF(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  ADD_DEFINITIONS(-DOSX)
ENDIF()

## build settings
set(PAHO_VERSION_MAJOR 1)
set(PAHO_VERSION_MINOR 2)
set(PAHO_VERSION_PATCH 1)
set(CLIENT_VERSION ${PAHO_VERSION_MAJOR}.${PAHO_VERSION_MINOR}.${PAHO_VERSION_PATCH})

string(TIMESTAMP BUILD_TIMESTAMP UTC)
message(STATUS "Timestamp is ${BUILD_TIMESTAMP}")

set(PROJECT_SOURCE_DIR ${PROJECT_SOURCE_DIR}/paho.mqtt.c/src)

file(GLOB SOURCES_PAHO
        ${PROJECT_SOURCE_DIR}/Heap.c
        ${PROJECT_SOURCE_DIR}/LinkedList.c
        ${PROJECT_SOURCE_DIR}/Log.c
        ${PROJECT_SOURCE_DIR}/Messages.c
        ${PROJECT_SOURCE_DIR}/StackTrace.c
        ${PROJECT_SOURCE_DIR}/Tree.c
        ${PROJECT_SOURCE_DIR}/Thread.c
        ${PROJECT_SOURCE_DIR}/utf-8.c
        ${PROJECT_SOURCE_DIR}/WebSocket.c
        ${PROJECT_SOURCE_DIR}/MQTTPacket.c
        ${PROJECT_SOURCE_DIR}/MQTTPacketOut.c
        ${PROJECT_SOURCE_DIR}/MQTTProperties.c
        ${PROJECT_SOURCE_DIR}/MQTTClient.c
        )
file(GLOB INCLUDE_PAHO
        ${PROJECT_SOURCE_DIR}/Heap.h
        ${PROJECT_SOURCE_DIR}/Log.h
        ${PROJECT_SOURCE_DIR}/Messages.h
        ${PROJECT_SOURCE_DIR}/LinkedList.h
        ${PROJECT_SOURCE_DIR}/StackTrace.h
        ${PROJECT_SOURCE_DIR}/Tree.h
        ${PROJECT_SOURCE_DIR}/Thread.h
        ${PROJECT_SOURCE_DIR}/utf-8.h
        ${PROJECT_SOURCE_DIR}/WebSocket.h
        ${PROJECT_SOURCE_DIR}/MQTTPacket.h
        ${PROJECT_SOURCE_DIR}/MQTTPacketOut.h
        ${PROJECT_SOURCE_DIR}/MQTTProperties.h
        ${PROJECT_SOURCE_DIR}/MQTTClient.h
        )

configure_file( 
	"${PROJECT_SOURCE_DIR}/VersionInfo.h.in"
	"${PROJECT_BINARY_DIR}/VersionInfo.h"
	 )

include_directories( "${PROJECT_BINARY_DIR}" )

add_library(${PROJECT_NAME} ${SOURCES_PAHO} )

## Build Driver Dynamic Library ------------------------------------

project(LbwELI.KMGmbH C)

IF(WIN32)
  ADD_DEFINITIONS(-D_CRT_NONSTDC_NO_DEPRECATE)
ENDIF()

include_directories( "${PROJECT_SOURCE_DIR}/paho.mqtt.c/src" )
add_library(${PROJECT_NAME} SHARED 
  library.c library.h 
  session_list.c session_list.h 
  driver.c driver.h 
  utils.c utils.h)
target_link_libraries(${PROJECT_NAME} PahoMqtt Jsmn ${LIBS_SYSTEM} )

## Build Sample console Application --------------------------------

project(LbELI_cli C)

add_executable(${PROJECT_NAME}  main.c)

target_link_libraries(${PROJECT_NAME} LbwELI.KMGmbH ${LIBS_SYSTEM} )


IF(WIN32)
  message(STATUS "BinDir => ${CMAKE_CURRENT_BINARY_DIR}")
  file( GENERATE OUTPUT "./Debug/config.json" CONTENT "{ \"host\" : \"10.0.2.2\", \"port\": 1883 }"  )
ENDIF()
