@echo off
cls
SET DIR=%~dp0%
SET SRCDIR=%DIR%\src
SET DISTDIR=%DIR%\dist
SET PRODUCT=Dalion.ValueObjects

IF EXIST %DISTDIR% RD /S /Q %DISTDIR%

dotnet restore %SRCDIR%\%PRODUCT%.sln
dotnet build %SRCDIR%\%PRODUCT%\%PRODUCT%.csproj --no-restore --configuration Release -p:BaseOutputPath="%DISTDIR%\\" -p:ContinuousIntegrationBuild="true"

if "%1" == "nopause" goto end
pause
:end