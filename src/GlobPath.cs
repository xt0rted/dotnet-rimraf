namespace RimRaf;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

using DotNet.Globbing;

using Microsoft.Extensions.FileProviders;

using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

// This class is pieced together from bits of PowerShell's recursive delete function
internal class GlobPath
{
    private const uint IO_REPARSE_TAG_APPEXECLINK = 0x8000001B;

    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatformGuard("windows5.1.2600")]
    private readonly bool _isWindows = OperatingSystem.IsWindows();

    private readonly IConsoleWriter _console;
    private readonly IFileProvider _rootDirectory;
    private readonly IFileSystem _fileSystem;
    private readonly Glob _glob;

    public GlobPath(
        IConsoleWriter console,
        IFileProvider rootDirectory,
        IFileSystem fileSystem,
        Glob glob)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _glob = glob ?? throw new ArgumentNullException(nameof(glob));
    }

    public void Delete(bool preserveRoot, bool dryRun, CancellationToken cancellationToken)
        => EnumerateFolderContents("", preserveRoot, dryRun, cancellationToken);

    public void EnumerateFolderContents(
        string path,
        bool preserveRoot,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        foreach (var item in _rootDirectory.GetDirectoryContents(path))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (string.IsNullOrEmpty(item.PhysicalPath))
            {
                _console.SecondaryLine("{0} is not accessible", item.IsDirectory ? "Directory" : "File");

                continue;
            }

            if (!item.Exists)
            {
                _console.SecondaryLine("{0} doesn't exist", item.IsDirectory ? "Directory" : "File");

                continue;
            }

            var relativePath = _fileSystem.Path.Join(path, item.Name);
            var isMatch = _glob.IsMatch(relativePath.AsSpan());

            if (isMatch)
            {
                _console.LineVerbose(item.PhysicalPath);

                if (item.IsDirectory)
                {
                    var directory = _fileSystem.DirectoryInfo.New(item.PhysicalPath);

                    RemoveDirectoryInfoItem(directory, preserveRoot, dryRun, cancellationToken);
                }
                else
                {
                    var file = _fileSystem.FileInfo.New(item.PhysicalPath);

                    RemoveFileSystemItem(file, dryRun);
                }

                continue;
            }

            if (item.IsDirectory)
            {
                EnumerateFolderContents(
                    _fileSystem.Path.Join(path, item.Name),
                    preserveRoot: false,
                    dryRun,
                    cancellationToken);
            }
        }
    }

    /// <summary>
    /// Removes a directory from the file system.
    /// </summary>
    /// <param name="directory">
    /// The DirectoryInfo object representing the directory to be removed.
    /// </param>
    /// <param name="keepSelf">
    /// If true, the directory will be kept but it's contents won't be.
    /// </param>
    /// <param name="dryRun">
    /// If true, no physical file operations actually occur.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IOException" />
    public void RemoveDirectoryInfoItem(IDirectoryInfo directory, bool keepSelf, bool dryRun, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(directory);

        if (IsReparsePointLikeSymlink(directory))
        {
            try
            {
                if (!dryRun)
                {
                    // Name surrogates should just be detached
                    directory.Delete();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("Cannot remove item {0}: {1}", directory.FullName, e.Message);

                throw new IOException(errorMessage, e);
            }

            return;
        }

        // Loop through each of the contained directories and recurse into them for removal
        foreach (var childDir in directory.EnumerateDirectories())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (childDir is not null)
            {
                RemoveDirectoryInfoItem(childDir, keepSelf: false, dryRun, cancellationToken);
            }
        }

        // Loop through each of the contained files and remove them
        foreach (var file in directory.EnumerateFiles())
        {
            // Making sure to obey the StopProcessing.
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (file is not null)
            {
                RemoveFileSystemItem(file, dryRun);
            }
        }

        // Finally, remove the directory
        if (!keepSelf)
        {
            // Check to see if the item still has children
            if (directory.EnumerateFileSystemInfos().Any())
            {
                if (!dryRun)
                {
                    _console.Error("Directory {0} cannot be removed because it is not empty", directory.FullName);
                }
            }
            else
            {
                RemoveFileSystemItem(directory, dryRun);
            }
        }
    }

    /// <summary>
    /// Removes the file system object from the file system.
    /// </summary>
    /// <param name="fileSystemInfo">
    /// The FileSystemInfo object representing the file or directory to be removed.
    /// </param>
    /// <param name="dryRun">
    /// If true, no physical file operations actually occur.
    /// </param>
    private void RemoveFileSystemItem(IFileSystemInfo fileSystemInfo, bool dryRun)
    {
        // Store the old attributes in case we fail to delete
        var oldAttributes = fileSystemInfo.Attributes;
        var attributeRecoveryRequired = false;

        try
        {
            if (!dryRun)
            {
                // Try to delete the item. Strip any problematic attributes.
                fileSystemInfo.Attributes &= ~(FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
                attributeRecoveryRequired = true;

                fileSystemInfo.Delete();

                attributeRecoveryRequired = false;
            }
        }
        catch (Exception fsException)
        {
            _console.Error("Cannot remove item {0}: {1}", fileSystemInfo.FullName, fsException.Message);

            throw;
        }
        finally
        {
            if (attributeRecoveryRequired)
            {
                try
                {
                    if (fileSystemInfo.Exists)
                    {
                        fileSystemInfo.Attributes = oldAttributes;
                    }
                }
                catch (Exception attributeException)
                {
                    if ((attributeException is DirectoryNotFoundException) ||
                        (attributeException is SecurityException) ||
                        (attributeException is ArgumentException) ||
                        (attributeException is FileNotFoundException) ||
                        (attributeException is IOException))
                    {
                        _console.Error("Cannot restore attributes on item {0}: {1}", fileSystemInfo.FullName, attributeException.Message);
                    }

                    throw;
                }
            }
        }
    }

    internal bool IsReparsePointLikeSymlink(IFileSystemInfo fileInfo)
    {
        if (!_isWindows)
        {
            // Reparse point on Unix is a symlink
            return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        var fullPath = _fileSystem.Path.TrimEndingDirectorySeparator(fileInfo.FullName);

        using (var handle = PInvoke.FindFirstFile(fullPath, out var data))
        {
            if (handle.IsInvalid)
            {
                // Our handle could be invalidated by something else touching the filesystem,
                // so ensure we deal with that possibility here
                var lastError = Marshal.GetLastWin32Error();

                throw new Win32Exception(lastError);
            }

            // We already have the file attribute information from our Win32 call,
            // so no need to take the expense of the FileInfo.FileAttributes call
            if ((data.dwFileAttributes & (uint)FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_REPARSE_POINT) == 0)
            {
                // Not a reparse point
                return false;
            }

            // The name surrogate bit 0x20000000 is defined in https://docs.microsoft.com/windows/win32/fileio/reparse-point-tags
            // Name surrogates (0x20000000) are reparse points that point to other named entities local to the filesystem
            // (like symlinks and mount points).
            // In the case of OneDrive, they are not name surrogates and would be safe to recurse into.
            if ((data.dwReserved0 & 0x20000000) == 0 && (data.dwReserved0 != IO_REPARSE_TAG_APPEXECLINK))
            {
                return false;
            }
        }

        return true;
    }
}
