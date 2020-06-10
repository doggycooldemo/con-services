PARAM (
    [Parameter(Mandatory = $false)][string]$service,
    [Parameter(Mandatory = $false)][string]$action,
    [Parameter(Mandatory = $false)][string]$awsRepositoryName = '940327799086.dkr.ecr.us-west-2.amazonaws.com',
    [Parameter(Mandatory = $false)][string]$branch,
    [Parameter(Mandatory = $false)][string]$buildId
)

enum ReturnCode {
    SUCCESS
    INVALID_ACTION
    CANNOT_FIND_PATH
    CONTAINER_BUILD_FAILED
    CONTAINER_CREATE_FAILED
    UNABLE_TO_FIND_IMAGE
    UNABLE_TO_FIND_TEST_RESULTS
    UNABLE_TO_FIND_TEST_COVERAGE
    IMAGE_TAG_FAILED
    IMAGE_PUSH_FAILED
    AWS_ECR_LOGIN_FAILED
}

$services = @{
    Common     = 'common'
    FileAccess = 'service/FileAccess'
    Mock       = 'service/MockProjectWebApi'
    Push       = 'service/Push'
}

$servicePath = ''
$serviceName = ''

function Build-Solution {
    Login-Aws

    $imageTag = "$serviceName-build"

    Write-Host "`nBuilding container image '$imageTag'..." -ForegroundColor Green
    docker build -f $servicePath/build/Dockerfile.build --tag $imageTag --no-cache --build-arg SERVICE_PATH=$servicePath .

    if (!$?) { Exit-With-Code ([ReturnCode]::CONTAINER_BUILD_FAILED) }

    Write-Host "`nImage details:" -ForegroundColor Green
    $image = docker images $imageTag

    if ($image.Count -eq 0) {
        Write-Host "`nUnable to validate container image" -ForegroundColor Red
        Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_IMAGE)
    }
    else {
        Write-Host $image[0]
        Write-Host $image[1]
        Write-Host "`nBuild of '$imageTag' image complete" -ForegroundColor Green
    }

    Exit-With-Code ([ReturnCode]::SUCCESS)
}

function Unit-Test-Solution {
    $container_name = "$serviceName-unittest"

    # Ensure required image exists
    $buildImage = "$serviceName-build:latest"

    if ($(docker images $buildImage -q).Count -eq 0) {
        Write-Host "Unable to find required build image '$buildImage'." -ForegroundColor Red
        Write-Host "Found the following '$serviceName*' images:`n" -ForegroundColor Red
        docker images $serviceName*

        Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_IMAGE)
    }

    # Clean up from earlier runs
    Remove-Item -Path UnitTestResults -Recurse -ErrorAction SilentlyContinue

    # Build and run containerized unit tests
    Write-Host "`nBuilding unit test container..." -ForegroundColor Green

    # We don't require a build context here because everything needed is present in the already present [service]-build image.
    # Docker build allows the - < token indicating the dockerfile is passed via STDIN; this means the build context only consists of the Dockerfile.
    # Powershell doesn't have an input redirection feature so it's done using the Get-Content cmdlet.
    Get-Content $servicePath/build/Dockerfile.unittest | docker build --tag $container_name --no-cache --build-arg SERVICE_PATH=$servicePath - 
    if (-not $?) { Exit-With-Code ([ReturnCode]::CONTAINER_BUILD_FAILED) }

    Write-Host "`nCreating unit test container..." -ForegroundColor Green
    $unique_container_name = "$container_name`_$(Get-Random -Minimum 1000 -Maximum 9999)"

    # Start the container image and terminate and detach immediately
    docker create --name $unique_container_name $container_name
    if (-not $?) { Exit-With-Code ([ReturnCode]::CONTAINER_CREATE_FAILED) }

    Write-Host "Copying test results and coverage files..." -ForegroundColor Green
    docker cp $unique_container_name`:/build/$servicePath/test/UnitTests/UnitTestResults/. $servicePath/test/UnitTestResults

    if (-not (Test-Path "$servicePath/test/UnitTestResults/TestResults.xml" -PathType Leaf)) {
        Write-Host 'Unable to find TestResults file on local host.' -ForegroundColor Red
        Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_TEST_RESULTS)
    }
    if (-not (Test-Path "$servicePath/test/UnitTestResults/Coverage.xml" -PathType Leaf)) {
        Write-Host 'Unable to find test coverage file on local host.' -ForegroundColor Red
        Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_TEST_COVERAGE)
    }

    Write-Host "`nRemoving test container..." -ForegroundColor Green
    docker rm $unique_container_name
    Write-Host "`nUnit test run complete" -ForegroundColor Green

    Exit-With-Code ([ReturnCode]::SUCCESS)
}

function Publish-Service {
    $publishImage = "$serviceName-webapi"

    # Ensure required image exists
    $buildImage = "$serviceName-build:latest"

    if ($(docker images $buildImage -q).Count -eq 0) {
        Write-Host "Unable to find required build image '$buildImage'." -ForegroundColor Red
        Write-Host "Found the following '$serviceName' images:`n" -ForegroundColor Red
        docker images $serviceName*

        Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_IMAGE)
    }

    # Build and run containerized unit tests
    Write-Host "`nBuilding published service container..." -ForegroundColor Green

    # We don't require a build context here because everything needed is present in the already present [service]-build image.
    # Docker build allows the - < token indicating the dockerfile is passed via STDIN; this means the build context only consists of the Dockerfile.
    # Powershell doesn't have an input redirection feature so it's done using the Get-Content cmdlet.
    Get-Content $servicePath/build/Dockerfile.runtime | docker build --tag $publishImage --no-cache --build-arg SERVICE_PATH=$servicePath - 

    if (-not $?) { Exit-With-Code ([ReturnCode]::CONTAINER_BUILD_FAILED) }

    Write-Host "`nPublish application complete" -ForegroundColor Green
    Exit-With-Code ([ReturnCode]::SUCCESS)
}

function Push-Container-Image {
    Login-Aws

    $publishImage = "$serviceName-webapi"

    if ($(docker images $publishImage -q).Count -eq 0) {
        Write-Host "Unable to find required publish image '$publishImage'. Looking for build image..." -ForegroundColor Green
        $publishImage = "$serviceName-build"

        if ($(docker images $publishImage -q).Count -eq 0) {
            Write-Host "Unable to find required build image '$publishImage'." -ForegroundColor Red
            Write-Host "Found the following '$serviceName' images:`n" -ForegroundColor Red
            docker images $serviceName*
    
            Exit-With-Code ([ReturnCode]::UNABLE_TO_FIND_IMAGE)
        }
        else {
            Write-Host "Found fallback image '$publishImage'" -ForegroundColor Green
        }
    }

    $ecr_prefix = 'rpd-ccss-'
    $branch = $branch -replace '.*/' # Remove everything up to and including the last forward slash.

    $versionNumber = $branch + "-" + $buildId
    $ecrRepository = "${awsRepositoryName}/${ecr_prefix}${serviceName}-webapi:${versionNumber}"

    Write-Host "`nPushing image '$ecrRepository'..." -ForegroundColor Green
    docker tag $publishImage $ecrRepository
    if (-not $?) { Exit-With-Code ([ReturnCode]::IMAGE_TAG_FAILED) }

    docker push $ecrRepository
    if (-not $?) { Exit-With-Code ([ReturnCode]::IMAGE_PUSH_FAILED) }

    Write-Host "`nImage push complete" -ForegroundColor Green

    Exit-With-Code ([ReturnCode]::SUCCESS)
}

function Login-Aws {
    Write-Host "`nAuthenticating with AWS ECR..." -ForegroundColor Green

    #aws ecr get-login-password --region us-west-2 | docker login --username AWS --password-stdin 940327799086.dkr.ecr.us-west-2.amazonaws.com
    #if (-not $?) { Exit-With-Code ([ReturnCode]::AWS_ECR_LOGIN_FAILED) }
}

function TrackTime($Time) {
    if (!($Time)) { Return Get-Date } Else {
        return ((Get-Date) - $Time)
    }
}

function Exit-With-Code {
    param(
        [ReturnCode][Parameter(Mandatory = $true)]$code
    )

    $executionTime = TrackTime $timeStart
    $executionMinutes = "{0:N2}" -f $executionTime.TotalMinutes
    Write-Host "Script completed in ${executionMinutes} minutes."

    if ($code -eq [ReturnCode]::SUCCESS) {
        Write-Host "`nExiting: $code" -ForegroundColor Green
    }
    else {
        Write-Host "`nExiting with error: $code" -ForegroundColor Red
    }

    Pop-Location
    Exit $code
}

# Get on with the real work...

# Set the environment working directory.
Push-Location $PSScriptRoot
[Environment]::CurrentDirectory = $PWD

Set-Location -Path '../src'
if (!$?) { Exit-With-Code ([ReturnCode]::CANNOT_FIND_PATH) }

# Run the script action.
$servicePath = $services[$service -replace '-']
$serviceName = $service.ToLower()

Write-Host 'Script Variables:' -ForegroundColor Green
Write-Host "  action = $action"
Write-Host "  branch = $branch"
Write-Host "  buildId = $buildId"
Write-Host "  service = $service"
Write-Host "  servicePath = $servicePath"
Write-Host "  serviceName = $serviceName"
Write-Host "  awsRepositoryName = $awsRepositoryName"
Write-Host "  Working Directory ="($pwd).path

$timeStart = Get-Date

# Run the appropriate action.
switch ($action) {
    'build' {
        Build-Solution
        continue
    }
    'unittest' {
        Unit-Test-Solution
        continue
    }
    'publish' {
        Publish-Service
        continue
    }
    'pushImage' {
        Push-Container-Image
        continue
    }
    default {
        Write-Host "Invalid action ($action)"
        Exit-With-Code ([ReturnCode]::INVALID_ACTION)
    }
}