
namespace Icculus.PhysFS.NET;

/// <summary>
/// Access mode of the file system object.
/// </summary>
/// <remarks>
/// Determines which operation will be used for this file.<br/><br/>
/// 
/// See also:<br/>
/// <seealso cref="FileSystemObject"/>
/// </remarks>
public enum FileSystemObjectAccess
{
    /// <summary>
    /// Open a file for reading.
    /// </summary>
    Read = 0,

    /// <summary>
    /// Open a file for writing.
    /// Truncates the file to zero bytes.
    /// </summary>
    Truncate = 1,

    /// <summary>
    /// Open a file for writing.
    /// Sets the writing offset to the end of the file.
    /// </summary>
    Append = 2
}

/// <summary>
/// A PhysicsFS file.
/// </summary>
/// <remarks>
/// You get one of these when you open a file for reading,
/// writing, or appending via PhysFS.<br/><br/>
/// 
/// Don't try to manipulate the file handle, just pass this
/// object, unmolested, to various PhysicsFS APIs.<br/><br/>
/// 
/// See also:<br/>
/// <seealso cref="PhysFS.OpenFile(string, FileSystemObjectAccess)"/><br/>
/// <seealso cref="PhysFS.CloseFile(FileSystemObject)"/><br/>
/// <seealso cref="PhysFS.ReadBytes(FileSystemObject)"/><br/>
/// <seealso cref="PhysFS.WriteBytes(FileSystemObject, byte[])"/><br/>
/// <seealso cref="PhysFS.IsEndOfFile(FileSystemObject)"/><br/>
/// <seealso cref="PhysFS.Tell(FileSystemObject)"/><br/>
/// <seealso cref="PhysFS.Seek(FileSystemObject, ulong)"/><br/>
/// <seealso cref="PhysFS.FileLength(FileSystemObject)"/><br/>
/// <seealso cref="PhysFS.SetBufferSize(FileSystemObject, ulong)"/><br/>
/// <seealso cref="PhysFS.Flush(FileSystemObject)"/><br/>
/// </remarks>
public record FileSystemObject : IDisposable
{
    /// <summary>
    /// Full path to the file.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// File metadata.
    /// </summary>
    public required FileStat Stats { get; init; }

    /// <summary>
    /// Gets a value that determines what mode this file was opened in.
    /// </summary>
    public required FileSystemObjectAccess Access { get; init; }

    /// <summary>
    /// PhysFS file handle.
    /// </summary>
    internal FileHandle Handle { get; set; }

    /// <summary>
    /// A value that determines if the end of file has been reached.
    /// </summary>
    public bool IsEndOfFile => PhysFS.IsEndOfFile(this);

    /// <summary>
    /// Total length of a file in bytes.
    /// </summary>
    public ulong Length => PhysFS.FileLength(this);

    /// <summary>
    /// Hidden constructor so the user doesn't try to instantiate this.
    /// </summary>
    internal FileSystemObject()
    {
        Handle = FileHandle.Invalid;
    }

    /// <summary>
    /// Determine current position within a PhysicsFS file.
    /// </summary>
    /// <returns>offset in bytes from start of file</returns>
    public long Tell() => PhysFS.Tell(this);

    /// <summary>
    /// Seek to a new position within a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// The next read or write will occur at that place. Seeking past the
    /// beginning or end of the file is not allowed, and causes an error.<br/><br/>
    /// </remarks>
    /// <param name="offset"></param>
    public void Seek(ulong offset) => PhysFS.Seek(this, offset);

    /// <summary>
    /// Set up buffering for a PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// Define an i/o buffer for a file handle. A memory block of (bufsize) bytes
    /// will be allocated and associated with(handle).<br/><br/>
    /// 
    /// For files opened for reading, up to (bufsize) bytes are read from (handle)
    /// and stored in the internal buffer. Calls to
    /// <seealso cref="ReadBytes(FileSystemObject)"/> will pull
    /// from this buffer until it is empty, and then refill it for more reading.
    /// Note that compressed files, like ZIP archives, will decompress while
    /// buffering, so this can be handy for offsetting CPU-intensive operations.
    /// The buffer isn't filled until you do your next read.<br/><br/>
    ///
    /// For files opened for writing, data will be buffered to memory until the
    /// buffer is full or the buffer is flushed. Closing a handle implicitly
    /// causes a flush...check your return values!<br/><br/>
    ///
    /// Seeking, etc transparently accounts for buffering.<br/><br/>
    ///
    /// You can resize an existing buffer by calling this function more than once
    /// on the same file. Setting the buffer size to zero will free an existing
    /// buffer.<br/><br/>
    ///
    /// PhysicsFS file handles are unbuffered by default.<br/><br/>
    ///
    /// Please check the return value of this function! Failures can include
    /// not being able to seek backwards in a read-only file when removing the
    /// buffer, not being able to allocate the buffer, and not being able to
    /// flush the buffer to disk, among other unexpected problems.
    /// </remarks>
    /// <param name="size"></param>
    public void SetBufferSize(ulong size) => PhysFS.SetBufferSize(this, size);

    /// <summary>
    /// Flush a buffered PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// For buffered files opened for writing, this will put the current contents
    /// of the buffer to disk and flag the buffer as empty if possible.<br/><br/>
    /// 
    /// For buffered files opened for reading or unbuffered files, this is a safe
    /// no-op, and will report success.
    /// </remarks>
    public void Flush() => PhysFS.Flush(this);

    /// <summary>
    /// Read bytes from the PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.
    /// </remarks>
    /// <returns>
    /// The bytes read from file. Throws a warning if the number of bytes read
    /// is less than file length; this does not
    /// signify an error, necessarily (a short read may mean EOF).
    /// </returns>
    public byte[] ReadBytes() => PhysFS.ReadBytes(this);

    /// <summary>
    /// Read contents of the PhysicsFS file to a stream.
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.
    /// </remarks>
    /// <returns>
    /// An unmanaged stream. Throws a warning if the number of bytes read
    /// is less than file length; this does not
    /// signify an error, necessarily (a short read may mean EOF).
    /// </returns>
    public UnmanagedMemoryStream ReadToStream() => PhysFS.ReadToStream(this);

    /// <summary>
    /// Write data to the PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// Please note that you are limited to 63 bits (9223372036854775807 bytes),
    /// so we can return a negative value on error. If length is greater than
    /// 0x7FFFFFFFFFFFFFFF, this function will immediately fail. For systems
    /// without a 64-bit datatype, you are limited to 31 bits (0x7FFFFFFF, or
    /// 2147483647 bytes). We trust most things won't need to do multiple
    /// gigabytes of i/o in one call anyhow, but why limit things?
    /// </remarks>
    /// <param name="bytes">
    /// buffer of bytes to write to this file.
    /// </param>
    /// <returns>
    /// number of bytes written. This may be less than (len); in the case
    /// of an error, the system may try to write as many bytes as possible,
    /// so an incomplete write might occur.
    /// </returns>
    public void WriteBytes(byte[] bytes) => PhysFS.WriteBytes(this, bytes);

    /// <summary>
    /// Close the PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// This call is capable of failing if the operating system was buffering
    /// writes to the physical media, and, now forced to write those changes to
    /// physical media, can not store the data for some reason.
    /// </remarks>
    public void Dispose()
    {
        PhysFS.CloseFile(this);
        GC.SuppressFinalize(this);
    }
}
