cmake_minimum_required(VERSION 3.12)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)


## Build PahoMqtt Library 


project(PahoMqtt C)
set(CMAKE_C_STANDARD 11)

message(STATUS "Current Binary Dir is ${CMAKE_CURRENT_BINARY_DIR}")


## build settings
set(PAHO_VERSION_MAJOR 1)
set(PAHO_VERSION_MINOR 2)
set(PAHO_VERSION_PATCH 1)
set(CLIENT_VERSION ${PAHO_VERSION_MAJOR}.${PAHO_VERSION_MINOR}.${PAHO_VERSION_PATCH})

string(TIMESTAMP BUILD_TIMESTAMP UTC)
message(STATUS "Timestamp is ${BUILD_TIMESTAMP}")

set(PROJECT_SOURCE_DIR ${PROJECT_SOURCE_DIR}/paho.mqtt.c/src)

file(GLOB SOURCES_PAHO ${PROJECT_SOURCE_DIR}/*.c)
file(GLOB INCLUDE_PAHO ${PROJECT_SOURCE_DIR}/MQTT*.h)

configure_file( 
	"${PROJECT_SOURCE_DIR}/VersionInfo.h.in"
	"${PROJECT_BINARY_DIR}/build/VersionInfo.h"
	 )

include_directories( "${PROJECT_BINARY_DIR}/build" )

add_library(${PROJECT_NAME} ${SOURCES_PAHO} )

## Build Driver Dynamic Library 

project(LbELI C)

## set( CMAKE_RUNTIME_OUTPUT_DIRECTORY ./build )

include_directories( "${PROJECT_SOURCE_DIR}/paho.mqtt.c/src" ) 
add_library(${PROJECT_NAME} library.c session_list.c)
target_link_libraries(${PROJECT_NAME} PahoMqtt)

## Build Sample console Application 

project(LbELI_cli)

## set( CMAKE_RUNTIME_OUTPUT_DIRECTORY ./build )

add_executable(${PROJECT_NAME}  main.c)
target_link_libraries(${PROJECT_NAME} LbELI)

