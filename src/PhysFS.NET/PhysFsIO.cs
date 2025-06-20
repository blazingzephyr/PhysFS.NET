using static Icculus.PhysFS.NET.PhysFsIo;

namespace Icculus.PhysFS.NET;

/// <summary>
/// Read more data.
/// </summary>
/// <remarks>
/// Read <paramref name="len"/> bytes from the interface, at the current i/o
/// position, and store them in <paramref name="buf"/>.
/// The current i/o position should move ahead by the number of
/// bytes successfully read.<br/><br/>
/// <paramref name="len"/> bytes long and can't be <see langword="null"/>.
/// </remarks>
/// <param name="io">The i/o instance to read from.</param>
/// <param name="buf">The buffer to store data into. It must be at least</param>
/// <param name="len">The number of bytes to read from the interface.</param>
/// <returns>number of bytes read from file, 0 on EOF, -1 if complete failure.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate ulong PhysFsIoRead(ref IoHandle io, IntPtr buf, ulong len);

/// <summary>
/// Write more data.
/// </summary>
/// <remarks>
/// Write <paramref name="len"/> bytes from <paramref name="buffer"/>
/// to the interface at the current i/o
/// position. The current i/o position should move ahead by the number of
/// bytes successfully written.<br/><br/>
/// You are allowed to buffer; a write can succeed here and then later
/// fail when flushing. Note that PHYSFS_setBuffer() may be operating a
/// level above your i/o, so you should usually not implement your
/// own buffering routines.<br/><br/>
/// <paramref name="len"/> bytes long and can't be <see langword="null"/>.
/// </remarks>
/// <param name="io">The i/o instance to write to.</param>
/// <param name="buffer">The buffer to read data from. It must be at least</param>
/// <param name="len">The number of bytes to read from (buffer).</param>
/// <returns>number of bytes written to file, -1 if complete failure.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate ulong PhysFsIoWrite(ref IoHandle io, IntPtr buffer, ulong len);

/// <summary>
/// Move i/o position to a given byte offset from start.
/// </summary>
/// <remarks>
/// This method moves the i/o position, so the next read/write will
/// be of the byte at <paramref name="offset"/> offset. Seeks past the end of
/// file should be treated as an error condition.
/// </remarks>
/// <param name="io">The i/o instance to seek.</param>
/// <param name="offset">The new byte offset for the i/o position.</param>
/// <returns>
/// <see langword="true"/> on success, <see langword="false"/> on error.
/// </returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool PhysFsIoSeek(ref IoHandle io, ulong offset);

/// <summary>
/// Report current i/o position.
/// </summary>
/// <remarks>
/// Return bytes offset, or -1 if you aren't able to determine. A failure
/// will almost certainly be fatal to further use of this stream, so you
/// may not leave this unimplemented.
/// </remarks>
/// <param name="io">The i/o instance to query.</param>
/// <returns>The current byte offset for the i/o position, -1 if unknown.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate ulong PhysFsIoTell(ref IoHandle io);

/// <summary>
/// Determine size of the i/o instance's dataset.
/// </summary>
/// <remarks>
/// Return number of bytes available in the file, or -1 if you
/// aren't able to determine. A failure will almost certainly be fatal
/// to further use of this stream, so you may not leave this unimplemented.
/// </remarks>
/// <param name="io">The i/o instance to query.</param>
/// <returns>Total size, in bytes, of the dataset.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate ulong PhysFsIoLength(ref IoHandle io);

/// <summary>
/// Duplicate this i/o instance.
/// </summary>
/// <remarks>
/// This needs to result in a full copy of this <see cref="PhysFsIo"/>,
/// that can live completely independently. The copy needs to be able to
/// perform all its operations without altering the original, including
/// either object being destroyed separately (so, for example: they can't
/// share a file handle; they each need their own).<br/><br/>
/// If you can't duplicate a handle, it's legal to return <see langword="null"/>,
/// but you almost certainly need this functionality if you want to use
/// this to <see cref="PhysFsIo"/> to back an archive.
/// </remarks>
/// <param name="io">The i/o instance to duplicate.</param>
/// <returns>
/// A new value for a stream's opaque field, or <see langword="null"/> on error.
/// </returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate ref IoHandle PhysFsIoDuplicate(ref IoHandle io);

/// <summary>
/// Flush resources to media, or wherever.
/// </summary>
/// <remarks>
/// This is the chance to report failure for writes that had claimed
/// success earlier, but still had a chance to actually fail.
/// This function may be called before <see cref="PhysFsIo.Destroy"/>,
/// as it can report failure and <see cref="PhysFsIo.Destroy"/> can not.
/// It may be called at other times, too.
/// </remarks>
/// <param name="io">The i/o instance to flush.</param>
/// <returns>
/// <see langword="false"/> on error, <see langword="true"/> on success.
/// </returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool PhysFsIoFlush(ref IoHandle io);

/// <summary>
/// Cleanup and deallocate i/o instance.
/// </summary>
/// <remarks>
/// Free associated resources, including opaque if applicable.<br/><br/>
/// This function must always succeed: as such, it returns void. The
/// system may call your <see cref="PhysFsIo.Flush"/> method before this.
/// You may report failure there if necessary. This method may still be
/// called if <see cref="PhysFsIo.Flush"/> fails, in which case you'll have
/// to abandon unflushed data and other failing conditions and clean up.<br/><br/>
/// Once this method is called for a given instance, the system will assume
/// it is unsafe to touch that instance again and will discard any
/// references to it.
/// </remarks>
/// <param name="s">The i/o instance to destroy.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PhysFsIoDestroy(ref IoHandle s);

/// <summary>
/// An abstract i/o interface.
/// </summary>
/// <remarks>
/// This is advanced, hardcore stuff. You don't need this unless you
/// really know what you're doing. Most apps will not need this.<br/><br/>
/// Historically, PhysicsFS provided access to the physical filesystem and
/// archives within that filesystem. However, sometimes you need more power
/// than this. Perhaps you need to provide an archive that is entirely
/// contained in RAM, or you need to bridge some other file i/o API to
/// PhysicsFS, or you need to translate the bits (perhaps you have a
/// a standard .zip file that's encrypted, and you need to decrypt on the fly
/// for the unsuspecting zip archiver).<br/><br/>
/// A <see cref="PhysFsIo"/> is the interface that Archivers use to get archive data.
/// Historically, this has mapped to file i/o to the physical filesystem, but
/// as of PhysicsFS 2.1, applications can provide their own i/o implementations
/// at runtime.<br/><br/>
/// This interface isn't necessarily a good universal fit for i/o. There are a
/// few requirements of note:<br/><br/>
/// - They only do blocking i/o (at least, for now).
/// - They need to be able to duplicate. If you have a file handle from
///   fopen(), you need to be able to create a unique clone of it (so we
///   have two handles to the same file that can both seek/read/etc without
///   stepping on each other).
/// - They need to know the size of their entire data set.
/// - They need to be able to seek and rewind on demand.<br/><br/>
/// ...in short, you're probably not going to write an HTTP implementation.<br/><br/>
/// Thread safety: <see cref="PhysFsIo"/> implementations are not guaranteed to be thread
/// safe in themselves. Under the hood where PhysicsFS uses them, the library
/// provides its own locks. If you plan to use them directly from separate
/// threads, you should either use mutexes to protect them, or don't use the
/// same <see cref="PhysFsIo"/> from two threads at the same time.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.MountIo"/>
/// </remarks>
public readonly ref struct PhysFsIo
{
    /// <summary>
    /// A PhysicsFS IO handle.
    /// Incapsulates the function pointers, which are then passed to PhysicsFS.
    /// </summary>
    /// <remarks>
    /// As you can see from the lack of meaningful fields, you should treat this
    /// as opaque data. Don't try to manipulate the IO handle, just pass the
    /// pointer you got, unmolested, to various PhysicsFS APIs.
    /// </remarks>
    public struct IoHandle
    {
        internal uint version;
        internal IntPtr opaque;
        internal IntPtr read;
        internal IntPtr write;
        internal IntPtr seek;
        internal IntPtr tell;
        internal IntPtr length;
        internal IntPtr duplicate;
        internal IntPtr flush;
        internal IntPtr destroy;
    }

    /// <summary>
    /// Native handle this IO points to.
    /// </summary>
    public readonly ref IoHandle Handle => ref _handle;

    private readonly ref IoHandle _handle;

    /// <summary>
    /// Binary compatibility information.
    /// </summary>
    /// <remarks>
    /// This must be set to zero at this time. Future versions of this
    /// struct will increment this field, so we know what a given
    /// implementation supports. We'll presumably keep supporting older
    /// versions as we offer new features, though.
    /// </remarks>
    public uint Version
    {
        get => _handle.version;
        set => _handle.version = value;
    }

    /// <summary>
    /// Read more data.
    /// </summary>
    /// <remarks>
    /// You don't have to implement this; set it to <see langword="null"/>
    /// if not implemented.
    /// This will only be used if the file is opened for reading. If set to
    /// <see langword="null"/>, a default implementation that immediately
    /// reports failure will be used.<br/><br/>
    /// See <see cref="PhysFsIoRead"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoRead? Read
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoRead>(_handle.read);
        set
        {
            IntPtr f = value == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            _handle.read = f;
        }
    }

    /// <summary>
    /// Write more data.
    /// </summary>
    /// <remarks>
    /// You don't have to implement this; set it to <see langword="null"/>
    /// if not implemented.
    /// This will only be used if the file is opened for writing. If set to
    /// <see langword="null"/>, a default implementation that immediately
    /// reports failure will be used.<br/><br/>
    /// See <see cref="PhysFsIoWrite"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoWrite Write
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoWrite>(_handle.write);
        set => _handle.write = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Move i/o position to a given byte offset from start.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsIoSeek"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoSeek Seek
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoSeek>(_handle.seek);
        set => _handle.seek = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Report current i/o position.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsIoTell"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoTell Tell
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoTell>(_handle.tell);
        set => _handle.tell = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Determine size of the i/o instance's dataset.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsIoLength"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoLength Length
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoLength>(_handle.length);
        set => _handle.length = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Duplicate this i/o instance.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsIoDuplicate"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoDuplicate Duplicate
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoDuplicate>(_handle.duplicate);
        set => _handle.duplicate = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Flush resources to media, or wherever.
    /// </summary>
    /// <remarks>
    /// This method can be <see langword="null"/> if flushing isn't necessary.<br/><br/>
    /// See <see cref="PhysFsIoFlush"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoFlush Flush
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoFlush>(_handle.flush);
        set => _handle.flush = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Cleanup and deallocate i/o instance.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsIoDestroy"/> for additional information.
    /// </remarks>
    public readonly PhysFsIoDestroy Destroy
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsIoDestroy>(_handle.destroy);
        set => _handle.destroy = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Creates a friendly wrapper for a PhysicsFS IO.
    /// </summary>
    /// <param name="io">
    /// Holds native function pointers. Treat this as opaque data.
    /// </param>
    public PhysFsIo(ref IoHandle io)
    {
        _handle = ref io;
    }
}
