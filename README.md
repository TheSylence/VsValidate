# VsValidate
Validator for Visual Studio projects to make sure all projects in a solution are configured properly.

## Usage
Call the program with your configuration file and specify which projects (or solutions) to validate.

`vsvalidate.exe --config your-configuration-file.yml --project your-project.sln`

## Configuration
Configurations are stored in simply YAML files and are easily editable.

Sample configuration:
```yml
properties:
    - name: TreatWarningsAsErrors
      value: True
      optional: false

    - name: TargetFramework
      value: net5.0
      optional: false
```

This configuration checks that all projects are built for .NET 5.0 and have *Treat warnings as errors* enabled.

Or if you want to make sure that a certain nuget package is installed in your projects:
```yml
packages:
    - name: Newtonsoft.Json
      required: true
      version: "^12.0.0"
```
This checks that all projects reference *Newtonsoft.Json* and checks that some version with a major version of 12 is used.
(See [https://github.com/npm/node-semver#versions](https://github.com/npm/node-semver#versions) for a reference for version ranges)

You can of course combine both package and property checks in a single configuration file.

## Rules

### Property
Enables checks for properties in all projects. Listed under `properties` in your configuration file.

