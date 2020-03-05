param (
  [Parameter(Mandatory=$true)][string]$fullVersion
)
$command = & aws ecr get-login --region us-west-2 --no-include-email
$fixedCommand = $command.Replace("https://","")
Invoke-Expression $fixedCommand

& docker push 940327799086.dkr.ecr.us-west-2.amazonaws.com/rpd-ccss-mock-webapi:${fullVersion}
& docker push 940327799086.dkr.ecr.us-west-2.amazonaws.com/rpd-ccss-mock-webapi:latest
& docker rmi -f 940327799086.dkr.ecr.us-west-2.amazonaws.com/rpd-ccss-mock-webapi:${fullVersion}
& docker rmi -f 940327799086.dkr.ecr.us-west-2.amazonaws.com/rpd-ccss-mock-webapi:latest
