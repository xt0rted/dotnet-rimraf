namespace RimRaf;

internal interface IConsoleWriter
{
    void VerboseBanner();

    void BlankVerboseLine();

    void Line(string? message, params object?[] args);

    void LineVerbose(string? message = null, params object?[] args);

    void SecondaryLine(string? message, params object?[] args);

    void SecondaryLineVerbose(string? message, params object?[] args);

    void Information(string? message, params object?[] args);

    void InformationVerbose(string? message, params object?[] args);

    void Error(string? message, params object?[] args);

    void ErrorVerbose(string? message, params object?[] args);
}
