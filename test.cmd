@echo off
cls
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set DOTNET_NOLOGO=true
SET DIR=%~dp0%
SET SRCDIR=%DIR%\src
SET PRODUCT=Dalion.ValueObjects

dotnet test %SRCDIR%\%PRODUCT%.Tests\%PRODUCT%.Tests.csproj
if errorlevel 1 (
   echo One or more tests failed.
   exit /b %errorlevel%
)

dotnet test %SRCDIR%\%PRODUCT%.Rules.Tests\%PRODUCT%.Rules.Tests.csproj
if errorlevel 1 (
   echo One or more tests failed.
   exit /b %errorlevel%
)

dotnet test %SRCDIR%\%PRODUCT%.SnapshotTests\%PRODUCT%.SnapshotTests.csproj -f net8.0
if errorlevel 1 (
   echo One or more tests failed.
   exit /b %errorlevel%
)

dotnet test %SRCDIR%\%PRODUCT%.SnapshotTests\%PRODUCT%.SnapshotTests.csproj -f net9.0
if errorlevel 1 (
   echo One or more tests failed.
   exit /b %errorlevel%
)

if "%1" == "nopause" goto end
pause
:end