<#
	Sets the environment variables required for running the tests locally against Docker

	TEST_DATA_PATH -> Relative path to the test data directory
	COMPACTION_SVC_BASE_URI -> compaction service end point, ususally a port or /compaction
	NOTIFICATION_SVC_BASE_URI -> notification service end point, usually a port or /notification
	REPORT_SVC_BASE_URI  -> report service end point, ususally a port or /Report
	TAG_SVC_BASE_URI -> Tag file service, usually a port or /TagProc
	COORD_SVC_BASE_URI Coordinates service, usually a port or /Coord
	PROD_SVC_BASE_URI Prod service, usually a port or /ProdData
	FILE_ACCESS_SVC_BASE_URI Coordinate service, usually a port or /FileAccess
	RAPTOR_WEBSERVICES_HOST -> server used for base
#>	

[Environment]::SetEnvironmentVariable("TEST_DATA_PATH", "../../TestData/", "Machine")
[Environment]::SetEnvironmentVariable("COMPACTION_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("NOTIFICATION_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("REPORT_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("TAG_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("COORD_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("PROD_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("FILE_ACCESS_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("RAPTOR_WEBSERVICES_HOST", "172.17.180.238", "Machine")

<#
To run tests from within Visual Studio against Raptor Services running in local containers)

[Environment]::SetEnvironmentVariable("TEST_DATA_PATH", "../../../tests/ProductionDataSvc.AcceptanceTests/TestData/", "Machine")
[Environment]::SetEnvironmentVariable("COMPACTION_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("NOTIFICATION_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("REPORT_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("TAG_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("COORD_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("PROD_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("FILE_ACCESS_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("RAPTOR_WEBSERVICES_HOST", "172.17.180.238", "Machine")
#>


<#
Here's the values for running tests from within Visual Studio against Raptor Services running locally (no containers)

[Environment]::SetEnvironmentVariable("TEST_DATA_PATH", "../../../ProductionDataSvc.AcceptanceTests/TestData/", "Machine")
[Environment]::SetEnvironmentVariable("COMPACTION_SVC_BASE_URI", ":80", "Machine")
[Environment]::SetEnvironmentVariable("NOTIFICATION_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("REPORT_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("TAG_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("COORD_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("PROD_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("FILE_ACCESS_SVC_BASE_URI", ":5000", "Machine")
[Environment]::SetEnvironmentVariable("RAPTOR_WEBSERVICES_HOST", "localhost", "Machine")
#>

