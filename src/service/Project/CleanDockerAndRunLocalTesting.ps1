[Console]::ResetColor()

# Setting the COMPOSE_CONVERT_WINDOWS_PATHS environment variable before trying 
# to bring up the containers seems to fix the docker-compose bug reported here: https://github.com/docker/for-win/issues/1829
$Env:COMPOSE_CONVERT_WINDOWS_PATHS=1

# Running locally we cannot 'see' the Jenkins image repository so override it with a comparable image from visible repo.
$Env:APP_BUILD_IMAGE="microsoft/dotnet:2.1-sdk"

& $PSScriptRoot/DockerEnvironmentVariables.ps1

Write-Host "Stopping Docker containers" -ForegroundColor DarkGray
docker stop $(docker ps -aq)

# This is not ideal; but too often the Kafka container throws the following error, fails to start and breaks the dependency chain;
# java.lang.RuntimeException: A broker is already registered on the path /brokers/ids/1001
Write-Host "Removing Kafka, Filter WebAPI and Filter acceptance test containers" -ForegroundColor DarkGray
docker rm $(docker ps -aq --filter "name=project_kafka")
docker rm $(docker ps -aq --filter "name=project_webapi")
docker rm $(docker ps -aq --filter "name=project_accepttest")

Write-Host "Connecting to image host" -ForegroundColor DarkGray
Invoke-Expression -Command (aws ecr get-login --no-include-email --region us-west-2)

Write-Host "Building solution" -ForegroundColor DarkGray

$artifactsWorkingDir = "${PSScriptRoot}/artifacts/ProjectWebApi"

Remove-Item -Path ./artifacts -Recurse -Force -ErrorAction Ignore
Invoke-Expression "dotnet publish ./src/ProjectWebApi/VSS.Project.WebApi.csproj -o ../../artifacts/ProjectWebApi -f netcoreapp2.1 -c Docker"
Invoke-Expression "dotnet build ./test/UnitTests/MasterDataProjectTests/VSS.Project.WebApi.Tests.csproj"
Copy-Item ./src/ProjectWebApi/appsettings.json $artifactsWorkingDir
New-Item -ItemType directory ./artifacts/logs | out-null

Write-Host "Copying static deployment files" -ForegroundColor DarkGray
Set-Location ./src/ProjectWebApi
Copy-Item ./appsettings.json $artifactsWorkingDir
Copy-Item ./Dockerfile $artifactsWorkingDir
Copy-Item ./web.config $artifactsWorkingDir
Copy-Item ./log4net.xml $artifactsWorkingDir

& $PSScriptRoot/AcceptanceTests/Scripts/deploy_win.ps1

Write-Host "Building image dependencies" -ForegroundColor DarkGray
Set-Location $PSScriptRoot
Invoke-Expression "docker-compose --file docker-compose-local.yml pull"

Write-Host "Building Docker containers" -ForegroundColor DarkGray

$detach = ""

if ($args -contains "--detach")
{
    $detach = "--detach"
}

Invoke-Expression "docker-compose --file docker-compose-local.yml up --build $detach > c:\temp\output.log"

if (-not $?) {
    Write-Host "Error: Environment failed to start" -ForegroundColor Red
    Exit 1
}

Write-Host "Finished" -ForegroundColor Green
