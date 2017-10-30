cd ..
RMDIR /S /Q deploy
mkdir deploy
copy Dockerfile deploy\
copy scripts\runtests.sh deploy\
copy scripts\wait-for-it.sh deploy\
copy scripts\rm_cr.sh deploy\
mkdir deploy\testresults

dotnet restore VSS.Productivity3D.Scheduler.AcceptanceTests.sln --no-cache

cd tests
dotnet publish RepositoryTests/RepositoryTests.csproj -o ..\..\deploy\RepositoryTests -f netcoreapp1.1
dotnet publish SchedulerTests/SchedulerTests.csproj -o ..\..\deploy\SchedulerTests -f netcoreapp1.1

cd ..
cd utilities
dotnet publish TestRun/TestRun.csproj -o ..\..\deploy\TestRun -f netcoreapp1.1
cd ..