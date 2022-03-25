#pragma warning disable IDISP003

namespace RimRaf;

using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.Runtime.CompilerServices;

using DotNet.Globbing;

using Microsoft.Extensions.FileProviders;

using static VerifyConsole;

[UsesVerify]
public sealed class GlobPathTests : IClassFixture<GlobPathTestEnvironment>, IDisposable
{
    private readonly FileSystem _fileSystem = new();
    private readonly CancellationTokenSource _cancellationSource = new();

    // ctor
    private readonly GlobPathTestEnvironment _environment;

    // SetUpTest
    private string? _testName;
    private string? _testPath;
    private PhysicalFileProvider? _fileProvider;

    public GlobPathTests(GlobPathTestEnvironment environment)
    {
        _environment = environment;
        _environment.SetFixtureName(GetType().Name);
    }

    public void Dispose()
    {
        _fileProvider?.Dispose();
        _environment?.CleanUpTest(_testName);
    }

    [Fact]
    public async Task Should_delete_folder()
    {
        // Given
        var (console, path, targetFolder) = SetUpTest();

        // When
        path.Delete(preserveRoot: false, dryRun: false, _cancellationSource.Token);

        // Then
        Directory.Exists(targetFolder).ShouldBeFalse();

        await VerifyConsoleOutput(console);
    }

    [Fact]
    public async Task Should_delete_folder_contents()
    {
        // Given
        var (console, path, targetFolder) = SetUpTest();

        // When
        path.Delete(preserveRoot: true, dryRun: false, _cancellationSource.Token);

        // Then
        Directory.Exists(targetFolder).ShouldBeTrue();
        Directory.GetDirectories(targetFolder).ShouldBeEmpty();
        Directory.GetFiles(targetFolder).ShouldBeEmpty();

        await VerifyConsoleOutput(console);
    }

    [Fact]
    public void Should_not_delete_if_cancelled()
    {
        // Given
        var (console, path, targetFolder) = SetUpTest();

        // When
        _cancellationSource.Cancel();

        path.Delete(preserveRoot: false, dryRun: false, _cancellationSource.Token);

        // Then
        Directory.Exists(targetFolder).ShouldBeTrue();
        Directory.GetDirectories(targetFolder).ShouldNotBeEmpty();
        Directory.GetFiles(targetFolder).ShouldNotBeEmpty();

        console.Out.ToString().ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_delete_a_glob()
    {
        // Given
        var (console, path, targetFolder) = SetUpTest("**/f-*-1");

        // When
        path.Delete(preserveRoot: true, dryRun: false, _cancellationSource.Token);

        // Then
        Directory.Exists(targetFolder).ShouldBeTrue();
        Directory.GetDirectories(targetFolder).ShouldNotBeEmpty();
        Directory.GetFiles(targetFolder).ShouldNotBeEmpty();

        await VerifyConsoleOutput(console);
    }

    private (IConsole console, GlobPath path, string targetFolder) SetUpTest(
        string globPattern = "test",
        [CallerMemberName] string testName = "")
    {
        _testName = testName;
        _testPath = Path.Join(_environment.TestFolder, _testName);

        Directory.CreateDirectory(_testPath);

        var targetFolder = Path.Join(_testPath, "test");

        TestHelpers.LoadFileSystem(4, 10, 2, targetFolder);

        _fileProvider = new PhysicalFileProvider(_testPath);

        var console = new TestConsole();
        var consoleFormatProvider = new ConsoleFormatInfo
        {
            SupportsAnsiCodes = false,
        };
        var consoleWriter = new ConsoleWriter(console, consoleFormatProvider, verbose: true);

        var glob = Glob.Parse(globPattern);

        var path = new GlobPath(
            consoleWriter,
            _fileProvider,
            _fileSystem,
            glob);

        return (console, path, targetFolder);
    }
}
