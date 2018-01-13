# MongoDB ASP.NET Core Docker Starter  

This is an opinionated starter-kit for any ASP.NET Core API Server that uses MongoDB in the backend.  
Not only as the backend datastore, but we are using MongoDB for user Authentication/Authorization via Identity Service as well, in the application.  


## Getting Started  

The following steps assumes that you have MongoDB installed locaclly in your machine.  
I prefer running mongodb with a config file like this: `mongod --config ./mongod.cfg`, a sample config file is available in `tools` directory.  
For more information on installing and running MongoDB locally, [refer this link.](https://docs.mongodb.com/manual/installation/#mongodb-community-edition)  
I am currently using MongoDB Community Edition, 'version 3.6'.  

Once MongoDB is installed and running, 

1. Checkout the repository  
2. `cd src/dbdriver` and `dotnet restore`  
3. `cd src/apiserver` and `dotnet restore`  
4. Create settings file for NLogger  
   + `cd src/apiserver/settings/logger-settings`  
   + Rename `nlog.sample.config` to `nlog.development.config`  
4. Create settings file for API application  
   + `cd src/apiserver/settings/app-settings`  
   + Rename `settings.sample.json` to `settings.development.json`  
5. `export ASPNETCORE_ENVIRONMENT=Development` this will tell ASP.NET Core API Server to run in 'development' mode  
6. cd `src/apiserver` and start the server: `dotnet run localhost <your port# of choice>`  


### Deploy the app as a container  

+ Perform step #1 to #4 above  
+ Make sure you have [installed Docker](https://docs.docker.com/engine/installation/) and installed [Docker-Compose](https://docs.docker.com/engine/installation/)  
+ Make sure you are in `src` directory  
+ Run `docker-compose up`  


## Pending Items  

+ Make the app deployable as a docker container `[✓]`  
+ Add some more realistic features to the application `[✓]`  
   - Admin Policies for more granular authorization `[✓]`  
   - GET/POST/DEL/PUT/PATCH on some resources `[✓]`  
+ Add Unit Tests 😃   

