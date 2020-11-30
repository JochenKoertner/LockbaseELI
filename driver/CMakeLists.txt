cmake_minimum_required(VERSION 3.12)

IF (WIN32)
    SET(LIBS_SYSTEM ws2_32 crypt32 RpcRT4)

    set(VSVersion "14.15.26726")
    set(VSPath "C:/wintoolchain/VC/Tools/MSVC")
    set(VSBasePath "${VSPath}/${VSVersion}")

    set(WinSDKVersion "10.0.15063.0")
    set(WinSDKPath "C:/wintoolchain/Windows Kits/10")
    set(MACHINE "x64")

    
ENDIF()

set(CMAKE_C_STANDARD 11)

## Build PahoMqtt Library ------------------------------------------
project(PahoMqtt C)

IF(WIN32)
    ADD_DEFINITIONS(-D_WINSOCK_DEPRECATED_NO_WARNINGS -D_CRT_SECURE_NO_DEPRECATE -DWIN32_LEAN_AND_MEAN -D_WINSOCK_DEPRECATED_NO_WARNINGS -MD)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Wl,--no-warn-search-mismatch")
ELSEIF(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
    ADD_DEFINITIONS(-DOSX)
ENDIF()

## build settings
set(PAHO_VERSION_MAJOR 1)
set(PAHO_VERSION_MINOR 3)
set(PAHO_VERSION_PATCH 5)
set(CLIENT_VERSION ${PAHO_VERSION_MAJOR}.${PAHO_VERSION_MINOR}.${PAHO_VERSION_PATCH})

string(TIMESTAMP BUILD_TIMESTAMP UTC)
message(STATUS "Timestamp is ${BUILD_TIMESTAMP}")

set(PROJECT_SOURCE_DIR ${PROJECT_SOURCE_DIR}/paho.mqtt.c/src)


file(GLOB SOURCES_PAHO
        ${PROJECT_SOURCE_DIR}/Base64.c
        ${PROJECT_SOURCE_DIR}/Clients.c
        ${PROJECT_SOURCE_DIR}/Heap.c
        ${PROJECT_SOURCE_DIR}/LinkedList.c
        ${PROJECT_SOURCE_DIR}/Log.c
        ${PROJECT_SOURCE_DIR}/Messages.c
        ${PROJECT_SOURCE_DIR}/MQTTClient.c
        ${PROJECT_SOURCE_DIR}/MQTTPacket.c
        ${PROJECT_SOURCE_DIR}/MQTTPacketOut.c
        ${PROJECT_SOURCE_DIR}/MQTTPersistence.c
        ${PROJECT_SOURCE_DIR}/MQTTPersistenceDefault.c
        ${PROJECT_SOURCE_DIR}/MQTTProperties.c
        ${PROJECT_SOURCE_DIR}/MQTTProtocol.c
        ${PROJECT_SOURCE_DIR}/MQTTProtocolClient.c
        ${PROJECT_SOURCE_DIR}/MQTTProtocolOut.c
        ${PROJECT_SOURCE_DIR}/MQTTReasonCodes.c
        ${PROJECT_SOURCE_DIR}/MQTTTime.c
        ${PROJECT_SOURCE_DIR}/OsWrapper.c
        ${PROJECT_SOURCE_DIR}/SHA1.c
        ${PROJECT_SOURCE_DIR}/Socket.c
        ${PROJECT_SOURCE_DIR}/SocketBuffer.c
        ${PROJECT_SOURCE_DIR}/SSLSocket.c
        ${PROJECT_SOURCE_DIR}/StackTrace.c
        ${PROJECT_SOURCE_DIR}/Thread.c
        ${PROJECT_SOURCE_DIR}/Tree.c
        ${PROJECT_SOURCE_DIR}/utf-8.c
        ${PROJECT_SOURCE_DIR}/WebSocket.c
        )
file(GLOB INCLUDE_PAHO
        ${PROJECT_SOURCE_DIR}/Base64.h
        ${PROJECT_SOURCE_DIR}/Clients.h
        ${PROJECT_SOURCE_DIR}/Heap.h
        ${PROJECT_SOURCE_DIR}/Log.h
        ${PROJECT_SOURCE_DIR}/Messages.h
        ${PROJECT_SOURCE_DIR}/LinkedList.h
        ${PROJECT_SOURCE_DIR}/MQTTPacket.h
        ${PROJECT_SOURCE_DIR}/MQTTPacketOut.h
        ${PROJECT_SOURCE_DIR}/MQTTProperties.h
        ${PROJECT_SOURCE_DIR}/MQTTPersistence.h
        ${PROJECT_SOURCE_DIR}/MQTTPersistenceDefault.h
        ${PROJECT_SOURCE_DIR}/MQTTProtocol.h
        ${PROJECT_SOURCE_DIR}/MQTTProtocolClient.h
        ${PROJECT_SOURCE_DIR}/MQTTProtocolOut.h
        ${PROJECT_SOURCE_DIR}/MQTTClient.h
		${PROJECT_SOURCE_DIR}/MQTTExportDeclarations.h
		${PROJECT_SOURCE_DIR}/MQTTReasonCodes.h
		${PROJECT_SOURCE_DIR}/MQTTSubscripeOpts.h
        ${PROJECT_SOURCE_DIR}/MQTTTime.h
        ${PROJECT_SOURCE_DIR}/mutex_type.h
        ${PROJECT_SOURCE_DIR}/OsWrapper.h
        ${PROJECT_SOURCE_DIR}/SHA1.h
        ${PROJECT_SOURCE_DIR}/Socket.h
        ${PROJECT_SOURCE_DIR}/SocketBuffer.h
        ${PROJECT_SOURCE_DIR}/SSLSocket.h
        ${PROJECT_SOURCE_DIR}/StackTrace.h
        ${PROJECT_SOURCE_DIR}/Thread.h
        ${PROJECT_SOURCE_DIR}/Tree.h
        ${PROJECT_SOURCE_DIR}/utf-8.h
        ${PROJECT_SOURCE_DIR}/WebSocket.h
        )

configure_file( 
	"${PROJECT_SOURCE_DIR}/VersionInfo.h.in"
	"${PROJECT_BINARY_DIR}/VersionInfo.h"
	 )

include_directories( 
#   "${WinSDKPath}/Include/${WinSDKVersion}/shared" 
#   "${WinSDKPath}/Include/${WinSDKVersion}/ucrt"
#   "${WinSDKPath}/Include/${WinSDKVersion}/um"
#   "${VSBasePath}/include"
    "${PROJECT_BINARY_DIR}" 
)

add_library(${PROJECT_NAME} ${SOURCES_PAHO} )

## Build Driver Dynamic Library ------------------------------------

project(LbwELI.Demo C)

IF(WIN32)
    ADD_DEFINITIONS(-D_CRT_NONSTDC_NO_DEPRECATE)

    link_directories(
        "${WinSDKPath}/Lib/${WinSDKVersion}/um/${MACHINE}"
    )
ENDIF()

include_directories( "${PROJECT_SOURCE_DIR}/paho.mqtt.c/src" )
add_library(${PROJECT_NAME} SHARED 
  library.c library.h 
  session_list.c session_list.h 
  driver.c driver.h 
  utils.c utils.h jsmn.c)
target_link_libraries(${PROJECT_NAME} PahoMqtt ${LIBS_SYSTEM}  )

set_target_properties(${PROJECT_NAME} PROPERTIES PREFIX "")

## Build Sample console Application --------------------------------

project(LbELI_cli C)

add_executable(${PROJECT_NAME}  main.c)

target_link_libraries(${PROJECT_NAME} LbwELI.Demo ${LIBS_SYSTEM} )

IF(WIN32)
  file( GENERATE OUTPUT "./config.json" CONTENT "{ \"host\" : \"10.0.2.2\", \"port\": 1883 }"  )
ENDIF()
