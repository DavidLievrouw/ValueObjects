@echo off
cls
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set DOTNET_NOLOGO=true
SET DIR=%~dp0%
SET DISTDIR=%~dp0%dist\
SET PACKAGESDIR=%DISTDIR%Release\
SET KEY=s3cr3t
SET SOURCE=https://api.nuget.org/v3/index.json

for /f "delims=" %%x in (version.txt) do set VERSION=%%x

ECHO Press [ENTER] to push the package Dalion.ValueObjects v%VERSION% to nuget.org
PAUSE
dotnet nuget push %PACKAGESDIR%Dalion.ValueObjects.%VERSION%.nupkg -k %KEY% -s %SOURCE%

PAUSE
