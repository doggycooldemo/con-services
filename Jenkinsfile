node ('jenkinsslave-pod') {
    def branch = env.BRANCH_NAME
    def buildNumber = env.BUILD_NUMBER
    def versionPrefix = ""
    def suffix = ""
    def branchName = ""

    if (branch.contains("release")) {
        versionPrefix = "1.0."
        branchName = "Release"
    } else if (branch.contains("Dev")) {
        versionPrefix = "0.99."
        branchName = "Dev"
    } else if (branch.contains("master")) {
        versionPrefix = "1.0."
        branchName = "master"
    } else {
        branchName = branch.substring(branch.lastIndexOf("/") + 1)
        suffix = "-" + branchName
        versionPrefix = "0.98."
    }

    def versionNumber = versionPrefix + buildNumber
    def fullVersion = versionNumber + suffix
    try {
        stage('Test Solution') {
                checkout scm
                dir("/TestResults") {}
                def building = docker.build("vss.trex:${fullVersion}", "-f DockerfileBuild .")

                // Currently we need to execute the tests like this, because the pipeline docker plugin being aware of DIND, and attempting to map
                // the volume to the bare metal host
                sh "docker run -v ${env.WORKSPACE}/TestResults:/TestResults ${building.id} dotnet test --test-adapter-path:. --logger:\"xunit;LogFilePath=/TestResults/RaptorClassLibraryTestResults.xml\" \
                    /build/tests/netstandard/RaptorClassLibrary.Tests.netcore/RaptorClassLibrary.Tests.netcore.csproj"
                // building.inside("-v ${env.WORKSPACE}/TestResults:/TestResults"){
                //     sh 'dotnet test --test-adapter-path:. --logger:"xunit;LogFilePath=/TestResults/RaptorClassLibraryTestResults.xml" \
                //         /build/tests/netstandard/RaptorClassLibrary.Tests.netcore/RaptorClassLibrary.Tests.netcore.csproj'
                // }
                sh "ls ${env.WORKSPACE}/TestResults"
        }
    }
    finally {
        stage('Publish Results'){
            step([$class: 'XUnitBuilder',
                thresholds: [[$class: 'FailedThreshold', unstableThreshold: '10']],
                tools: [[$class: 'XUnitDotNetTestType', pattern: '/TestResults/**']]])
        }
    }
}