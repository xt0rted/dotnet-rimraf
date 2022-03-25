namespace RimRaf;

internal static class VerifyConsole
{
    public static Task VerifyConsoleOutput(IConsole console)
        => Verify(
            console.Out
                ?.ToString()
                ?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Replace('\\', '/'))
                .OrderBy(l => l, StringComparer.Ordinal)
        );
}
