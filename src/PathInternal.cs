#if NETCOREAPP3_1

#pragma warning disable IO0006

namespace RimRaf;

internal static class PathInternal
{
    public static string TrimEndingDirectorySeparator(this IPath _, string path)
        => Path.TrimEndingDirectorySeparator(path);
}

#endif
