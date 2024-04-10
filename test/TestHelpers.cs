namespace RimRaf;

internal static class TestHelpers
{
    // This is a copy of the method used in the node rimraf package
    public static void LoadFileSystem(int depth, int files, int folders, string target)
    {
        Directory.CreateDirectory(target);

        for (var f = files; f > 0; f--)
        {
            File.CreateText(Path.Combine(target, $"f-{depth}-{f}")).Dispose();
        }

        // valid symlink
        CreateFileSymbolicLink(
            Path.Combine(target, $"link-{depth}-good"),
            Path.Combine(target, $"f-{depth}-1")
        );

        // invalid symlink
        CreateFileSymbolicLink(
            Path.Combine(target, $"link-{depth}-bad"),
            Path.Combine(target, "does-not-exist")
        );

        // file with a name that looks like a glob
        File.CreateText(Path.Combine(target, "[a-z0-9].txt")).Dispose();

        depth--;

        if (depth <= 0)
        {
            return;
        }

        for (var f = folders; f > 0; f--)
        {
            Directory.CreateDirectory(Path.Combine(target, $"folder-{depth}-{f}"));
            LoadFileSystem(depth, files, folders, Path.Combine(target, $"d-{depth}-{f}"));
        }
    }

    private static void CreateFileSymbolicLink(string path, string pathToTarget)
        => File.CreateSymbolicLink(path, pathToTarget);
}
