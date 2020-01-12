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

### MacOS

`export ASPNETCORE_ENVIRONMENT=Development`
`export ASPNETCORE_CONTENTROOT=../../../../../frontend`
`dotnet run`

### Windows
`$Env:ASPNETCORE_ENVIRONMENT = "Development"`
`$Env:ASPNETCORE_CONTENTROOT = "../../../../../frontend"`


## Build Deployment

This Builds a single executable that don't needs any netcore installed.
`dotnet publish -o deploy -c Debug -r win-x64 /p:PublishSingleFile=true`

## Build the driver 

Change path to `/driver/build` path. 

## Windows Defender Firewall Regeln scripten 

Get-Command -module Netsecurity 
Get-NetFirewallRule
Show-NetFirewallRule



