# <img src="assets/icon.svg" align="left" height="45"> dotnet-rimraf

[![CI build status](https://github.com/xt0rted/dotnet-rimraf/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/xt0rted/dotnet-rimraf/actions/workflows/ci.yml)
[![NuGet Package](https://img.shields.io/nuget/v/rimraf?logo=nuget)](https://www.nuget.org/packages/rimraf)
[![GitHub Package Registry](https://img.shields.io/badge/github-package_registry-yellow?logo=nuget)](https://nuget.pkg.github.com/xt0rted/index.json)
[![Project license](https://img.shields.io/github/license/xt0rted/dotnet-rimraf)](LICENSE)

Deep deletion command for .NET (like rm -rf).

This is based on the [node tool](https://github.com/isaacs/rimraf) of the same name.

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

## Keeping current

Tools like [Dependabot](https://github.com/apps/dependabot) (https://github.com/github/feedback/discussions/13825) and [Renovate](https://github.com/marketplace/renovate) don't currently support updating dotnet local tools.
One way to automate this is to use a [GitHub Actions workflow](https://github.com/xt0rted/dotnet-tool-update-test) to check for updates and create PRs when new versions are available, which is what this repo does.

## Options

Name | Description
-- | --
`--dry-run` | See what would be deleted (enables `--verbose`)
`--no-preserve-root` | Delete the directory being acted on instead of preserving it
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

## Development

This project uses the [run-script](https://github.com/xt0rted/dotnet-run-script) dotnet tool to manage its build and test scripts.
To use this you'll need to run `dotnet tool install` and then `dotnet r` to see the available commands or look at the `scripts` section in the [global.json](global.json).
