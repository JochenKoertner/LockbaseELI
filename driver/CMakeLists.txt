cmake_minimum_required(VERSION 3.12)

## build settings
SET(PAHO_VERSION_MAJOR 1)
SET(PAHO_VERSION_MINOR 2)
SET(PAHO_VERSION_PATCH 1)
SET(CLIENT_VERSION ${PAHO_VERSION_MAJOR}.${PAHO_VERSION_MINOR}.${PAHO_VERSION_PATCH})

STRING(TIMESTAMP BUILD_TIMESTAMP UTC)
MESSAGE(STATUS "Timestamp is ${BUILD_TIMESTAMP}")


file(GLOB SOURCES_PAHO paho.mqtt.c/src/*.c)
file(GLOB INCLUDE_PAHO paho.mqtt.c/src/*.h)


# Lockbase Electronic Lock Interface - Library
project(LbELI C)
set(CMAKE_C_STANDARD 11)
# configure a header file to pass some of the CMake settings
# to the source code
configure_file (
        "paho.mqtt.c/src/VersionInfo.h.in"
        "../paho.mqtt.c/src/VersionInfo.h"
)
add_library(lbweli_driver ${SOURCES_PAHO} library.c library.h VersionInfo.h)

# Lockbase Electronic Lock Interface - CommandLineInterface
project(LbELI_cli)
set(CMAKE_C_STANDARD 11)
add_executable(mqtt_cli  ${INCLUDE_PAHO} main.c)
target_link_libraries(mqtt_cli lbweli_driver)

