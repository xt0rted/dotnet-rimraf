namespace RimRaf;

using System.CommandLine.IO;
using System.CommandLine.Rendering;

using DotNet.Globbing;

using Microsoft.Extensions.FileProviders;

public sealed class GlobPathTestEnvironment : IDisposable
{
    public GlobPathTestEnvironment()
        => TestRoot = AttributeReader.GetProjectDirectory(GetType().Assembly);

    public string TestRoot { get; }

    public string? TestFixture { get; private set; }

    public string TestFolder
    {
        get
        {
            if (TestFixture is null)
            {
                throw new InvalidOperationException($"{nameof(SetFixtureName)} was not called");
            }

            return Path.Join(TestRoot, TestFixture);
        }
    }

    public void SetFixtureName(string name)
        => TestFixture = name + "_";

    public void CleanUpTest(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        // This cleanup process assumes we knew what we were doing and everything
        // works, if it doesn't then I'm sorry for the extra files & folders that
        // will be left hanging around.
        var testPath = Path.Join(TestFolder, name);
        var consoleFormatProvider = new ConsoleFormatInfo();
        var consoleWriter = new ConsoleWriter(
            new SystemConsole(),
            consoleFormatProvider,
            verbose: false);
        var fileSystem = new FileSystem();
        var glob = Glob.Parse("**/*");

        using var fileProvider = new PhysicalFileProvider(testPath);

        var path = new GlobPath(
            consoleWriter,
            fileProvider,
            fileSystem,
            glob);

        path.Delete(preserveRoot: false, dryRun: false, cancellationToken: default);

        Directory.Delete(testPath);
    }

    public void Dispose() => Directory.Delete(TestFolder);
}
