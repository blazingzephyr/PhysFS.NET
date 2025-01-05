
namespace Icculus.PhysFS.NET.Internals;

public static unsafe partial class physfs
{
    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial void PHYSFS_getLinkedVersion(out PHYSFS_Version ver);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_init(string? argv0);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_deinit();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial IntPtr PHYSFS_supportedArchiveTypes();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial void PHYSFS_freeList(IntPtr listVar);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getDirSeparator();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial void PHYSFS_permitSymbolicLinks([MarshalAs(UnmanagedType.I1)] bool allow);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial IntPtr PHYSFS_getCdRomDirs();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getBaseDir();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getWriteDir();

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_setWriteDir(string newDir);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial IntPtr PHYSFS_getSearchPath();

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_setSaneConfig(
        string organization,
        string appName,
        string? archiveExt,
        [MarshalAs(UnmanagedType.I1)] bool includeCdRoms,
        [MarshalAs(UnmanagedType.I1)] bool archivesFirst
    );

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_mkdir(string dirName);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_delete(string filename);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getRealDir(string filename);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial IntPtr PHYSFS_enumerateFiles(string dir);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_exists(string fname);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial FileHandle PHYSFS_openWrite(string filename);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial FileHandle PHYSFS_openAppend(string filename);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial FileHandle PHYSFS_openRead(string filename);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_close(FileHandle _handle);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_eof(FileHandle handle);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial long PHYSFS_tell(FileHandle handle);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_seek(FileHandle handle, ulong pos);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial ulong PHYSFS_fileLength(FileHandle handle);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_setBuffer(FileHandle handle, ulong bufsize);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_flush(FileHandle handle);

    //

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_isInit();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_symbolicLinksPermitted();

    //PHYSFS_DECL int PHYSFS_setAllocator(const PHYSFS_Allocator *allocator);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_mount(
        string newDir,
        string mountPoint,
        [MarshalAs(UnmanagedType.I1)] bool appendToPath
    );

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string? PHYSFS_getMountPoint(string dir);

    //

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_enumerate(
        string fn,
        PHYSFS_EnumerateCallback cb,
        IntPtr data
    );

    [LibraryImport("physfs.dll", StringMarshallingCustomType = typeof(Utf8StringMarshallerSkipFree))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_unmount(string oldDir);



    //PHYSFS_DECL PHYSFS_sint16 PHYSFS_swapSLE16(PHYSFS_sint16 val);
    //PHYSFS_DECL PHYSFS_uint16 PHYSFS_swapULE16(PHYSFS_uint16 val);
    //PHYSFS_DECL PHYSFS_sint32 PHYSFS_swapSLE32(PHYSFS_sint32 val);
    //PHYSFS_DECL PHYSFS_uint32 PHYSFS_swapULE32(PHYSFS_uint32 val);
    //PHYSFS_DECL PHYSFS_sint64 PHYSFS_swapSLE64(PHYSFS_sint64 val);
    //PHYSFS_DECL PHYSFS_uint64 PHYSFS_swapULE64(PHYSFS_uint64 val);
    //PHYSFS_DECL PHYSFS_sint16 PHYSFS_swapSBE16(PHYSFS_sint16 val);
    //PHYSFS_DECL PHYSFS_uint16 PHYSFS_swapUBE16(PHYSFS_uint16 val);
    //PHYSFS_DECL PHYSFS_sint32 PHYSFS_swapSBE32(PHYSFS_sint32 val);
    //PHYSFS_DECL PHYSFS_uint32 PHYSFS_swapUBE32(PHYSFS_uint32 val);
    //PHYSFS_DECL PHYSFS_sint64 PHYSFS_swapSBE64(PHYSFS_sint64 val);
    //PHYSFS_DECL PHYSFS_uint64 PHYSFS_swapUBE64(PHYSFS_uint64 val);
    //PHYSFS_DECL int PHYSFS_readSLE16(PHYSFS_File *file, PHYSFS_sint16 *val);
    //PHYSFS_DECL int PHYSFS_readULE16(PHYSFS_File *file, PHYSFS_uint16 *val);
    //PHYSFS_DECL int PHYSFS_readSBE16(PHYSFS_File *file, PHYSFS_sint16 *val);
    //PHYSFS_DECL int PHYSFS_readUBE16(PHYSFS_File *file, PHYSFS_uint16 *val);
    //PHYSFS_DECL int PHYSFS_readSLE32(PHYSFS_File *file, PHYSFS_sint32 *val);
    //PHYSFS_DECL int PHYSFS_readULE32(PHYSFS_File *file, PHYSFS_uint32 *val);
    //PHYSFS_DECL int PHYSFS_readSBE32(PHYSFS_File *file, PHYSFS_sint32 *val);
    //PHYSFS_DECL int PHYSFS_readUBE32(PHYSFS_File *file, PHYSFS_uint32 *val);
    //PHYSFS_DECL int PHYSFS_readSLE64(PHYSFS_File *file, PHYSFS_sint64 *val);
    //PHYSFS_DECL int PHYSFS_readULE64(PHYSFS_File *file, PHYSFS_uint64 *val);
    //PHYSFS_DECL int PHYSFS_readSBE64(PHYSFS_File *file, PHYSFS_sint64 *val);
    //PHYSFS_DECL int PHYSFS_readUBE64(PHYSFS_File *file, PHYSFS_uint64 *val);
    //PHYSFS_DECL int PHYSFS_writeSLE16(PHYSFS_File *file, PHYSFS_sint16 val);
    //PHYSFS_DECL int PHYSFS_writeULE16(PHYSFS_File *file, PHYSFS_uint16 val);
    //PHYSFS_DECL int PHYSFS_writeSBE16(PHYSFS_File *file, PHYSFS_sint16 val);
    //PHYSFS_DECL int PHYSFS_writeUBE16(PHYSFS_File *file, PHYSFS_uint16 val);
    //PHYSFS_DECL int PHYSFS_writeSLE32(PHYSFS_File *file, PHYSFS_sint32 val);
    //PHYSFS_DECL int PHYSFS_writeULE32(PHYSFS_File *file, PHYSFS_uint32 val);
    //PHYSFS_DECL int PHYSFS_writeSBE32(PHYSFS_File *file, PHYSFS_sint32 val);
    //PHYSFS_DECL int PHYSFS_writeUBE32(PHYSFS_File *file, PHYSFS_uint32 val);
    //PHYSFS_DECL int PHYSFS_writeSLE64(PHYSFS_File *file, PHYSFS_sint64 val);
    //PHYSFS_DECL int PHYSFS_writeULE64(PHYSFS_File *file, PHYSFS_uint64 val);
    //PHYSFS_DECL int PHYSFS_writeSBE64(PHYSFS_File *file, PHYSFS_sint64 val);
    //PHYSFS_DECL int PHYSFS_writeUBE64(PHYSFS_File *file, PHYSFS_uint64 val);
    //PHYSFS_DECL int PHYSFS_setAllocator(const PHYSFS_Allocator *allocator);

    //PHYSFS_DECL void PHYSFS_getCdRomDirsCallback(PHYSFS_StringCallback c, void *d);
    //PHYSFS_DECL void PHYSFS_getSearchPathCallback(PHYSFS_StringCallback c, void *d);
    //PHYSFS_DECL void PHYSFS_utf8FromUcs4(const PHYSFS_uint32 *src, char *dst,
    //PHYSFS_DECL void PHYSFS_utf8ToUcs4(const char *src, PHYSFS_uint32 *dst,
    //PHYSFS_DECL void PHYSFS_utf8FromUcs2(const PHYSFS_uint16 *src, char *dst,
    //PHYSFS_DECL void PHYSFS_utf8ToUcs2(const char *src, PHYSFS_uint16 *dst,
    //PHYSFS_DECL void PHYSFS_utf8FromLatin1(const char *src, char *dst,
    //PHYSFS_DECL int PHYSFS_caseFold(const PHYSFS_uint32 from, PHYSFS_uint32 *to);
    //PHYSFS_DECL int PHYSFS_utf8stricmp(const char *str1, const char *str2);
    //PHYSFS_DECL int PHYSFS_utf16stricmp(const PHYSFS_uint16 *str1,
    //PHYSFS_DECL int PHYSFS_ucs4stricmp(const PHYSFS_uint32 *str1,
    //PHYSFS_DECL int PHYSFS_enumerate(const char *dir, PHYSFS_EnumerateCallback c,
    //PHYSFS_DECL int PHYSFS_unmount(const char *oldDir);
    //PHYSFS_DECL const PHYSFS_Allocator *PHYSFS_getAllocator(void);
    //PHYSFS_DECL int PHYSFS_stat(const char *fname, PHYSFS_Stat *stat);
    //PHYSFS_DECL void PHYSFS_utf8FromUtf16(const PHYSFS_uint16 *src, char *dst,
    //PHYSFS_DECL void PHYSFS_utf8ToUtf16(const char *src, PHYSFS_uint16 *dst,
    //PHYSFS_DECL PHYSFS_sint64 PHYSFS_readBytes(PHYSFS_File *handle, void *buffer,
    //PHYSFS_DECL PHYSFS_sint64 PHYSFS_writeBytes(PHYSFS_File *handle,
    //PHYSFS_DECL int PHYSFS_mountIo(PHYSFS_Io *io, const char *newDir,
    //PHYSFS_DECL int PHYSFS_mountMemory(const void *buf, PHYSFS_uint64 len,
    //PHYSFS_DECL int PHYSFS_mountHandle(PHYSFS_File *file, const char *newDir,
    //PHYSFS_DECL PHYSFS_ErrorCode PHYSFS_getLastErrorCode(void);
    //PHYSFS_DECL const char *PHYSFS_getErrorByCode(PHYSFS_ErrorCode code);
    //PHYSFS_DECL void PHYSFS_setErrorCode(PHYSFS_ErrorCode code);
    //PHYSFS_DECL const char *PHYSFS_getPrefDir(const char *org, const char *app);
    //PHYSFS_DECL int PHYSFS_registerArchiver(const PHYSFS_Archiver *archiver);
    //PHYSFS_DECL int PHYSFS_deregisterArchiver(const char *ext);
    //PHYSFS_DECL int PHYSFS_setRoot(const char *archive, const char *subdir);
    // 









    //









    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial PHYSFS_ErrorCode PHYSFS_getLastErrorCode();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getErrorByCode(PHYSFS_ErrorCode code);


    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getPrefDir(string org, string app);







    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_stat(string fname, out PHYSFS_Stat stat);



    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial long PHYSFS_readBytes(FileHandle handle, IntPtr buffer, ulong _len);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial long PHYSFS_writeBytes(FileHandle handle, IntPtr buffer, ulong _len);

}
