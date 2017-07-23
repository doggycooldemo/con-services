#!/bin/bash

rm -rf artifacts

dotnet publish ./src/ProjectWebApi/VSS.Project.WebApi.csproj -o ../../artifacts/ProjectWebApi -f netcoreapp1.1 -c Docker

cp src/ProjectWebApi/appsettings.json artifacts/ProjectWebApi/

mkdir artifacts/logs