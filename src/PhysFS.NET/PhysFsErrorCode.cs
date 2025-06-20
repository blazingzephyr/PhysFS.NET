namespace Icculus.PhysFS.NET;

/// <summary>
/// Values that represent specific causes of failure.
/// </summary>
/// <remarks>
/// Most of the time, you should only concern yourself with whether a given
/// operation failed or not, but there may be occasions where you plan to
/// handle a specific failure case gracefully, so we provide specific error
/// codes.<br/><br/>
/// Most of these errors are a little vague, and most aren't things you can
/// fix...if there's a permission error, for example, all you can really do
/// is pass that information on to the user and let them figure out how to
/// handle it. In most these cases, your program should only care that it
/// failed to accomplish its goals, and not care specifically why.<br/><br/><br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.GetLastErrorCode"/><br/>
/// <seealso cref="PhysicsFS.GetErrorByCode"/><br/>
/// <seealso cref="PhysFsExceptionUtility.GetExceptionForPhysFsErr"/>
/// </remarks>
public enum PhysFsErrorCode
{
    /// <summary>
    /// Success; no error.
    /// </summary>
    PHYSFS_ERR_OK,
    /// <summary>
    /// Error not otherwise covered here. 
    /// </summary>
    PHYSFS_ERR_OTHER_ERROR,
    /// <summary>
    /// Memory allocation failed.
    /// </summary>
    PHYSFS_ERR_OUT_OF_MEMORY,
    /// <summary>
    /// PhysicsFS is not initialized.
    /// </summary>
    PHYSFS_ERR_NOT_INITIALIZED,
    /// <summary>
    /// PhysicsFS is already initialized.
    /// </summary>
    PHYSFS_ERR_IS_INITIALIZED,
    /// <summary>
    /// Needed argv[0], but it is <see langword="null"/>.
    /// </summary>
    PHYSFS_ERR_ARGV0_IS_NULL,
    /// <summary>
    /// Operation or feature unsupported.
    /// </summary>
    PHYSFS_ERR_UNSUPPORTED,
    /// <summary>
    /// Attempted to access past end of file.
    /// </summary>
    PHYSFS_ERR_PAST_EOF,
    /// <summary>
    /// Files still open.
    /// </summary>
    PHYSFS_ERR_FILES_STILL_OPEN,
    /// <summary>
    /// Bad parameter passed to an function.
    /// </summary>
    PHYSFS_ERR_INVALID_ARGUMENT,
    /// <summary>
    /// Requested archive/dir not mounted.
    /// </summary>
    PHYSFS_ERR_NOT_MOUNTED,
    /// <summary>
    /// File (or whatever) not found.
    /// </summary>
    PHYSFS_ERR_NOT_FOUND,
    /// <summary>
    /// Symlink seen when not permitted.
    /// </summary>
    PHYSFS_ERR_SYMLINK_FORBIDDEN,
    /// <summary>
    /// No write dir has been specified.
    /// </summary>
    PHYSFS_ERR_NO_WRITE_DIR,
    /// <summary>
    /// Wrote to a file opened for reading.
    /// </summary>
    PHYSFS_ERR_OPEN_FOR_READING,
    /// <summary>
    /// Read from a file opened for writing.
    /// </summary>
    PHYSFS_ERR_OPEN_FOR_WRITING,
    /// <summary>
    /// Needed a file, got a directory (etc).
    /// </summary>
    PHYSFS_ERR_NOT_A_FILE,
    /// <summary>
    /// Wrote to a read-only filesystem.
    /// </summary>
    PHYSFS_ERR_READ_ONLY,
    /// <summary>
    /// Corrupted data encountered.
    /// </summary>
    PHYSFS_ERR_CORRUPT,
    /// <summary>
    /// Infinite symbolic link loop.
    /// </summary>
    PHYSFS_ERR_SYMLINK_LOOP,
    /// <summary>
    /// i/o error (hardware failure, etc).
    /// </summary>
    PHYSFS_ERR_IO,
    /// <summary>
    /// Permission denied.
    /// </summary>
    PHYSFS_ERR_PERMISSION,
    /// <summary>
    /// No space (disk full, over quota, etc)
    /// </summary>
    PHYSFS_ERR_NO_SPACE,
    /// <summary>
    /// Filename is bogus/insecure.
    /// </summary>
    PHYSFS_ERR_BAD_FILENAME,
    /// <summary>
    /// Tried to modify a file the OS needs.
    /// </summary>
    PHYSFS_ERR_BUSY,
    /// <summary>
    /// Tried to delete dir with files in it.
    /// </summary>
    PHYSFS_ERR_DIR_NOT_EMPTY,
    /// <summary>
    /// Unspecified OS-level error.
    /// </summary>
    PHYSFS_ERR_OS_ERROR,
    /// <summary>
    /// Duplicate entry.
    /// </summary>
    PHYSFS_ERR_DUPLICATE,
    /// <summary>
    /// Bad password.
    /// </summary>
    PHYSFS_ERR_BAD_PASSWORD,
    /// <summary>
    /// Application callback reported error.
    /// </summary>
    PHYSFS_ERR_APP_CALLBACK
};

/// <summary>
/// A simple utility for retrieving C# exceptions corresponding to PhysicsFS errors.
/// </summary>
public static class PhysFsExceptionUtility
{
    /// <summary>
    /// Get the exception for the current error code.
    /// </summary>
    /// <param name="errorCode">
    /// Usually returned from <see cref="PhysicsFS.GetLastErrorCode"/>.
    /// </param>
    /// <param name="errorText">
    /// Additional text information from <see cref="PhysicsFS.GetErrorByCode"/>.
    /// </param>
    /// <returns>An exception describing the provided PhysicsFS error.</returns>
    public static Exception GetExceptionForPhysFsErr(PhysFsErrorCode errorCode, string? errorText)
    {
        string text = $"{errorCode}: {errorText}.";
        return errorCode switch
        {
            PhysFsErrorCode.PHYSFS_ERR_OTHER_ERROR       => new Exception(text),
            PhysFsErrorCode.PHYSFS_ERR_OUT_OF_MEMORY     => new OutOfMemoryException(text),
            PhysFsErrorCode.PHYSFS_ERR_NOT_INITIALIZED   => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_IS_INITIALIZED    => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_ARGV0_IS_NULL     => new ArgumentNullException(text),
            PhysFsErrorCode.PHYSFS_ERR_UNSUPPORTED       => new NotImplementedException(text),
            PhysFsErrorCode.PHYSFS_ERR_PAST_EOF          => new EndOfStreamException(text),
            PhysFsErrorCode.PHYSFS_ERR_FILES_STILL_OPEN  => new IOException(text),
            PhysFsErrorCode.PHYSFS_ERR_INVALID_ARGUMENT  => new ArgumentException(text),
            PhysFsErrorCode.PHYSFS_ERR_NOT_MOUNTED       => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_NOT_FOUND         => new FileNotFoundException(text),
            PhysFsErrorCode.PHYSFS_ERR_SYMLINK_FORBIDDEN => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_NO_WRITE_DIR      => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_OPEN_FOR_READING  => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_OPEN_FOR_WRITING  => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_NOT_A_FILE        => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_READ_ONLY         => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_CORRUPT           => new InvalidDataException(text),
            PhysFsErrorCode.PHYSFS_ERR_SYMLINK_LOOP      => new InvalidDataException(text),
            PhysFsErrorCode.PHYSFS_ERR_IO                => new IOException(text),
            PhysFsErrorCode.PHYSFS_ERR_PERMISSION        => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_NO_SPACE          => new InsufficientMemoryException(text),
            PhysFsErrorCode.PHYSFS_ERR_BAD_FILENAME      => new ArgumentException(text),
            PhysFsErrorCode.PHYSFS_ERR_BUSY              => new UnauthorizedAccessException(text),
            PhysFsErrorCode.PHYSFS_ERR_DIR_NOT_EMPTY     => new IOException(text),
            PhysFsErrorCode.PHYSFS_ERR_OS_ERROR          => new SystemException(text),
            PhysFsErrorCode.PHYSFS_ERR_DUPLICATE         => new ArgumentException(text),
            PhysFsErrorCode.PHYSFS_ERR_BAD_PASSWORD      => new ArgumentException(text),
            PhysFsErrorCode.PHYSFS_ERR_APP_CALLBACK      => new InvalidOperationException(text),
            PhysFsErrorCode.PHYSFS_ERR_OK or _           => new NotSupportedException(text)
        };
    }
}
