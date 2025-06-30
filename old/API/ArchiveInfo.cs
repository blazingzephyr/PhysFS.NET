
namespace Old.Icculus.PhysFS.NET;

/// <summary>
/// Information on various PhysicsFS-supported archives.
/// </summary>
/// <remarks>
/// This structure gives you details on what sort of archives are supported
/// by this implementation of PhysicsFS. Archives tend to be things like
/// ZIP files and such.<br/><br/>
/// 
/// See also:
/// <seealso cref="PhysFS.GetSupportedArchiveTypes"/>
/// </remarks>
public record ArchiveInfo
{
    /// <summary>
    /// Archive file extension.
    /// </summary>
    public required string Extension { get; init; }

    /// <summary>
    /// Human-readable archive description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Person who did support for this archive.
    /// </summary>
    public required string Author { get; init; }

    /// <summary>
    /// URL related to this archive.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets a value that determines whether the archive offers symbolic links.
    /// </summary>
    public required bool SupportsSymlinks { get; init; }

    /// <summary>
    /// Hidden constructor so the user doesn't try to instantiate this.
    /// </summary>
    internal ArchiveInfo()
    {

    }
}
