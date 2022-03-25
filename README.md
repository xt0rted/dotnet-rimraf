# dotnet-rimraf

[![CI build status](https://github.com/xt0rted/dotnet-rimraf/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/xt0rted/dotnet-rimraf/actions/workflows/ci.yml)
[![NuGet Package](https://img.shields.io/nuget/v/rimraf?logo=nuget)](https://www.nuget.org/packages/rimraf)
[![GitHub Package Registry](https://img.shields.io/badge/github-package_registry-yellow?logo=nuget)](https://nuget.pkg.github.com/xt0rted/index.json)
[![Project license](https://img.shields.io/github/license/xt0rted/dotnet-rimraf)](LICENSE)

Deep deletion command for .NET (like rm -rf).

## Installation

This tool can be used as a dotnet global tool, or a dotnet local tool.
If using it as part of a build script it's recommended to install it as a local tool.

### Global

```console
dotnet tool install rimraf --global
```

### Local

```console
dotnet new tool-manifest
dotnet tool install rimraf
```

## Options

Name | Description
-- | --
`--dry-run` | See what would be deleted (enables `--verbose`)
`--no-preserve-root` | Do not treat `/` specially
`--verbose` | Enable verbose output
`--version` | Show version information
`--help` | Show help and usage information

## Usage

Use `dotnet rimraf` (`rimraf` if using as a global tool) to delete files and directories.
You can pass one or more paths or globs to delete.
Globbing is handled by the [`DotNet.Glob`](https://github.com/dazinator/DotNet.Glob) library and supports of all its patterns and wildcards.

```console
rimraf artifacts coverage
```

```console
rimraf artifacts/**/*-pre*.nupkg
```

```console
rimraf **/bin **/obj
```

### Working directory

The working directory is the directory that `rimraf` is run from.
