cmake_minimum_required(VERSION 3.12)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})


## Build PahoMqtt Library 


project(PahoMqtt C)
set(CMAKE_C_STANDARD 11)

IF(WIN32)
  ADD_DEFINITIONS(-D_CRT_SECURE_NO_DEPRECATE -DWIN32_LEAN_AND_MEAN -MD)
ELSEIF(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  ADD_DEFINITIONS(-DOSX)
ENDIF()

IF (WIN32)
    SET(LIBS_SYSTEM ws2_32)
ELSEIF (UNIX)
    IF(CMAKE_SYSTEM_NAME MATCHES "Linux")
        SET(LIBS_SYSTEM c dl pthread)
    ELSEIF (CMAKE_SYSTEM_NAME MATCHES "FreeBSD")
        SET(LIBS_SYSTEM compat pthread)
    ELSE()
        SET(LIBS_SYSTEM c pthread)
    ENDIF()
ENDIF()


message(STATUS "Current Binary Dir is ${CMAKE_CURRENT_BINARY_DIR}")
message(STATUS "C-Compiler is ${CMAKE_C_COMPILER}" )

## build settings
set(PAHO_VERSION_MAJOR 1)
set(PAHO_VERSION_MINOR 2)
set(PAHO_VERSION_PATCH 1)
set(CLIENT_VERSION ${PAHO_VERSION_MAJOR}.${PAHO_VERSION_MINOR}.${PAHO_VERSION_PATCH})

string(TIMESTAMP BUILD_TIMESTAMP UTC)
message(STATUS "Timestamp is ${BUILD_TIMESTAMP}")

# ADD_SUBDIRECTORY(paho.mqtt.c/src)

set(PROJECT_SOURCE_DIR ${PROJECT_SOURCE_DIR}/paho.mqtt.c/src)

file(GLOB SOURCES_PAHO ${PROJECT_SOURCE_DIR}/*.c)
file(GLOB INCLUDE_PAHO ${PROJECT_SOURCE_DIR}/MQTT*.h)

configure_file( 
	"${PROJECT_SOURCE_DIR}/VersionInfo.h.in"
	"${PROJECT_BINARY_DIR}/VersionInfo.h"
	 )

include_directories( "${PROJECT_BINARY_DIR}" )

add_library(${PROJECT_NAME} ${SOURCES_PAHO} )

## Build Driver Dynamic Library 

project(LbELI C)

set(CMAKE_C_STANDARD 11)

#IF(WIN32)
#  ADD_DEFINITIONS(-DWIN32_LEAN_AND_MEAN -MD)
#ELSEIF(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
#  ADD_DEFINITIONS(-DOSX)
#ENDIF()

#IF (WIN32)
#    SET(LIBS_SYSTEM ws2_32)
#ENDIF()


include_directories( "${PROJECT_SOURCE_DIR}/paho.mqtt.c/src" ) 
add_library(${PROJECT_NAME} library.c session_list.c)
target_link_libraries(${PROJECT_NAME} PahoMqtt)

## Build Sample console Application 

project(LbELI_cli C)

set(CMAKE_C_STANDARD 11)


IF (WIN32)
    SET(LIBS_SYSTEM ws2_32)
ENDIF()


add_executable(${PROJECT_NAME}  main.c)
target_link_libraries(${PROJECT_NAME} 
    LbELI 	
    ${LIBS_SYSTEM}
)