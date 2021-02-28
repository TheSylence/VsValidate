param(
    [String] $exe,
    [String] $baseFolder
)

$failingFolder = "$($baseFolder)/Failing"
Get-ChildItem $failingFolder -Filter *.yml | ForEach-Object {
    $projectFile = [System.IO.Path]::ChangeExtension($_, ".proj");

    & dotnet $exe --config $($_) --project $($projectFile) > $null 2>&1
    if ( $LASTEXITCODE -eq 0 ) {
        Write-Error "Expected $($_) to fail but it did not"
    }
}

$succeedingFolder = "$($baseFolder)/Succeeding"
Get-ChildItem $succeedingFolder -Filter *.yml | ForEach-Object {
    $projectFile = [System.IO.Path]::ChangeExtension($_, ".proj");

    & dotnet $exe --config $($_) --project $($projectFile) > $null 2>&1
    if ( $LASTEXITCODE -ne 0 ) {
        Write-Error "Expected $($_) to succeed but it did not"
    }
}
