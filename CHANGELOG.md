# Changelog

## Unreleased

## [0.2.0](https://github.com/xt0rted/dotnet-rimraf/compare/v0.1.0...v0.2.0) - 2024-04-10

### Added

- Added support for .NET 8

### Removed

- Dropped support for .NET Core 3.1
- Dropped support for .NET 5
- Dropped support for .NET 6

### Fixed

- Fixed console color output check

### Updated

- Bumped `Microsoft.Windows.CsWin32` from 0.1.635-beta to 0.3.49-beta
- Bumped `System.CommandLine` from 2.0.0-beta3.22114.1 to 2.0.0-beta4.22272.1
- Bumped `System.CommandLine.Rendering` from 0.4.0-alpha.22114.1 to 0.4.0-alpha.22272.1
- Bumped `System.IO.Abstractions` from 16.1.25 to 21.0.2
- Switched from [actions/setup-dotnet](https://github.com/actions/setup-dotnet) to [xt0rted/setup-dotnet](https://github.com/xt0rted/setup-dotnet)
- No longer built and tested on macOS 10.15 due to GitHub Actions [no longer supporting](https://github.com/actions/virtual-environments/issues/5583) that version

## [0.1.0](https://github.com/xt0rted/dotnet-rimraf/releases/tag/v0.1.0) - 2022-03-25

- Cross platform (tested on macOS, Ubuntu, and Windows)
- Won't return an error if the target path/glob doesn't exist
- Can install as a local or global tool
- Support for `--dry-run` to see what would be deleted
- Support for `--no-preserve-root` which will delete the directory being acted on (default behavior is to preserve it)
