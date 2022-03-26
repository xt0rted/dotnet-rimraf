using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using RimRaf;

var workingDirectory = Environment.CurrentDirectory;

using (var rootDirectory = new PhysicalFileProvider(workingDirectory, ExclusionFilters.None))
{
     var consoleFormatProvider = ConsoleFormatInfo.CurrentInfo;
    var rootCommand = new RimRafCommand(rootDirectory, new FileSystem(), consoleFormatProvider);
    var parser = new CommandLineBuilder(rootCommand)
        .UseVersionOption()
        .UseHelp()
        .UseParseDirective()
        .UseSuggestDirective()
        .RegisterWithDotnetSuggest()
        .UseParseErrorReporting()
        .UseExceptionHandler((ex, ctx) =>
        {
            var verbose = ctx.ParseResult.HasOption(ToolOptions.Verbose);
            var writer = new ConsoleWriter(ctx.Console, consoleFormatProvider, verbose);

            if (verbose)
            {
                writer.Error(ex.ToString());
            }
            else
            {
                writer.Error(ex.Message);
            }
        })
        .CancelOnProcessTermination()
        .Build();

    var parseResult = parser.Parse(args);

    return await parseResult.InvokeAsync();
}
