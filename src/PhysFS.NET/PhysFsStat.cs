namespace Icculus.PhysFS.NET;

/// <summary>
/// Type of a File
/// </summary>
/// <remarks>
/// Possible types of a file.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.Stat"/>
/// </remarks>
public enum PhysFsFileType
{
    /// <summary>
    /// a normal file
    /// </summary>
    Regular,
    /// <summary>
    /// a directory
    /// </summary>
    Directory,
    /// <summary>
    /// a symlink
    /// </summary>
    Symlink,
    /// <summary>
    /// something completely different like a device
    /// </summary>
    Unknown
}

/// <summary>
/// Meta data for a file or directory
/// </summary>
/// <remarks>
/// Container for various meta data about a file in the virtual file system.
/// <see cref="PhysicsFS.Stat"/> uses this structure for
/// returning the information. The time data will be either the number of
/// seconds since the Unix epoch (midnight, Jan 1, 1970), or -1 if the information
/// isn't available or applicable.<br/>
/// The <see cref="FileSize"/> field is measured in bytes.<br/>
/// The <see cref="IsReadOnly"/> field tells you whether the archive thinks a file is
/// not writable, but tends to be only an estimate (for example, your write
/// dir might overlap with a .zip file, meaning you _can_ successfully open
/// that path for writing, as it gets created elsewhere.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.Stat"/><br/>
/// <seealso cref="PhysFsFileType"/>
/// </remarks>
public record PhysFsStat
{
    /// <summary>
    /// Size in bytes, -1 for non-files and unknown.
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// Last file modification time.
    /// </summary>
    public DateTime LastModifiedAt { get; init; }

    /// <summary>
    /// Like <see cref="LastModifiedAt"/>, but for file creation time.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Like <see cref="LastModifiedAt"/>, but for file access time.
    /// </summary>
    public DateTime LastAccessedAt { get; init; }

    /// <summary>
    /// File? Directory? Symlink?
    /// </summary>
    public PhysFsFileType FileType { get; init; }

    /// <summary>
    /// <see langword="true"/> if read only, <see langword="false"/> if writable.
    /// </summary>
    public bool IsReadOnly { get; init; }
}
