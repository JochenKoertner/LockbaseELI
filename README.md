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

* /driver - LowLevel driver connected to the Web-App (written in C)
* /doc - Lockbase ELI specification and other documents
* /sample - Empty DLL body and other samples

## Build the frontend

Change path to /frontend

`yarn build`


### Upgrade single package 

`yarn add package eslint-utils`
`yarn add package fstream`
`yarn add package lodash.template`

### Update yarn complete 

`brew upgrade yarn`

### yarn audit 

`export FORCE_COLOR=0`
`yarn audit --level high > audit.log`
