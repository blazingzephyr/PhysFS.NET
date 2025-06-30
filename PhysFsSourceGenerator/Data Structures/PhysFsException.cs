using System;
using System.IO;

namespace Icculus.PhysFS.NET.SourceGeneration;

public enum PhysFsErrorCode
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

public static class PhysFsExceptionUtility
{
    public static Exception GetExceptionForPhysFsErr(PhysFsErrorCode errorCode, string? errorText)
    {
        return errorCode switch
        {
            PhysFsErrorCode.PHYSFS_ERR_OTHER_ERROR => new Exception(errorText),
            PhysFsErrorCode.PHYSFS_ERR_OUT_OF_MEMORY => new OutOfMemoryException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NOT_INITIALIZED => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_IS_INITIALIZED => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_ARGV0_IS_NULL => new ArgumentNullException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_UNSUPPORTED => new NotImplementedException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_PAST_EOF => new EndOfStreamException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_FILES_STILL_OPEN => new IOException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_INVALID_ARGUMENT => new ArgumentException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NOT_MOUNTED => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NOT_FOUND => new FileNotFoundException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_SYMLINK_FORBIDDEN => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NO_WRITE_DIR => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_OPEN_FOR_READING => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_OPEN_FOR_WRITING => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NOT_A_FILE => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_READ_ONLY => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_CORRUPT => new InvalidDataException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_SYMLINK_LOOP => new InvalidDataException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_IO => new IOException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_PERMISSION => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_NO_SPACE => new InsufficientMemoryException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_BAD_FILENAME => new ArgumentException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_BUSY => new UnauthorizedAccessException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_DIR_NOT_EMPTY => new IOException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_OS_ERROR => new SystemException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_DUPLICATE => new ArgumentException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_BAD_PASSWORD => new ArgumentException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_APP_CALLBACK => new InvalidOperationException(errorText),
            PhysFsErrorCode.PHYSFS_ERR_OK or _ => new Exception(errorText)
        };
    }
}
