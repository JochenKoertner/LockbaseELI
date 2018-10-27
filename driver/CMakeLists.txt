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

file(GLOB SOURCES_PAHO ${PROJECT_SOURCE_DIR}/*.c)
file(GLOB INCLUDE_PAHO ${PROJECT_SOURCE_DIR}/MQTT*.h)

configure_file( 
	"${PROJECT_SOURCE_DIR}/VersionInfo.h.in"
	"${PROJECT_BINARY_DIR}/VersionInfo.h"
	 )

include_directories( "${PROJECT_BINARY_DIR}" )

add_library(${PROJECT_NAME} ${SOURCES_PAHO} )

## Build Driver Dynamic Library ------------------------------------

project(LbwELI.KMGmbH C)

include_directories( "${PROJECT_SOURCE_DIR}/paho.mqtt.c/src" )
add_library(${PROJECT_NAME} SHARED library.c session_list.c driver.c driver.h utils.c utils.h)
target_link_libraries(${PROJECT_NAME} PahoMqtt Jsmn ${LIBS_SYSTEM} )

## Build Sample console Application --------------------------------

project(LbELI_cli C)

add_executable(${PROJECT_NAME}  main.c)

target_link_libraries(${PROJECT_NAME} LbwELI.KMGmbH ${LIBS_SYSTEM} )