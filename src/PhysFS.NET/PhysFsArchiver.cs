using static Icculus.PhysFS.NET.PhysFsIo;

namespace Icculus.PhysFS.NET;

/// <summary>
/// Open an archive provided by <paramref name="io"/>.
/// </summary>
/// <remarks>
/// This is where resources are allocated and data is parsed when mounting
/// an archive.
/// </remarks>
/// <param name="name">
/// Filename associated with <paramref name="io"/>, but doesn't
/// necessarily map to anything, let alone a real filename.
/// This possibly-meaningless name is in platform-dependent notation.
/// </param>
/// <param name="forWrite">
/// is <see langword="true"/> if this is to be used for
/// the write directory, and <see langword="false"/> if this is to be used
/// for an element of the search path.
/// </param>
/// <param name="claimed">
/// should be set to <see langword="true"/> if this is definitely an archive your
/// archiver implementation can handle, even if it fails. We use to
/// decide if we should stop trying other archivers if you fail to open
/// it. For example: the .zip archiver will set this to <see langword="true"/>
/// for something that's got a .zip file signature, even if it failed because
/// the file was also truncated. No sense in trying other archivers here, we
/// already tried to handle it with the appropriate implementation!.
/// </param>
/// <returns>
/// Return <see cref="nint.Zero"/> on failure and set <paramref name="claimed"/>
/// appropriately. If no archiver opened the archive or set <paramref name="claimed"/>,
/// <see cref="PhysicsFS.Mount"/> will report
/// <see cref="PhysFsErrorCode.PHYSFS_ERR_UNSUPPORTED"/>. Otherwise, it will report
/// the error from the archiver that claimed the data through <paramref name="claimed"/>
/// Return non-<see cref="nint.Zero"/> on success. The pointer returned will be
/// passed as the "opaque" parameter for later calls.
/// </returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate IntPtr PhysFsArchiverOpenArchive(ref IoHandle io,
    string name, bool forWrite, out bool claimed);

/// <summary>
/// List all files in <paramref name="dirname"/>.
/// </summary>
/// <remarks>
/// Each file is passed to <paramref name="cb"/>, where a copy is made if
/// appropriate, so you can dispose of it upon return from the callback.
/// <paramref name="dirname"/> is in platform-independent notation.
/// If you have a failure, call <see cref="PhysicsFS.SetErrorCode"/> with
/// whatever code seem appropriate and return
/// <see cref="PhysFsEnumerateCallbackResult.Error"/>.
/// If the callback returns <see cref="PhysFsEnumerateCallbackResult.Error"/>,
/// please call <see cref="PhysicsFS.SetErrorCode"/> with
/// <see cref="PhysFsErrorCode.PHYSFS_ERR_APP_CALLBACK"/> and then return
/// <see cref="PhysFsEnumerateCallbackResult.Error"/> as well.
/// Don't call the callback again in any circumstances.
/// If the callback returns <see cref="PhysFsEnumerateCallbackResult.Stop"/>,
/// stop enumerating and return <see cref="PhysFsEnumerateCallbackResult.Stop"/>
/// as well. Don't call the callback again in any
/// circumstances. Don't set an error code in this case.
/// Callbacks are only supposed to return a value from
/// <see cref="PhysFsEnumerateCallbackResult"/>. Any other result has undefined
/// behavior.
/// As long as the callback returned <see cref="PhysFsEnumerateCallbackResult.OK"/>
/// and you haven't experienced any errors of your own, keep enumerating until you're done
/// and then return <see cref="PhysFsEnumerateCallbackResult.OK"/> without
/// setting an error code.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate PhysFsEnumerateCallbackResult PhysFsArchiverEnumerate(IntPtr opaque,
    string dirname, PhysFsEnumerateCallback<object> cb, string origdir, IntPtr callbackdata);

/// <summary>
/// Open a file in this archive.
/// </summary>
/// <remarks>
/// This filename, <paramref name="fnm"/>, is in platform-independent notation.<br/><br/>
/// Returns <see cref="NullableRef&lt;IoHandle&gt;.Null"/> on failure,
/// and calls <see cref="PhysicsFS.SetErrorCode"/>.
/// Returns non-<see cref="NullableRef&lt;IoHandle&gt;.Null"/> on success.
/// The pointer returned will be passed as the "opaque" parameter for later file calls.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate NullableRef<IoHandle> PhysFsArchiverOpenFile(IntPtr opaque, string fnm);

/// <summary>
/// Modify the archive.
/// </summary>
/// <remarks>
/// If the archive is read-only, this operation should fail.
/// Return <see langword="true"/> on success, <see langword="false"/> on failure.
/// This filename is in platform-independent notation.
/// On failure, call <see cref="PhysicsFS.SetErrorCode"/>.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool PhysFsArchiverModify(IntPtr opaque, string fnm);

/// <summary>
/// Obtain basic file metadata.
/// </summary>
/// <remarks>
/// On success, fill in all the fields in <paramref name="stat"/>, using
/// reasonable defaults for fields that apply to your archive.<br/><br/>
/// Returns <see langword="true"/> on success, <see langword="false"/> on failure.
/// This filename is in platform-independent notation.
/// On failure, call <see cref="PhysicsFS.SetErrorCode"/>.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate bool PhysFsArchiverStat(IntPtr opaque, string fn, IntPtr stat);

/// <summary>
/// Destruct a previously-opened archive.
/// </summary>
/// <remarks>
/// Close this archive, and free any associated memory,
/// including the original <see cref="PhysFsIo"/> and
/// <paramref name="opaque"/> itself, if
/// applicable. Implementation can assume that it won't be called if
/// there are still files open from this archive.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PhysFsArchiverCloseArchive(IntPtr opaque);

/// <summary>
/// Abstract interface to provide support for user-defined archives.
/// </summary>
/// <remarks>
/// This is advanced, hardcore stuff. You don't need this unless you
/// really know what you're doing. Most apps will not need this.<br/><br/>
/// Historically, PhysicsFS provided a means to mount various archive file
/// formats, and physical directories in the native filesystem. However,
/// applications have been limited to the file formats provided by the
/// library. This interface allows an application to provide their own
/// archive file types.<br/><br/>
/// Conceptually, a <see cref="PhysFsArchiver"/> provides directory entries, while
/// <see cref="PhysFsIo"/> provides data streams for those directory entries. The most
/// obvious use of <see cref="PhysFsArchiver"/> is to provide support for an archive
/// file type that isn't provided by PhysicsFS directly: perhaps some
/// proprietary format that only your application needs to understand.<br/><br/>
/// Internally, all the built-in archive support uses this interface, so the
/// best examples for building a <see cref="PhysFsArchiver"/> is the source code to
/// PhysicsFS itself.<br/><br/>
/// An archiver is added to the system with <see cref="PhysicsFS.RegisterArchiver"/>,
/// and then it will be available for use automatically with <see cref="PhysicsFS.Mount"/>;
/// if a given archive can be handled with your archiver, it will be given control
/// as appropriate.<br/><br/>
/// These methods deal with dir handles. You have one instance of your
/// archiver, and it generates a unique, opaque handle for each opened
/// archive in its <see cref="OpenArchive"/> method. Since the lifetime
/// of an Archiver (not an archive) is generally the entire lifetime of the process, and it's
/// assumed to be a singleton, we do not provide any instance data for the
/// archiver itself; the app can just use some static variables if necessary.<br/><br/>
/// Symlinks should always be followed (except in <see cref="Stat"/>); PhysicsFS will
/// use the <see cref="Stat"/> method to check for symlinks and make a judgement on
/// whether to continue to call other methods based on that.<br/><br/>
/// Archivers, when necessary, should set the PhysicsFS error state with
/// <see cref="PhysicsFS.SetErrorCode"/> before returning. PhysicsFS will pass these errors
/// back to the application unmolested in most cases.<br/><br/>
/// Thread safety: <see cref="PhysFsArchiver"/> implementations are not guaranteed to be
/// thread safe in themselves. PhysicsFS provides thread safety when it calls
/// into a given archiver inside the library, but it does not promise that
/// using the same <see cref="PhysFsFileHandle"/> from two threads at once is thread-safe; as
/// such, your <see cref="PhysFsArchiver"/> can assume that locking is handled for you
/// so long as the <see cref="PhysFsIo"/> you return from <see cref="PhysicsFS.OpenRead"/>
/// doesn't change any of your Archiver state, as the <see cref="PhysFsIo"/> won't be
/// as aggressively protected.<br/><br/><br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.RegisterArchiver"/><br/>
/// <seealso cref="PhysicsFS.DeregisterArchiver"/><br/>
/// <seealso cref="PhysicsFS.SupportedArchiveTypes"/>
/// </remarks>
public readonly ref struct PhysFsArchiver
{
    /// <summary>
    /// A PhysicsFS archiver handle.
    /// Incapsulates the function pointers, which are then passed to PhysicsFS.
    /// </summary>
    /// <remarks>
    /// As you can see from the lack of meaningful fields, you should treat this
    /// as opaque data. Don't try to manipulate the archiver handle, just pass the
    /// pointer you got, unmolested, to various PhysicsFS APIs.
    /// </remarks>
    public struct ArchiverHandle
    {
        internal uint version;
        internal PHYSFS_ArchiveInfo info;
        internal IntPtr openArchive;
        internal IntPtr enumerate;
        internal IntPtr openRead;
        internal IntPtr openWrite;
        internal IntPtr openAppend;
        internal IntPtr remove;
        internal IntPtr mkdir;
        internal IntPtr stat;
        internal IntPtr closeArchive;
    }

    /// <summary>
    /// Native handle this archiver points to.
    /// </summary>
    public readonly ref ArchiverHandle Handle => ref _handle;

    private readonly ref ArchiverHandle _handle;

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
    /// Basic info about this archiver.
    /// </summary>
    /// <remarks>
    /// This is used to identify your archive, and is returned in
    /// <see cref="PhysicsFS.SupportedArchiveTypes"/>.
    /// </remarks>
    public readonly PhysFsArchiveInfo Info
    {
        get => PhysFsArchiveInfo.FromNative(_handle.info);
        set => _handle.info = value.ToNative();
    }

    /// <summary>
    /// Open an archive provided by <see cref="PhysFsIo"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsArchiverOpenArchive"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverOpenArchive OpenArchive
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverOpenArchive>(_handle.openArchive);
        set => _handle.openArchive = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// List all files in a directory.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsArchiverEnumerate"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverEnumerate Enumerate
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverEnumerate>(_handle.enumerate);
        set => _handle.enumerate = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Open a file in this archive for reading.
    /// </summary>
    /// <remarks>
    /// Fail if the file does not exist.<br/><br/>
    /// See <see cref="PhysFsArchiverOpenFile"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverOpenFile OpenRead
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverOpenFile>(_handle.openRead);
        set => _handle.openRead = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Open a file in this archive for writing.
    /// </summary>
    /// <remarks>
    /// If the file does not exist, it should be created. If it exists,
    /// it should be truncated to zero bytes. The writing offset should
    /// be the start of the file.
    /// If the archive is read-only, this operation should fail.<br/><br/>
    /// See <see cref="PhysFsArchiverOpenFile"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverOpenFile OpenWrite
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverOpenFile>(_handle.openWrite);
        set => _handle.openWrite = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Open a file in this archive for appending.
    /// </summary>
    /// <remarks>
    /// If the file does not exist, it should be created. The writing
    /// offset should be the end of the file.
    /// If the archive is read-only, this operation should fail.<br/><br/>
    /// See <see cref="PhysFsArchiverOpenFile"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverOpenFile OpenAppend
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverOpenFile>(_handle.openAppend);
        set => _handle.openAppend = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Create a directory in the archive.
    /// </summary>
    /// <remarks>
    /// If the application is trying to make multiple dirs, PhysicsFS
    /// will split them up into multiple calls before passing them to
    /// your driver.<br/><br/>
    /// See <see cref="PhysFsArchiverModify"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverModify Remove
    {
        get => Marshal.GetDelegateForFunctionPointer< PhysFsArchiverModify>(_handle.remove);
        set => _handle.remove = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Create a directory in the archive.
    /// </summary>
    /// <remarks>
    /// If the application is trying to make multiple dirs, PhysicsFS
    /// will split them up into multiple calls before passing them to
    /// your driver.<br/><br/>
    /// See <see cref="PhysFsArchiverModify"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverModify Mkdir
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverModify>(_handle.mkdir);
        set => _handle.mkdir = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Obtain basic file metadata.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsArchiverStat"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverStat Stat
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverStat>(_handle.stat);
        set => _handle.stat = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Destruct a previously-opened archive.
    /// </summary>
    /// <remarks>
    /// See <see cref="PhysFsArchiverCloseArchive"/> for additional
    /// information.
    /// </remarks>
    public readonly PhysFsArchiverCloseArchive CloseArchive
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsArchiverCloseArchive>(_handle.closeArchive);
        set => _handle.closeArchive = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Creates a friendly wrapper for a PhysicsFS archiver.
    /// </summary>
    /// <param name="archiver">
    /// Holds native function pointers. Treat this as opaque data.
    /// </param>
    public PhysFsArchiver(ref ArchiverHandle archiver)
    {
        _handle = ref archiver;
    }
}
