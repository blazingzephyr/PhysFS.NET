
namespace Icculus.PhysFS.NET.Internals;

public static class ExceptionUtility
{
    public static void ThrowExceptionForPhysFSErr(PHYSFS_ErrorCode err, string? message)
    {
        Exception? exception = GetExceptionForPhysFSErr(err, message);
        if (exception != null)
        {
            throw exception;
        }
    }

    public static Exception? GetExceptionForPhysFSErr(PHYSFS_ErrorCode err, string? message)
    {
        return err switch
        {
            PHYSFS_ErrorCode.PHYSFS_ERR_OTHER_ERROR => new Exception(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_OUT_OF_MEMORY => new OutOfMemoryException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NOT_INITIALIZED => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_IS_INITIALIZED => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_ARGV0_IS_NULL => new ArgumentNullException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_UNSUPPORTED => new NotImplementedException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_PAST_EOF => new EndOfStreamException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_FILES_STILL_OPEN => new IOException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_INVALID_ARGUMENT => new ArgumentException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NOT_MOUNTED => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NOT_FOUND => new FileNotFoundException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_SYMLINK_FORBIDDEN => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NO_WRITE_DIR => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_OPEN_FOR_READING => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_OPEN_FOR_WRITING => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NOT_A_FILE => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_READ_ONLY => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_CORRUPT => new InvalidDataException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_SYMLINK_LOOP => new InvalidDataException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_IO => new IOException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_PERMISSION => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_NO_SPACE => new InsufficientMemoryException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_BAD_FILENAME => new ArgumentException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_BUSY => new UnauthorizedAccessException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_DIR_NOT_EMPTY => new IOException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_OS_ERROR => new SystemException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_DUPLICATE => new ArgumentException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_BAD_PASSWORD => new ArgumentException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_APP_CALLBACK => new InvalidOperationException(message),
            PHYSFS_ErrorCode.PHYSFS_ERR_OK or _ => null
        };
    }
}
