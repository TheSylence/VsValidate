param(
    [String] $exe,
    [String] $baseFolder
)

$ErrorActionPreference = 'continue'
$failedTests = $false

$failingFolder = "$($baseFolder)/Failing"
Get-ChildItem $failingFolder -Filter *.yml | ForEach-Object {
    $projectFile = [System.IO.Path]::ChangeExtension($_, ".proj");

    & dotnet $exe --config $($_) --project $($projectFile) > $null 2>&1
    if ( $LASTEXITCODE -eq 0 ) {
        Write-Error "Expected $($_) to fail but it did not"
        $failedTests = $true
    }
}

$succeedingFolder = "$($baseFolder)/Succeeding"
Get-ChildItem $succeedingFolder -Filter *.yml | ForEach-Object {
    $projectFile = [System.IO.Path]::ChangeExtension($_, ".proj");
    if(-not (Test-Path $projectFile -PathType leaf)) {
        $projectFile = [System.IO.Path]::ChangeExtension($_, ".sln");

        if(-not (Test-Path $projectFile -PathType leaf)) {
            $projectFile = [System.IO.Path]::ChangeExtension($_, ".csproj");
        }
    }

    & dotnet $exe --config $($_) --project $($projectFile) > $null 2>&1
    if ( $LASTEXITCODE -ne 0 ) {
        Write-Error "Expected $($_) to succeed but it did not"
        $failedTests = $true
    }
}

& dotnet $exe --config "$($baseFolder)/vsvalidate.yml" --project "$($baseFolder)/../VsValidate.sln"
if ( $LASTEXITCODE -ne 0 ) {
    Write-Error "Expected validation of VsValidate.sln to succeed but it did not"
    $failedTests = $true
}

if ( $failedTests ) {
    Write-Error "One or more tests have failed"
    Exit 1
}
Write-Output "All test cases successfully run"
Exit 0