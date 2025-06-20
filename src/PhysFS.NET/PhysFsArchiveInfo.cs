
namespace Icculus.PhysFS.NET;

internal unsafe struct PHYSFS_ArchiveInfo
{
    public byte* extension;
    public byte* description;
    public byte* author;
    public byte* url;
    public int supportsSymlinks;
}

/// <summary>
/// Information on various PhysicsFS-supported archives.
/// </summary>
/// <remarks>
/// This structure gives you details on what sort of archives are supported
/// by this implementation of PhysicsFS. Archives tend to be things like
/// ZIP files and such.<br/><br/>
/// Not all binaries are created equal! PhysicsFS can be built with
/// or without support for various archives. You can check with
/// <see cref="PhysicsFS.SupportedArchiveTypes"/> to see if your archive type is
/// supported.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.SupportedArchiveTypes"/><br/>
/// <seealso cref="PhysicsFS.RegisterArchiver"/><br/>
/// <seealso cref="PhysicsFS.DeregisterArchiver"/>
/// </remarks>
public record PhysFsArchiveInfo
{
    /// <summary>
    /// Archive file extension: "ZIP", for example.
    /// </summary>
    public string? Extension { get; init; }

    /// <summary>
    /// Human-readable archive description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Person who did support for this archive.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// URL related to this archive
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// <see langword="true"/> if archive offers symbolic links.
    /// </summary>
    public bool SupportsSymlinks { get; init; }

    internal static PhysFsArchiveInfo FromNative(PHYSFS_ArchiveInfo info)
    {
        unsafe
        {
            return new PhysFsArchiveInfo
            {
                Extension = Utf8StringMarshaller.ConvertToManaged(info.extension),
                Description = Utf8StringMarshaller.ConvertToManaged(info.description),
                Author = Utf8StringMarshaller.ConvertToManaged(info.author),
                Url = Utf8StringMarshaller.ConvertToManaged(info.url),
                SupportsSymlinks = info.supportsSymlinks != 0
            };
        }
    }

    internal PHYSFS_ArchiveInfo ToNative()
    {
        unsafe
        {
            return new PHYSFS_ArchiveInfo
            {
                extension = Utf8StringMarshaller.ConvertToUnmanaged(Extension),
                description = Utf8StringMarshaller.ConvertToUnmanaged(Description),
                author = Utf8StringMarshaller.ConvertToUnmanaged(Author),
                url = Utf8StringMarshaller.ConvertToUnmanaged(Url),
                supportsSymlinks = SupportsSymlinks ? 1 : 0
            };
        }
    }
}
