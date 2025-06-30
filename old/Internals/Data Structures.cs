
namespace Old.Icculus.PhysFS.NET.Internals;

public struct PHYSFS_ArchiveInfo
{
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string extension;      /**< Archive file extension: "ZIP", for example. */
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string description;    /**< Human-readable archive description. */
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string author;         /**< Person who did support for this archive. */
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string url;            /**< URL related to this archive */
    [MarshalAs(UnmanagedType.I1)]
    public bool supportsSymlinks; /**< non-zero if archive offers symbolic links. */
}

public struct PHYSFS_Version
{
    public byte major; /**< major revision */
    public byte minor; /**< minor revision */
    public byte patch; /**< patchlevel */
}


public enum PHYSFS_EnumerateCallbackResult
{
    PHYSFS_ENUM_ERROR = -1,   /**< Stop enumerating, report error to app. */
    PHYSFS_ENUM_STOP = 0,     /**< Stop enumerating, report success to app. */
    PHYSFS_ENUM_OK = 1        /**< Keep enumerating, no problems */
}

public delegate PHYSFS_EnumerateCallbackResult PHYSFS_EnumerateCallback
(
    IntPtr data,
    [MarshalUsing(typeof(Utf8StringMarshallerSkipFree))] string origdir,
    [MarshalUsing(typeof(Utf8StringMarshallerSkipFree))] string fname
);

public enum PHYSFS_FileType : byte
{
    PHYSFS_FILETYPE_REGULAR,   /**< a normal file */
    PHYSFS_FILETYPE_DIRECTORY, /**< a directory */
    PHYSFS_FILETYPE_SYMLINK,   /**< a symlink */
    PHYSFS_FILETYPE_OTHER      /**< something completely different like a device */
}

public struct PHYSFS_Stat
{
    public long filesize;            /**< size in bytes, -1 for non-files and unknown */
    public long modtime;             /**< last modification time */
    public long createtime;          /**< like modtime, but for file creation time */
    public long accesstime;          /**< like modtime, but for file access time */
    public PHYSFS_FileType filetype; /**< File? Directory? Symlink? */
    public int isreadonly;           /**< non-zero if read only, zero if writable. */
}

public delegate void UnmountCallback(IntPtr memory);
