@echo off
cls
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

if "%1" == "nopause" goto end
pause
:end