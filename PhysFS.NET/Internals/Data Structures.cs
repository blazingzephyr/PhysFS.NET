
namespace Icculus.PhysFS.NET.Internals;

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

public enum PHYSFS_ErrorCode
{
    PHYSFS_ERR_OK,               /**< Success; no error.                    */
    PHYSFS_ERR_OTHER_ERROR,      /**< Error not otherwise covered here.     */
    PHYSFS_ERR_OUT_OF_MEMORY,    /**< Memory allocation failed.             */
    PHYSFS_ERR_NOT_INITIALIZED,  /**< PhysicsFS is not initialized.         */
    PHYSFS_ERR_IS_INITIALIZED,   /**< PhysicsFS is already initialized.     */
    PHYSFS_ERR_ARGV0_IS_NULL,    /**< Needed argv[0], but it is NULL.       */
    PHYSFS_ERR_UNSUPPORTED,      /**< Operation or feature unsupported.     */
    PHYSFS_ERR_PAST_EOF,         /**< Attempted to access past end of file. */
    PHYSFS_ERR_FILES_STILL_OPEN, /**< Files still open.                     */
    PHYSFS_ERR_INVALID_ARGUMENT, /**< Bad parameter passed to an function.  */
    PHYSFS_ERR_NOT_MOUNTED,      /**< Requested archive/dir not mounted.    */
    PHYSFS_ERR_NOT_FOUND,        /**< File (or whatever) not found.         */
    PHYSFS_ERR_SYMLINK_FORBIDDEN,/**< Symlink seen when not permitted.      */
    PHYSFS_ERR_NO_WRITE_DIR,     /**< No write dir has been specified.      */
    PHYSFS_ERR_OPEN_FOR_READING, /**< Wrote to a file opened for reading.   */
    PHYSFS_ERR_OPEN_FOR_WRITING, /**< Read from a file opened for writing.  */
    PHYSFS_ERR_NOT_A_FILE,       /**< Needed a file, got a directory (etc). */
    PHYSFS_ERR_READ_ONLY,        /**< Wrote to a read-only filesystem.      */
    PHYSFS_ERR_CORRUPT,          /**< Corrupted data encountered.           */
    PHYSFS_ERR_SYMLINK_LOOP,     /**< Infinite symbolic link loop.          */
    PHYSFS_ERR_IO,               /**< i/o error (hardware failure, etc).    */
    PHYSFS_ERR_PERMISSION,       /**< Permission denied.                    */
    PHYSFS_ERR_NO_SPACE,         /**< No space (disk full, over quota, etc) */
    PHYSFS_ERR_BAD_FILENAME,     /**< Filename is bogus/insecure.           */
    PHYSFS_ERR_BUSY,             /**< Tried to modify a file the OS needs.  */
    PHYSFS_ERR_DIR_NOT_EMPTY,    /**< Tried to delete dir with files in it. */
    PHYSFS_ERR_OS_ERROR,         /**< Unspecified OS-level error.           */
    PHYSFS_ERR_DUPLICATE,        /**< Duplicate entry.                      */
    PHYSFS_ERR_BAD_PASSWORD,     /**< Bad password.                         */
    PHYSFS_ERR_APP_CALLBACK      /**< Application callback reported error.  */
};

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
