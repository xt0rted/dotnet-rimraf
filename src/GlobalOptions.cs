namespace RimRaf;

internal static class ToolOptions
{
    public static readonly Option<bool> DryRun = new("--dry-run", "See what would be deleted (enables --verbose).");

    public static readonly Option<bool> NoPreserveRoot = new("--no-preserve-root", "Delete the directory being acted on instead of preserving it.");

    public static readonly Option<bool> Verbose = new("--verbose", "Enable verbose output");
}
