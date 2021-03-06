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
`cd ./frontend`

`yarn build`

Start React App for proxy usage on port 3000.  
`yarn start`

### Upgrade single package 

`yarn add package eslint-utils`
`yarn add package fstream`
`yarn add package lodash.template`

### Install all packages again in node_modules/

`rm node_modules`
`yarn install`
`yarn upgrade`

For the purpose of reducing deduplication use package `yarn-deduplicate`
(https://medium.com/@bnaya/yarn-deduplicate-the-hero-we-need-f4497a362128)

`yarn yarn-deduplicate -s fewer yarn.lock`
`yarn install`


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
`dotnet run`



## Build Deployment

This Builds a single executable that don't needs any netcore installed.
`dotnet publish -o bin/deploy -c Release -r win-x64`

`dotnet publish -o bin/deploy -c Release -r osx-x64`

## Build the driver 

Change path to `/driver/build` path. 

For Mac Build call `../mac-build.sh` 

For Win Build call `../win-build.ps1` 

## Windows Defender Firewall Regeln scripten 

Get-Command -module Netsecurity 
Get-NetFirewallRule
Show-NetFirewallRule



