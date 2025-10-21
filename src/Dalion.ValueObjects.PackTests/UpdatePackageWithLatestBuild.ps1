$InformationPreference = 'Continue'

function Test-FilesWithWildcard
{
    param(
        [string]$FolderPath,
        [string]$FilterPattern
    )

    $files = Get-ChildItem -Path $FolderPath -Filter $FilterPattern

    return [bool]($null -ne $files)
}

$NupkgPath = $PSScriptRoot + "/../Dalion.ValueObjects/bin/Debug/*.nupkg"
$IsBuilt = Test-FilesWithWildcard -FolderPath $NupkgPath -FilterPattern "*.nupkg"

if ($IsBuilt -eq $false)
{
    Write-Error "No build found. Please build the Dalion.ValueObjects project first."
    return
}

# Delete cached version of the nuget package
$CachePath = [System.Environment]::ExpandEnvironmentVariables("%USERPROFILE%/.nuget/packages/dalion.valueobjects")
Remove-Item -Path $CachePath -Recurse -Force -ErrorAction Ignore

# Copy the latest build to the local nuget feed
$LocalFeedPath = [System.Environment]::ExpandEnvironmentVariables("C:/LocalNuGetFeed")
Copy-Item -Path $NupkgPath -Destination $LocalFeedPath -Force

# Install the latest build
dotnet restore "$PSScriptRoot/Dalion.ValueObjects.PackTests.csproj" --force --no-cache

Write-Information "Package refreshed"

