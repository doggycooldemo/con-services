cd ..
rm -rf deploy
mkdir deploy
cp Dockerfile deploy/
cp scripts/runtests.sh deploy/
cp scripts/wait-for-it.sh deploy/
cp scripts/rm_cr.sh deploy/
mkdir deploy/testresults

dotnet restore --no-cache VSS.Productivity3D.Scheduler.AcceptanceTests.sln

cd tests
dotnet publish RepositoryTests/RepositoryTests.csproj -o ../../deploy/RepositoryTests -f netcoreapp1.1
dotnet publish SchedulerTests/SchedulerTests.csproj -o ../../deploy/SchedulerTests -f netcoreapp1.1

cd ..
cd utilities
dotnet publish TestRun/TestRun.csproj -o ../../deploy/TestRun -f netcoreapp1.1