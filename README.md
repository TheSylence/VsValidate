# VsValidate
![GitHub release (latest by date)](https://img.shields.io/github/v/release/TheSylence/VsValidate)
[![CI](https://github.com/TheSylence/VsValidate/actions/workflows/Ci.yml/badge.svg?branch=main)](https://github.com/TheSylence/VsValidate/actions/workflows/Ci.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/bbb3b3809fee449f8a0326a1de0e49ff)](https://www.codacy.com/gh/TheSylence/VsValidate/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=TheSylence/VsValidate&amp;utm_campaign=Badge_Grade)
![Codacy coverage](https://img.shields.io/codacy/coverage/bbb3b3809fee449f8a0326a1de0e49ff)
![GitHub](https://img.shields.io/github/license/TheSylence/VsValidate)

Validator for Visual Studio projects to make sure all projects in a solution are configured properly.

## Requirements
.NET Core 5.0 Runtime must be installed on the machine.
If you build your app using `dotnet` you probably have installed the .NET Core SDK which includes the runtime.
The runtime doesn't have to be installed separately in this case.

The app should work on Windows x64, Linux x64 and Linux Arm64.

## Usage
Call the program with your configuration file and specify which projects (or solutions) to validate.

`vsvalidate.exe --config your-configuration-file.yml --project your-project.sln`

Validation errors will be written to stderr and the exit code of the program is either 0 (validation successful) or 1 (validation failed).

You can specify the `--verbose` switch to enable verbose logging to stdout or `--silent` to suppress all output.

## Configuration
Configurations are stored in simply YAML files and are easily editable.

Sample configuration:
```yml
properties:
    - name: TreatWarningsAsErrors
      value: True
      required: true

    - name: TargetFramework
      value: net5.0
      required: true
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
Checks that a property is correctly configured in all projects. Listed under `properties` in your configuration file.

| Field | Description | Required? | Default value |
|---|---|---|---|
| `name` | Name of the property. Case sensitive. | Yes | |
| `forbidden` | Fail validation if property exists. | No | `false` |
| `required` | Fail validation when property is not found. | No | `false` |
| `maximumOccurrences` | Fail validation if property is found more than `n` times in a project. | No | Don't check number of occurrences |
| `minimumOccurrences` | Fail validation if property is found less than `n` times in a project. | No | Don't check number of occurrences |
| `value` | Fail validation if property does not have this value. Case sensitive | No | Don't check value |

### Package
Checks that a package is correctly referenced in all projects. Listed under `packages` in your configuration file.

| Field | Description | Required? | Default value |
|---|---|---|---|
| `name` | Name of the referenced package. Case sensitive | Yes | |
| `required` | Fails validation if package is not referenced | No | `false` |
| `forbidden` | Fails validation if package is referenced | No | `false` |
| `version` | Fails validation if installed version of the package does not fall in this range. (See [https://github.com/npm/node-semver#versions](https://github.com/npm/node-semver#versions) for a reference for version ranges) | No | Don't check version | 

### Project Reference
Checks that a project is correctly referenced in all projects. Listed under `projects` in your configuration file.

| Field | Description | Required? | Default value |
|---|---|---|---|
| `name` | Name of the referenced project (paths are ignored). Case sensitive | Yes |
| `required` | Fails validation if project is not referenced. | No | `false` |
| `forbidden` | Fails validation if project is referenced. | No | `false ` |