# Changelog

## Unreleased

- Fixed console color output check
- Bumped `Emet.FileSystems` from 0.0.4 to 0.0.5

## [0.1.0](https://github.com/xt0rted/dotnet-rimraf/releases/tag/v0.1.0) - 2022-03-25

- Cross platform (tested on macOS, Ubuntu, and Windows)
- Won't return an error if the target path/glob doesn't exist
- Can install as a local or global tool
- Support for `--dry-run` to see what would be deleted
- Support for `--no-preserve-root` which will delete the directory being acted on (default behavior is to preserve it)
