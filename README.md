# LockbaseELI
LOCKBASE Electronic Lock Interface Demo

The SubDirectorys are 

* /backend - C# Backend hosting Mqtt Server
    * /coredomain - Main Business Logic of the application
    * /tests - Unit Test's for CoreDomain
    * /ui  - WebApp
* /frontend - React SPA 
    * /public   - Static content
    * /src  - Application

* /driver - C sample driver 
    * / jsmn   - Json parsing submodule
    * / paho.mqtt.c - MQTT Client lib

  

* /driver - LowLevel driver connected to the Web-App (written in C)
* /doc - Lockbase ELI specification and other documents
* /sample - Empty DLL body and other samples

## Build the frontend

Change path to /frontend

`yarn build`

Start React App for proxy usage on port 3000.  
`yarn start`

### Upgrade single package 

`yarn add package eslint-utils`
`yarn add package fstream`
`yarn add package lodash.template`

### Update yarn complete 

`brew upgrade yarn`

### yarn audit 

`export FORCE_COLOR=0`
`yarn audit --level high > audit.log`

## Build and Start backend

Change path to `/backend/ui` path.

`dotnet restore`
`dotnet build`


`export ASPNETCORE_ENVIRONMENT=Development`
`export ASPNETCORE_CONTENTROOT=../../../../../frontend`
`dotnet run`

## Build the driver 

Change path to `/driver/build` path. 
