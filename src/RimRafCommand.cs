namespace RimRaf;

using System.CommandLine.Invocation;

using DotNet.Globbing;

using Microsoft.Extensions.FileProviders;

internal class RimRafCommand : RootCommand, ICommandHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly IFileSystem _fileSystem;
    private readonly IFormatProvider _consoleFormatProvider;

    public RimRafCommand(
        IFileProvider fileProvider,
        IFileSystem fileSystem,
        IFormatProvider consoleFormatProvider)
        : base("Deep deletion command for .NET (like rm -rf)")
    {
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _consoleFormatProvider = consoleFormatProvider ?? throw new ArgumentNullException(nameof(consoleFormatProvider));

        AddArgument(ToolArguments.Paths);

        AddOption(ToolOptions.DryRun);
        AddOption(ToolOptions.NoPreserveRoot);
        AddOption(ToolOptions.Verbose);

        Handler = this;
    }

    public int Invoke(InvocationContext context)
        => throw new NotImplementedException();

    public Task<int> InvokeAsync(InvocationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var verbose = context.ParseResult.GetValueForOption(ToolOptions.Verbose);
        var dryRun = context.ParseResult.GetValueForOption(ToolOptions.DryRun);
        var console = new ConsoleWriter(context.Console, _consoleFormatProvider, verbose || dryRun);

        console.VerboseBanner();
        console.InformationVerbose("Dry run: {0}", dryRun);

        var paths = context.ParseResult.GetValueForArgument(ToolArguments.Paths);
        var preserveRoot = !context.ParseResult.GetValueForOption(ToolOptions.NoPreserveRoot);

        console.InformationVerbose("Preserve root: {0}", preserveRoot);

        var ct = context.GetCancellationToken();

        foreach (var path in paths)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            console.BlankLineVerbose();
            console.InformationVerbose("Path: {0}", path);
            console.BlankLineVerbose();

            var glob = Glob.Parse(path);

            new GlobPath(console, _fileProvider, _fileSystem, glob)
                .Delete(preserveRoot, dryRun, ct);
        }

        if (ct.IsCancellationRequested)
        {
            console.SecondaryLineVerbose("Cancellation requested");

            return Task.FromResult(1);
        }

        return Task.FromResult(0);
    }
}
