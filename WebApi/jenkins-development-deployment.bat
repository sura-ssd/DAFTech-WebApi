@echo off

echo "Start deploying app to production server"

set source=%cd%\src
set destination=C:\inetpub\wwwroot\daftech

IF not exist %destination% (
mkdir %destination%
echo %destination% directory created
)

cd %source%

echo "build project"
dotnet build

xcopy appsettings.json appsettings.json /Y

echo "publish api to " %destination%
dotnet publish -o %destination%


  