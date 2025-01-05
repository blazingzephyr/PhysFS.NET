
namespace Icculus.PhysFS.NET;

/// <summary>
/// Type of a File
/// </summary>
/// <remarks>
/// Possible types of a file.<br/><br/>
/// 
/// See also:<br/>
/// <seealso cref="FileInfo"/>
/// </remarks>
public enum FileType : byte
{
    /// <summary>
    /// A normal file eligible for reading or writing.
    /// </summary>
    File = 0,

    /// <summary>
    /// A directory.
    /// </summary>
    Directory = 1,

    /// <summary>
    /// A symlink.
    /// </summary>
    Symlink = 2,

    /// <summary>
    /// Something completely different, like a device.
    /// </summary>
    Other = 3
}

/// <summary>
/// Meta data for a file or directory.
/// </summary>
/// 
/// <remarks>
/// Container for various meta data about a file in the virtual file system.
/// <see cref="PhysFS.GetStatsFor(string)"/> uses this structure for returning the information.<br/><br/>
/// 
/// See also:<br/>
/// <seealso cref="PhysFS.GetStatsFor(string)"/><br/>
/// <seealso cref="FileType"/><br/>
/// </remarks>
public record FileStat
{
    /// <summary>
    /// Gets the size, in bytes, of the current file.
    /// </summary>
    public required long Length { get; init; }

    /// <summary>
    /// Gets the time the current file or directory was last accessed.
    /// -1 second since the Unix epoch (midnight, Jan 1, 1970) if the information ins't available.
    /// </summary>
    public required DateTime LastWriteTime { get; init; }

    /// <summary>
    /// Gets the creation time of the current file or directory.
    /// -1 second since the Unix epoch (midnight, Jan 1, 1970) if the information ins't available.
    /// </summary>
    public required DateTime CreationTime { get; init; }

    /// <summary>
    /// Gets the time the current file or directory was last accessed.
    /// -1 second since the Unix epoch (midnight, Jan 1, 1970) if the information ins't available.
    /// </summary>
    public required DateTime LastAccessTime { get; init; }

    /// <summary>
    /// Gets a value that determines whether the object is a file, a directory or a symlink.
    /// </summary>
    public required FileType Type { get; init; }

    /// <summary>
    /// Gets a value that determines hether the archive thinks a file is not writable,
    /// but tends to be only an estimate (for example, your write dir might overlap with a .zip file),
    /// you *can* successfully open that path for writing, as it gets created elsewhere.
    /// </summary>
    public required bool IsReadOnly { get; init; }

    /// <summary>
    /// Hidden constructor so the user doesn't try to instantiate this.
    /// </summary>
    internal FileStat()
    {
        // Considered naming this 'Inode', but decided to go with 'FileStat'
        // This is not a Unix file necessarily, so this is a bit more descriptive.
    }
}
