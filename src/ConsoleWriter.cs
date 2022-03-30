namespace RimRaf;

using System.CommandLine.Rendering;

internal class ConsoleWriter : IConsoleWriter
{
    private readonly IConsole _console;
    private readonly IFormatProvider? _consoleFormatProvider;
    private readonly bool _verbose;

    public ConsoleWriter(IConsole console, IFormatProvider consoleFormatProvider, bool verbose)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _consoleFormatProvider = consoleFormatProvider ?? throw new ArgumentNullException(nameof(consoleFormatProvider));
        _verbose = verbose;
    }

    private void WriteLine(
        AnsiControlCode textColor,
        bool verbose,
        string? message = null,
        params object?[] args)
    {
        if (!_verbose && verbose)
        {
            return;
        }

        if (message is not null)
        {
            _console.Out.Write(textColor.ToString(null, _consoleFormatProvider));

            if (args?.Length > 0)
            {
                _console.Out.Write(string.Format(message, args));
            }
            else
            {
                _console.Out.Write(message);
            }

            _console.Out.Write(Ansi.Color.Foreground.Default.ToString(null, _consoleFormatProvider));
        }

        _console.Out.Write(Environment.NewLine);
    }

    public void VerboseBanner()
        => LineVerbose("Verbose mode is on. This will print more information.");

    public void BlankLineVerbose()
        => WriteLine(Ansi.Color.Foreground.LightGray, verbose: true, null);

    public void Line(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.LightGray, verbose: false, message, args);

    public void LineVerbose(string? message = null, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.LightGray, verbose: true, message, args);

    public void SecondaryLine(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.DarkGray, verbose: false, message, args);

    public void SecondaryLineVerbose(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.DarkGray, verbose: true, message, args);

    public void Information(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.Cyan, verbose: false, message, args);

    public void InformationVerbose(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.Cyan, verbose: true, message, args);

    public void Error(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.Red, verbose: false, message, args);

    public void ErrorVerbose(string? message, params object?[] args)
        => WriteLine(Ansi.Color.Foreground.Red, verbose: true, message, args);
}
