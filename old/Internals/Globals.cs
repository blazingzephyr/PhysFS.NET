
namespace Old.Icculus.PhysFS.NET.Internals;

//public static unsafe partial class physfs
//{
//    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Interop.LibraryImportGenerator", "8.0.12.6609")]
//    [global::System.Runtime.CompilerServices.SkipLocalsInitAttribute]
//    public static partial void PHYSFS_init()
//    {
//        nint __retVal_native;
//        {
//            __retVal_native = __PInvoke();
//        }

//        if (__retVal_native == 0)
//        {
//            throw new Exception();
//        }

//        // Local P/Invoke
//        [global::System.Runtime.InteropServices.DllImportAttribute("physfs.dll", EntryPoint = "PHYSFS_init", ExactSpelling = true)]
//        [global::System.Runtime.InteropServices.UnmanagedCallConvAttribute(CallConvs = new global::System.Type[] { typeof(global::System.Runtime.CompilerServices.CallConvStdcall) })]
//        static extern unsafe nint __PInvoke();
//    }
//}

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
    [return: MarshalUsing(typeof(VoidMarshaller))]
    public static partial bool PHYSFS_deinit();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial IntPtr PHYSFS_supportedArchiveTypes();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial void PHYSFS_freeList(IntPtr listVar);

    // Not imported:
    // PHYSFS_DECL const char *PHYSFS_getLastError(void) PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_getLastErrorCode and PHYSFS_getErrorByCode.

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

    // Not imported.
    // PHYSFS_DECL const char *PHYSFS_getUserDir(void) PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_getPrefDir.

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getWriteDir();

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_setWriteDir(string newDir);

    // Not imported.
    // PHYSFS_DECL int PHYSFS_addToSearchPath(const char *newDir, int appendToPath)
    //                                         PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_mount.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_removeFromSearchPath(const char* oldDir)
    //                                        PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_unmount.

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

    // Not imported.
    // PHYSFS_DECL int PHYSFS_isDirectory(const char *fname) PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_stat.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_isSymbolicLink(const char *fname) PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_stat.

    // Not imported.
    // PHYSFS_DECL PHYSFS_sint64 PHYSFS_getLastModTime(const char *filename)
    //                                                 PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_stat.

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

    // Not imported.
    // PHYSFS_DECL PHYSFS_sint64 PHYSFS_read(PHYSFS_File* handle,
    //                                   void* buffer,
    //                                   PHYSFS_uint32 objSize,
    //                                   PHYSFS_uint32 objCount)
    //                                     PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_readBytes.

    // Not imported.
    // PHYSFS_DECL PHYSFS_sint64 PHYSFS_write(PHYSFS_File* handle,
    //                                    const void* buffer,
    //                                    PHYSFS_uint32 objSize,
    //                                    PHYSFS_uint32 objCount)
    //                                     PHYSFS_DEPRECATED;
    // Will not be supported. Use PHYSFS_writeBytes.

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

    // Not imported.
    // PHYSFS_DECL PHYSFS_sint16 PHYSFS_swapSLE16(PHYSFS_sint16 val);
    // PHYSFS_DECL PHYSFS_uint16 PHYSFS_swapULE16(PHYSFS_uint16 val);
    // PHYSFS_DECL PHYSFS_sint32 PHYSFS_swapSLE32(PHYSFS_sint32 val);
    // PHYSFS_DECL PHYSFS_uint32 PHYSFS_swapULE32(PHYSFS_uint32 val);
    // PHYSFS_DECL PHYSFS_sint64 PHYSFS_swapSLE64(PHYSFS_sint64 val);
    // PHYSFS_DECL PHYSFS_uint64 PHYSFS_swapULE64(PHYSFS_uint64 val);
    // PHYSFS_DECL PHYSFS_sint16 PHYSFS_swapSBE16(PHYSFS_sint16 val);
    // PHYSFS_DECL PHYSFS_uint16 PHYSFS_swapUBE16(PHYSFS_uint16 val);
    // PHYSFS_DECL PHYSFS_sint32 PHYSFS_swapSBE32(PHYSFS_sint32 val);
    // PHYSFS_DECL PHYSFS_uint32 PHYSFS_swapUBE32(PHYSFS_uint32 val);
    // PHYSFS_DECL PHYSFS_sint64 PHYSFS_swapSBE64(PHYSFS_sint64 val);
    // PHYSFS_DECL PHYSFS_uint64 PHYSFS_swapUBE64(PHYSFS_uint64 val);
    // Under consideration / TBA.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_readSLE16(PHYSFS_File* file, PHYSFS_sint16* val);
    // PHYSFS_DECL int PHYSFS_readULE16(PHYSFS_File* file, PHYSFS_uint16* val);
    // PHYSFS_DECL int PHYSFS_readSBE16(PHYSFS_File* file, PHYSFS_sint16* val);
    // PHYSFS_DECL int PHYSFS_readUBE16(PHYSFS_File* file, PHYSFS_uint16* val);
    // PHYSFS_DECL int PHYSFS_readSLE32(PHYSFS_File* file, PHYSFS_sint32* val);
    // PHYSFS_DECL int PHYSFS_readULE32(PHYSFS_File* file, PHYSFS_uint32* val);
    // PHYSFS_DECL int PHYSFS_readSBE32(PHYSFS_File* file, PHYSFS_sint32* val);
    // PHYSFS_DECL int PHYSFS_readUBE32(PHYSFS_File* file, PHYSFS_uint32* val);
    // PHYSFS_DECL int PHYSFS_readSLE64(PHYSFS_File* file, PHYSFS_sint64* val);
    // PHYSFS_DECL int PHYSFS_readULE64(PHYSFS_File* file, PHYSFS_uint64* val);
    // PHYSFS_DECL int PHYSFS_readSBE64(PHYSFS_File* file, PHYSFS_sint64* val);
    // PHYSFS_DECL int PHYSFS_readUBE64(PHYSFS_File* file, PHYSFS_uint64* val);
    // PHYSFS_DECL int PHYSFS_writeSLE16(PHYSFS_File* file, PHYSFS_sint16 val);
    // PHYSFS_DECL int PHYSFS_writeULE16(PHYSFS_File* file, PHYSFS_uint16 val);
    // PHYSFS_DECL int PHYSFS_writeSBE16(PHYSFS_File* file, PHYSFS_sint16 val);
    // PHYSFS_DECL int PHYSFS_writeUBE16(PHYSFS_File* file, PHYSFS_uint16 val);
    // PHYSFS_DECL int PHYSFS_writeSLE32(PHYSFS_File* file, PHYSFS_sint32 val);
    // PHYSFS_DECL int PHYSFS_writeULE32(PHYSFS_File* file, PHYSFS_uint32 val);
    // PHYSFS_DECL int PHYSFS_writeSBE32(PHYSFS_File* file, PHYSFS_sint32 val);
    // PHYSFS_DECL int PHYSFS_writeUBE32(PHYSFS_File* file, PHYSFS_uint32 val);
    // PHYSFS_DECL int PHYSFS_writeSLE64(PHYSFS_File* file, PHYSFS_sint64 val);
    // PHYSFS_DECL int PHYSFS_writeULE64(PHYSFS_File* file, PHYSFS_uint64 val);
    // PHYSFS_DECL int PHYSFS_writeSBE64(PHYSFS_File* file, PHYSFS_sint64 val);
    // PHYSFS_DECL int PHYSFS_writeUBE64(PHYSFS_File* file, PHYSFS_uint64 val);
    // Under consideration / TBA.

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_isInit();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_symbolicLinksPermitted();

    // Not imported.
    // PHYSFS_DECL int PHYSFS_setAllocator(const PHYSFS_Allocator *allocator);
    // Under consideration / TBA.

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

    // Not imported.
    // PHYSFS_DECL void PHYSFS_getCdRomDirsCallback(PHYSFS_StringCallback c, void *d);
    // Under consideration / TBA.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_getSearchPathCallback(PHYSFS_StringCallback c, void *d);
    // Under consideration / TBA.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_enumerateFilesCallback(const char *dir,
    //                                            PHYSFS_EnumFilesCallback c,
    //                                            void *d) PHYSFS_DEPRECATED;
    //  Will not be supported. Use PHYSFS_enumerate.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_utf8FromUcs4(const PHYSFS_uint32 *src, char *dst,
    //                                      PHYSFS_uint64 len);
    // Just use standard C# API.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_utf8ToUcs4(const char *src, PHYSFS_uint32 *dst,
    //                                PHYSFS_uint64 len);
    // Just use standard C# API.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_utf8FromUcs2(const PHYSFS_uint16 *src, char *dst,
    //                                  PHYSFS_uint64 len);
    // Just use standard C# API.

    // Not imported.
    // PHYSFS_DECL void PHYSFS_utf8ToUcs2(const char *src, PHYSFS_uint16 *dst,
    //                                    PHYSFS_uint64 len);
    //
    // byte[] ba = Encoding.UTF8.GetBytes(strMessage);
    // String strHex = BitConverter.ToString(ba);
    // strHex = strHex.Replace("-", "");

    // Not imported.
    // PHYSFS_DECL void PHYSFS_utf8FromLatin1(const char *src, char *dst,
    //                                    PHYSFS_uint64 len);
    // Use Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").getBytes(s)) instead.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_caseFold(const PHYSFS_uint32 from, PHYSFS_uint32 *to);
    // Use String.ToLower() instead.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_utf8stricmp(const char *str1, const char *str2);
    // Use String.Equals(a, b, StringComparison.CurrentCultureIgnoreCase) instead.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_utf16stricmp(const PHYSFS_uint16 *str1,
    //                                     const PHYSFS_uint16 *str2);
    // Use String.Equals(a, b, StringComparison.CurrentCultureIgnoreCase) instead.

    // Not imported.
    // PHYSFS_DECL int PHYSFS_ucs4stricmp(const PHYSFS_uint32* str1,
    //                                const PHYSFS_uint32* str2);
    // Use String.Equals(a, b, StringComparison.CurrentCultureIgnoreCase) instead.

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_enumerate(
        string fn,
        PHYSFS_EnumerateCallback cb,
        IntPtr data
    );

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_unmount(string oldDir);

    // Not imported:
    // PHYSFS_DECL const PHYSFS_Allocator *PHYSFS_getAllocator(void);
    // Under consideration / TBA.

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_stat(string fname, out PHYSFS_Stat stat);

    // Not imported:
    // PHYSFS_DECL void PHYSFS_utf8FromUtf16(const PHYSFS_uint16 *src, char *dst,
    // Just use Utf8.FromUtf16 in System.Text.Unicode.

    // Not imported:
    // PHYSFS_DECL void PHYSFS_utf8ToUtf16(const char *src, PHYSFS_uint16 *dst,
    // Just use Utf8.ToUtf16 in System.Text.Unicode.

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial long PHYSFS_readBytes(FileHandle handle, IntPtr buffer, ulong _len);

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial long PHYSFS_writeBytes(FileHandle handle, IntPtr buffer, ulong _len);

    // Not imported:
    // PHYSFS_DECL int PHYSFS_mountIo(PHYSFS_Io *io, const char *newDir,
    // Under consideration / TBA.

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_mountMemory(
        IntPtr buffer,
        ulong len,
        UnmountCallback? del,
        string newDir,
        string mountPoint,
        [MarshalAs(UnmanagedType.I1)] bool appendToPath
   );

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_mountHandle(
        FileHandle file,
        string newDir,
        string mountPoint,
        [MarshalAs(UnmanagedType.I1)] bool appendToPath
    );

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial PHYSFS_ErrorCode PHYSFS_getLastErrorCode();

    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getErrorByCode(PHYSFS_ErrorCode code);

    // Currently unused in the public API.
    [LibraryImport("physfs.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public static partial void PHYSFS_setErrorCode(PHYSFS_ErrorCode code);

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalUsing(typeof(Utf8StringMarshallerSkipFree))]
    public static partial string PHYSFS_getPrefDir(string org, string app);

    // Not imported:
    // PHYSFS_DECL int PHYSFS_registerArchiver(const PHYSFS_Archiver *archiver);
    // Under consideration / TBA.

    // Not imported:
    // PHYSFS_DECL int PHYSFS_deregisterArchiver(const char *ext);
    // Under consideration / TBA.

    [LibraryImport("physfs.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool PHYSFS_setRoot(string archive, string subdir);
}
