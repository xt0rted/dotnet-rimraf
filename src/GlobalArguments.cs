namespace RimRaf;

internal static class ToolArguments
{
    public static readonly Argument<string[]> Paths = new("paths")
    {
        Arity = ArgumentArity.OneOrMore,
    };
}
