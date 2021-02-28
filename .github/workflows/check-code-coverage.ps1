param(
    [int] $threshold = 100,
    [String] $resultFolder
)

Get-ChildItem $resultFolder -Filter *.xml -Recurse | ForEach-Object {
    Write-Output "Checking code coverage results in '$($_)' with a threshold of $($threshold)"
    $xml = [xml](Get-Content -Path $_)
    
    $lines = [double]::Parse( (Select-Xml -Xml $xml -XPath "//coverage/@line-rate"), [cultureinfo] 'en-US' ) * 100
    $branches = [double]::Parse( (Select-Xml -Xml $xml -XPath "//coverage/@branch-rate"), [cultureinfo] 'en-US' ) * 100

    if ( $lines -lt $threshold || $branches -lt $threshold ) {
        Write-Error "Code coverage too low. Lines: $($lines)%, branches: $($branches)%, threshold: $($threshold)%"
    }
}