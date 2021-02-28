param( 
    [String]$token="1234567"
)

Get-ChildItem ./VsValidate.Tests/TestResults -Filter *.xml -Recurse | ForEach-Object {
    java -jar ./codacy-test-reporter.jar report -l CSharp -r $_ -t $token --partial
}

java -jar ./codacy-test-reporter.jar final -t $token