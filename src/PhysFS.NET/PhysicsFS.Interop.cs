
using System.Diagnostics;
using static Icculus.PhysFS.NET.PhysFsArchiver;
using static Icculus.PhysFS.NET.PhysFsIo;
using AllocatorHandle = Icculus.PhysFS.NET.PhysFsAllocator.AllocatorHandle;

#if ANDROID
using Android.Runtime;
#endif

namespace Icculus.PhysFS.NET;

public static partial class PhysicsFS
{
    public static partial void AddToSearchPath(string newDir, bool appendToPath)
    {
        unsafe
        {
            byte* newDir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            nint result = PHYSFS_addToSearchPath(newDir_native, appendToPath ? 1 : 0);
            Utf8StringMarshaller.Free(newDir_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_addToSearchPath", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_addToSearchPath(byte* newDir, nint appendToPath);
    }

    public static partial char[] CaseFold(char from)
    {
        unsafe
        {
            uint[] to = ['\0', '\0', '\0'];
            fixed (uint* to_native = to)
            {
                nint result = PHYSFS_caseFold(from, to_native);
                return Array.ConvertAll(to, i => (char)i);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_caseFold", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_caseFold(uint from, uint* to);
    }

    public static partial void Deinit()
    {
        nint result = PHYSFS_deinit();
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_deinit", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_deinit();
    }

    public static partial void Delete(string fileName)
    {
        unsafe
        {
            byte* filename_native = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            nint result = PHYSFS_delete(filename_native);
            Utf8StringMarshaller.Free(filename_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_delete", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_delete(byte* filename);
    }

    public static partial void DeregisterArchiver(string fileExt)
    {
        unsafe
        {
            byte* fileExt_native = Utf8StringMarshaller.ConvertToUnmanaged(fileExt);
            nint result = PHYSFS_deregisterArchiver(fileExt_native);
            Utf8StringMarshaller.Free(fileExt_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_deregisterArchiver", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_deregisterArchiver(byte* ext);
    }

    public static partial void Enumerate<T>(string directory, PhysFsEnumerateCallback<T> callback, ref T data)
    {
        unsafe
        {
            nint Callback(void* data, byte* origdir, byte* fname)
            {
                ref T data_managed = ref Unsafe.AsRef<T>(data);
                string parentDir = Utf8StringMarshaller.ConvertToManaged(origdir)!;
                string fileName = Utf8StringMarshaller.ConvertToManaged(fname)!;

                PhysFsEnumerateCallbackResult result = callback(ref data_managed, parentDir, fileName);
                return (nint)result;
            }

            void* data_native = Unsafe.AsPointer(ref data);
            nint result = EnumerateBase(directory, Callback, data_native);
            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }
    }

    public static partial void Enumerate(string directory, PhysFsEnumerateCallback callback)
    {
        unsafe
        {
            nint Callback(void* data, byte* origdir, byte* fname)
            {
                string parentDir = Utf8StringMarshaller.ConvertToManaged(origdir)!;
                string fileName = Utf8StringMarshaller.ConvertToManaged(fname)!;

                PhysFsEnumerateCallbackResult result = callback(parentDir, fileName);
                return (nint)result;
            }

            nint result = EnumerateBase(directory, Callback, null);
            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }
    }

    private static unsafe nint EnumerateBase(string directory, PHYSFS_EnumerateCallback callback, void* data)
    {
        byte* dir = Utf8StringMarshaller.ConvertToUnmanaged(directory);
        IntPtr cb = Marshal.GetFunctionPointerForDelegate(callback);
        nint result = PHYSFS_enumerate(dir, cb, data);
        Utf8StringMarshaller.Free(dir);
        return result;

        [DllImport("physfs", EntryPoint = "PHYSFS_enumerate", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_enumerate(byte* fn, IntPtr cb, void* data);
    }

    public static partial IEnumerable<string> EnumerateFiles(string directory)
    {
        IntPtr ptr = EnumerateFilesPtr(directory);
        IntPtr files = ptr;

        if (ptr == IntPtr.Zero)
        {
            PHYSFS_freeList(ptr);
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }
        else
        {
            while (true)
            {
                IntPtr file = Marshal.ReadIntPtr(files);
                if (file == IntPtr.Zero) break;

                yield return Marshal.PtrToStringUTF8(file)!;
                files += IntPtr.Size;
            }

            PHYSFS_freeList(ptr);
        }
    }

    private static IntPtr EnumerateFilesPtr(string directory)
    {
        unsafe
        {
            byte* dir = Utf8StringMarshaller.ConvertToUnmanaged(directory);
            IntPtr result = PHYSFS_enumerateFiles(dir);
            Utf8StringMarshaller.Free(dir);
            return result;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_enumerateFiles", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_enumerateFiles(byte* dir);
    }

    public static partial void EnumerateFilesCallback<T>(string directory, PhysFsEnumFilesCallback<T> callback, ref T data)
    {
        unsafe
        {
            void Callback(void* data, byte* origdir, byte* fname)
            {
                ref T data_managed = ref Unsafe.AsRef<T>(data);
                string parentDir = Utf8StringMarshaller.ConvertToManaged(origdir)!;
                string fileName = Utf8StringMarshaller.ConvertToManaged(fname)!;
                callback(ref data_managed, parentDir, fileName);
            }

            void* data_native = Unsafe.AsPointer(ref data);
            nint result = EnumerateFilesBase(directory, Callback, data_native);
            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }
    }

    public static partial void EnumerateFilesCallback(string directory, PhysFsEnumFilesCallback callback)
    {
        unsafe
        {
            void Callback(void* data, byte* origdir, byte* fname)
            {
                string parentDir = Utf8StringMarshaller.ConvertToManaged(origdir)!;
                string fileName = Utf8StringMarshaller.ConvertToManaged(fname)!;
                callback(parentDir, fileName);
            }

            nint result = EnumerateFilesBase(directory, Callback, null);
            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }
    }

    private static unsafe nint EnumerateFilesBase(string directory, PHYSFS_EnumFilesCallback callback, void* data)
    {
        byte* dir = Utf8StringMarshaller.ConvertToUnmanaged(directory);
        IntPtr cb = Marshal.GetFunctionPointerForDelegate(callback);
        nint result = PHYSFS_enumerateFilesCallback(dir, cb, data);
        Utf8StringMarshaller.Free(dir);
        return result;

        [DllImport("physfs", EntryPoint = "PHYSFS_enumerateFilesCallback", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_enumerateFilesCallback(byte* fn, IntPtr cb, void* data);
    }

    public static partial bool EOF(PhysFsFileHandle handle)
    {
        return PHYSFS_eof(handle.DangerousGetHandle()) != 0;

        [DllImport("physfs", EntryPoint = "PHYSFS_eof", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_eof(IntPtr handle);
    }

    public static partial bool Exists(string fileName)
    {
        unsafe
        {
            byte* fname = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            nint result = PHYSFS_exists(fname);
            Utf8StringMarshaller.Free(fname);
            return result != 0;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_exists", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_exists(byte* fname);
    }

    public static partial long FileLength(PhysFsFileHandle handle)
    {
        return PHYSFS_fileLength(handle.DangerousGetHandle());

        [DllImport("physfs", EntryPoint = "PHYSFS_fileLength", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_fileLength(IntPtr handle);
    }

    public static partial void Flush(PhysFsFileHandle handle)
    {
        if (PHYSFS_flush(handle.DangerousGetHandle()) == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_flush", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_flush(IntPtr handle);
    }

    public static partial NullableRef<AllocatorHandle> GetAllocator()
    {
        unsafe
        {
            AllocatorHandle* result = PHYSFS_getAllocator();
            if (result == null) return NullableRef<AllocatorHandle>.Null;
            else
            {
                ref AllocatorHandle allocator = ref Unsafe.AsRef<AllocatorHandle>(result);
                return new NullableRef<AllocatorHandle>(ref allocator);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getAllocator", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern AllocatorHandle* PHYSFS_getAllocator();
    }

    private static string? GetBaseDir()
    {
        unsafe
        {
            byte* result = PHYSFS_getBaseDir();
            return Utf8StringMarshaller.ConvertToManaged(result);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getBaseDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern byte* PHYSFS_getBaseDir();
    }

    public static partial IEnumerable<string> GetCdRomDirs()
    {
        IntPtr ptr = PHYSFS_getCdRomDirs();
        IntPtr dirs = ptr;

        if (dirs == IntPtr.Zero)
        {
            PHYSFS_freeList(ptr);
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }
        else
        {
            while (true)
            {
                IntPtr dir = Marshal.ReadIntPtr(dirs);
                if (dir == IntPtr.Zero) break;

                yield return Marshal.PtrToStringUTF8(dir)!;
                dirs += IntPtr.Size;
            }

            PHYSFS_freeList(ptr);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getCdRomDirs", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_getCdRomDirs();
    }

    public static partial void GetCdRomDirsCallback<T>(PhysFsStringCallback<T> callback, ref T data)
    {
        unsafe
        {
            void Callback(void* data, byte* str)
            {
                ref T data_managed = ref Unsafe.AsRef<T>(data);
                string str_managed = Utf8StringMarshaller.ConvertToManaged(str)!;
                callback(ref data_managed, str_managed);
            }

            void* data_native = Unsafe.AsPointer(ref data);
            GetCdRomDirsCallbackBase(Callback, data_native);
        }
    }

    public static partial void GetCdRomDirsCallback(PhysFsStringCallback callback)
    {
        unsafe
        {
            void Callback(void* data, byte* str)
            {
                string str_managed = Utf8StringMarshaller.ConvertToManaged(str)!;
                callback(str_managed);
            }

            GetCdRomDirsCallbackBase(Callback, null);
        }
    }

    private static unsafe void GetCdRomDirsCallbackBase(PHYSFS_StringCallback callback, void* data)
    {
        IntPtr cb = Marshal.GetFunctionPointerForDelegate(callback);
        PHYSFS_getCdRomDirsCallback(cb, data);

        [DllImport("physfs", EntryPoint = "PHYSFS_getCdRomDirsCallback", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_getCdRomDirsCallback(IntPtr cb, void* data);
    }

    private static string GetDirSeparator()
    {
        unsafe
        {
            byte* result = PHYSFS_getDirSeparator();
            return Utf8StringMarshaller.ConvertToManaged(result)!;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getDirSeparator", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern byte* PHYSFS_getDirSeparator();
    }

    public static partial string? GetErrorByCode(PhysFsErrorCode code)
    {
        unsafe
        {
            byte* data = PHYSFS_getErrorByCode(code);
            return Utf8StringMarshaller.ConvertToManaged(data);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getErrorByCode", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getErrorByCode(PhysFsErrorCode code);
    }

    public static partial string? GetLastError()
    {
        unsafe
        {
            byte* data = PHYSFS_getLastError();
            return Utf8StringMarshaller.ConvertToManaged(data);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getLastError", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getLastError();
    }

    public static partial PhysFsErrorCode GetLastErrorCode()
    {
        PhysFsErrorCode errorCode = PHYSFS_getLastErrorCode();
        return errorCode;

        [DllImport("physfs", EntryPoint = "PHYSFS_getLastErrorCode", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern PhysFsErrorCode PHYSFS_getLastErrorCode();
    }

    public static partial DateTime GetLastModTime(string fileName)
    {
        unsafe
        {
            byte* filename = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            long result = PHYSFS_getLastModTime(filename);
            Utf8StringMarshaller.Free(filename);
            return new DateTime(result);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getLastModTime", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe long PHYSFS_getLastModTime(byte* filename);
    }

    private static Version GetLinkedVersion()
    {
        unsafe
        {
            PHYSFS_getLinkedVersion(out PHYSFS_Version version);
            return new Version(version.major, version.minor, version.patch);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getLinkedVersion", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_getLinkedVersion(out PHYSFS_Version code);
    }

    public static partial string GetMountPoint(string directory)
    {
        unsafe
        {
            byte* dir = Utf8StringMarshaller.ConvertToUnmanaged(directory);
            byte* result = PHYSFS_getMountPoint(dir);
            Utf8StringMarshaller.Free(dir);

            string? mountPoint = Utf8StringMarshaller.ConvertToManaged(result);
            if (mountPoint is null)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return mountPoint;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getMountPoint", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getMountPoint(byte* dir);
    }

    public static partial string GetPrefDir(string organization, string app)
    {
        unsafe
        {
            byte* org = Utf8StringMarshaller.ConvertToUnmanaged(organization);
            byte* app_native = Utf8StringMarshaller.ConvertToUnmanaged(app);
            byte* result = PHYSFS_getPrefDir(org, app_native);

            Utf8StringMarshaller.Free(org);
            Utf8StringMarshaller.Free(app_native);

            string? prefDir = Utf8StringMarshaller.ConvertToManaged(result);
            if (prefDir is null)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return prefDir;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getPrefDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getPrefDir(byte* org, byte* app);
    }

    public static partial string? GetRealDir(string fileName)
    {
        unsafe
        {
            byte* filename = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            byte* result = PHYSFS_getRealDir(filename);
            Utf8StringMarshaller.Free(filename);

            return Utf8StringMarshaller.ConvertToManaged(result);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getRealDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getRealDir(byte* filename);
    }

    public static partial IEnumerable<string> GetSearchPath()
    {
        IntPtr ptr = PHYSFS_getSearchPath();
        IntPtr paths = ptr;

        if (paths == IntPtr.Zero)
        {
            PHYSFS_freeList(ptr);
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }
        else
        {
            while (true)
            {
                IntPtr path = Marshal.ReadIntPtr(paths);
                if (path == IntPtr.Zero) break;

                yield return Marshal.PtrToStringUTF8(path)!;
                paths += IntPtr.Size;
            }

            PHYSFS_freeList(ptr);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getSearchPath", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_getSearchPath();
    }

    public static partial void GetSearchPathCallback<T>(PhysFsStringCallback<T> callback, ref T data)
    {
        unsafe
        {
            void Callback(void* data, byte* str)
            {
                ref T data_managed = ref Unsafe.AsRef<T>(data);
                string str_managed = Utf8StringMarshaller.ConvertToManaged(str)!;
                callback(ref data_managed, str_managed);
            }

            void* data_native = Unsafe.AsPointer(ref data);
            GetSearchPathCallbackBase(Callback, data_native);
        }
    }

    public static partial void GetSearchPathCallback(PhysFsStringCallback callback)
    {
        unsafe
        {
            void Callback(void* data, byte* str)
            {
                string str_managed = Utf8StringMarshaller.ConvertToManaged(str)!;
                callback(str_managed);
            }

            GetSearchPathCallbackBase(Callback, null);
        }
    }

    private static unsafe void GetSearchPathCallbackBase(PHYSFS_StringCallback callback, void* data)
    {
        IntPtr cb = Marshal.GetFunctionPointerForDelegate(callback);
        PHYSFS_getSearchPathCallback(cb, data);

        [DllImport("physfs", EntryPoint = "PHYSFS_getSearchPathCallback", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_getSearchPathCallback(IntPtr cb, void* data);
    }

    public static partial string GetUserDir()
    {
        unsafe
        {
            byte* result = PHYSFS_getUserDir();
            return Utf8StringMarshaller.ConvertToManaged(result)!;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getUserDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getUserDir();
    }

    public static partial string? GetWriteDir()
    {
        unsafe
        {
            byte* result = PHYSFS_getWriteDir();
            return Utf8StringMarshaller.ConvertToManaged(result);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_getWriteDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe byte* PHYSFS_getWriteDir();
    }

    public static partial void Init(string? argv0)
    {
        unsafe
        {
#if ANDROID
            IntPtr jni = JNIEnv.Handle;
            IntPtr context = Application.Context.Handle;
            var androidData = new PHYSFS_AndroidInit { jnienv = jni, context = context };
            nint result = PHYSFS_init(&androidData);
#else
            byte* argv0_native = Utf8StringMarshaller.ConvertToUnmanaged(argv0);
            nint result = PHYSFS_init(argv0_native);
            Utf8StringMarshaller.Free(argv0_native);
#endif
            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_init", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_init(void* argv0);
    }

    public static partial bool IsDirectory(string fileName)
    {
        unsafe
        {
            byte* filename = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            nint result = PHYSFS_isDirectory(filename);
            Utf8StringMarshaller.Free(filename);
            return result != 0;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_isDirectory", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_isDirectory(byte* argv0);
    }

    public static partial bool IsSymbolicLink(string fileName)
    {
        unsafe
        {
            byte* filename = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            nint result = PHYSFS_isSymbolicLink(filename);
            Utf8StringMarshaller.Free(filename);
            return result != 0;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_isSymbolicLink", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_isSymbolicLink(byte* argv0);
    }

    public static partial void Mkdir(string dirName)
    {
        unsafe
        {
            byte* dirName_native = Utf8StringMarshaller.ConvertToUnmanaged(dirName);
            nint result = PHYSFS_mkdir(dirName_native);
            Utf8StringMarshaller.Free(dirName_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_mkdir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_mkdir(byte* dirName);
    }

    public static partial void Mount(string newDir, string? mountPoint, bool appendToPath)
    {
        unsafe
        {
            byte* newDir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            byte* mountPoint_native = Utf8StringMarshaller.ConvertToUnmanaged(mountPoint);
            nint result = PHYSFS_mount(newDir_native, mountPoint_native, appendToPath ? 1 : 0);

            Utf8StringMarshaller.Free(newDir_native);
            Utf8StringMarshaller.Free(mountPoint_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_mount", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_mount(byte* newDir, byte* mountPoint, nint appendToPath);
    }

    public static partial void MountHandle(
        PhysFsFileHandle file,
        string newDir,
        string? mountPoint,
        bool appendToPath)
    {
        unsafe
        {
            IntPtr handle = file.DangerousGetHandle();
            byte* newDir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            byte* mountPoint_native = Utf8StringMarshaller.ConvertToUnmanaged(mountPoint);
            nint result = PHYSFS_mountHandle(handle, newDir_native, mountPoint_native, appendToPath ? 1 : 0);

            Utf8StringMarshaller.Free(newDir_native);
            Utf8StringMarshaller.Free(mountPoint_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_mountHandle", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_mountHandle(IntPtr file, byte* newDir,
            byte* mountPoint, nint appendToPath);
    }

    public static partial void MountIo(
        IoHandle io,
        string newDir,
        string? mountPoint,
        bool appendToPath)
    {
        unsafe
        {
            byte* newDir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            byte* mountPoint_native = Utf8StringMarshaller.ConvertToUnmanaged(mountPoint);
            nint result = PHYSFS_mountIo(&io, newDir_native, mountPoint_native, appendToPath ? 1 : 0);

            Utf8StringMarshaller.Free(newDir_native);
            Utf8StringMarshaller.Free(mountPoint_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_mountIo", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern nint PHYSFS_mountIo(IoHandle* io, byte* newDir,
            byte* mountPoint, nint appendToPath);
    }

    public static partial void MountMemory(
        IntPtr buf,
        ulong len,
        PhysFsMountMemoryDel? del,
        string newDir,
        string? mountPoint,
        bool appendToPath)
    {
        unsafe
        {
            IntPtr del_native = IntPtr.Zero;
            if (del is not null)
            {
                del_native = Marshal.GetFunctionPointerForDelegate(del);
            }

            byte* newDir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            byte* mountPoint_native = Utf8StringMarshaller.ConvertToUnmanaged(mountPoint);
            nint result = PHYSFS_mountMemory((void*)buf, len, del_native,
                newDir_native, mountPoint_native, appendToPath ? 1 : 0);

            Utf8StringMarshaller.Free(newDir_native);
            Utf8StringMarshaller.Free(mountPoint_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_mountMemory", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern nint PHYSFS_mountMemory(void* buf, ulong len,
            IntPtr del, byte* newDir, byte* mountPoint, nint appendToPath);
    }

    public static partial PhysFsFileHandle OpenAppend(string fileName)
    {
        unsafe
        {
            byte* fileName_native = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            IntPtr handle = PHYSFS_openAppend(fileName_native);
            Utf8StringMarshaller.Free(fileName_native);

            if (handle == IntPtr.Zero)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return new PhysFsFileHandle(handle);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_openAppend", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_openAppend(byte* filename);
    }

    public static partial PhysFsFileHandle OpenRead(string fileName)
    {
        unsafe
        {
            byte* fileName_native = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            IntPtr handle = PHYSFS_openRead(fileName_native);
            Utf8StringMarshaller.Free(fileName_native);

            if (handle == IntPtr.Zero)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return new PhysFsFileHandle(handle);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_openRead", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_openRead(byte* filename);
    }

    public static partial PhysFsFileHandle OpenWrite(string fileName)
    {
        unsafe
        {
            byte* fileName_native = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            IntPtr handle = PHYSFS_openWrite(fileName_native);
            Utf8StringMarshaller.Free(fileName_native);

            if (handle == IntPtr.Zero)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return new PhysFsFileHandle(handle);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_openWrite", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_openWrite(byte* filename);
    }

    public static partial long Read(PhysFsFileHandle handle, IntPtr buffer, uint objSize, uint objCount)
    {
        IntPtr fileHandle = handle.DangerousGetHandle();
        long result = PHYSFS_read(fileHandle, buffer, objSize, objCount);

        if (result == -1)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        if (result < objCount)
        {
            if (EOF(handle))
            {
                PrintWarning("Encountered EOF while reading.");
            }
            else
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                PrintWarning($"{code}: {text}");
            }
        }

        return result;

        [Conditional("DEBUG")]
        static void PrintWarning(string? text) => Console.WriteLine(text);

        [DllImport("physfs", EntryPoint = "PHYSFS_openWrite", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_read(IntPtr handle, IntPtr buffer, uint objSize, uint objCount);
    }

    public static partial long ReadBytes(PhysFsFileHandle handle, IntPtr buffer, ulong len)
    {
        IntPtr fileHandle = handle.DangerousGetHandle();
        long result = PHYSFS_readBytes(fileHandle, buffer, len);

        if (result == -1)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        if ((ulong)result < len)
        {
            if (EOF(handle))
            {
                PrintWarning("Encountered EOF while reading.");
            }
            else
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                PrintWarning($"{code}: {text}");
            }
        }

        return result;

        [Conditional("DEBUG")]
        static void PrintWarning(string? text) => Console.WriteLine(text);

        [DllImport("physfs", EntryPoint = "PHYSFS_readBytes", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_readBytes(IntPtr handle, IntPtr buffer, ulong len);
    }

    public static partial short ReadSBE16(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSBE16(file.DangerousGetHandle(), out short val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSBE16(IntPtr file, out short val);
    }

    public static partial int ReadSBE32(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSBE32(file.DangerousGetHandle(), out int val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSBE32(IntPtr file, out int val);
    }

    public static partial long ReadSBE64(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSBE64(file.DangerousGetHandle(), out long val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSBE64(IntPtr file, out long val);
    }

    public static partial short ReadSLE16(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSLE16(file.DangerousGetHandle(), out short val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSLE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSLE16(IntPtr file, out short val);
    }

    public static partial int ReadSLE32(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSLE32(file.DangerousGetHandle(), out int val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSLE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSLE32(IntPtr file, out int val);
    }

    public static partial long ReadSLE64(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readSLE64(file.DangerousGetHandle(), out long val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readSLE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readSLE64(IntPtr file, out long val);
    }

    public static partial short ReadUBE16(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readUBE16(file.DangerousGetHandle(), out short val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readUBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readUBE16(IntPtr file, out short val);
    }

    public static partial int ReadUBE32(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readUBE32(file.DangerousGetHandle(), out int val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readUBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readUBE32(IntPtr file, out int val);
    }

    public static partial long ReadUBE64(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readUBE64(file.DangerousGetHandle(), out long val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readUBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readUBE64(IntPtr file, out long val);
    }

    public static partial short ReadULE16(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readULE16(file.DangerousGetHandle(), out short val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readULE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readULE16(IntPtr file, out short val);
    }

    public static partial int ReadULE32(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readULE32(file.DangerousGetHandle(), out int val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readULE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readULE32(IntPtr file, out int val);
    }

    public static partial long ReadULE64(PhysFsFileHandle file)
    {
        nint result = PHYSFS_readULE64(file.DangerousGetHandle(), out long val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return val;

        [DllImport("physfs", EntryPoint = "PHYSFS_readULE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_readULE64(IntPtr file, out long val);
    }

    public static partial void RegisterArchiver(ref ArchiverHandle archiver)
    {
        unsafe
        {
            fixed (ArchiverHandle* handle = &archiver)
            {
                nint result = PHYSFS_registerArchiver(handle);
                if (result == 0)
                {
                    PhysFsErrorCode code = GetLastErrorCode();
                    string? text = GetErrorByCode(code);
                    throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_registerArchiver", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_registerArchiver(ArchiverHandle* archiver);
    }

    public static partial void RemoveFromSearchPath(string oldDir)
    {
        unsafe
        {
            byte* oldDir_native = Utf8StringMarshaller.ConvertToUnmanaged(oldDir);
            nint result = PHYSFS_removeFromSearchPath(oldDir_native);
            Utf8StringMarshaller.Free(oldDir_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_removeFromSearchPath", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe int PHYSFS_removeFromSearchPath(byte* oldDir);
    }

    public static partial void Seek(PhysFsFileHandle handle, ulong pos)
    {
        nint result = PHYSFS_seek(handle.DangerousGetHandle(), pos);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_seek", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_seek(IntPtr handle, ulong pos);
    }

    public static partial void SetAllocator(NullableRef<AllocatorHandle> allocator)
    {
        unsafe
        {
            nint result;
            if (allocator.HasValue)
            {
                fixed (AllocatorHandle* handle = &allocator.Value)
                {
                    result = PHYSFS_setAllocator(handle);
                }
            }
            else
            {
                result = PHYSFS_setAllocator(null);
            }

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_setAllocator", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static unsafe extern nint PHYSFS_setAllocator(AllocatorHandle* allocator);
    }

    public static partial void SetBuffer(PhysFsFileHandle handle, ulong bufsize)
    {
        nint result = PHYSFS_setBuffer(handle.DangerousGetHandle(), bufsize);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_setBuffer", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_setBuffer(IntPtr handle, ulong bufsize);
    }

    public static partial void SetErrorCode(PhysFsErrorCode code)
    {
        PHYSFS_setErrorCode(code);

        [DllImport("physfs", EntryPoint = "PHYSFS_setErrorCode", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern void PHYSFS_setErrorCode(PhysFsErrorCode code);
    }

    public static partial void SetRoot(string archive, string? subdir)
    {
        unsafe
        {
            byte* archive_native = Utf8StringMarshaller.ConvertToUnmanaged(archive);
            byte* subdir_native = Utf8StringMarshaller.ConvertToUnmanaged(subdir);
            nint result = PHYSFS_setRoot(archive_native, subdir_native);

            Utf8StringMarshaller.Free(archive_native);
            Utf8StringMarshaller.Free(subdir_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_setRoot", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_setRoot(byte* archive, byte* subdir);
    }

    public static partial void SetSaneConfig(string organization,
        string appName, string? archiveExt, bool includeCdRoms, bool archivesFirst)
    {
        unsafe
        {
            byte* org_native = Utf8StringMarshaller.ConvertToUnmanaged(organization);
            byte* app_native = Utf8StringMarshaller.ConvertToUnmanaged(appName);
            byte* ext_native = Utf8StringMarshaller.ConvertToUnmanaged(archiveExt);
            nint result = PHYSFS_setSaneConfig(org_native, app_native, ext_native,
                includeCdRoms ? 1 : 0, archivesFirst ? 1 : 0);

            Utf8StringMarshaller.Free(org_native);
            Utf8StringMarshaller.Free(app_native);
            Utf8StringMarshaller.Free(ext_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_setSaneConfig", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_setSaneConfig(byte* organization,
            byte* appName, byte* archiveExt, nint includeCdRoms, nint archivesFirst);
    }

    public static partial void SetWriteDir(string? newDir)
    {
        unsafe
        {
            byte* dir_native = Utf8StringMarshaller.ConvertToUnmanaged(newDir);
            nint result = PHYSFS_setWriteDir(dir_native);

            Utf8StringMarshaller.Free(dir_native);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_setWriteDir", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_setWriteDir(byte* newDir);
    }

    public static partial PhysFsStat Stat(string fileName)
    {
        unsafe
        {
            byte* fname = Utf8StringMarshaller.ConvertToUnmanaged(fileName);
            nint result = PHYSFS_stat(fname, out PHYSFS_FileStat stat);

            Utf8StringMarshaller.Free(fname);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }

            return new PhysFsStat
            {
                FileSize = stat.filesize,
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stat.createtime).DateTime,
                LastModifiedAt = DateTimeOffset.FromUnixTimeSeconds(stat.modtime).DateTime,
                LastAccessedAt = DateTimeOffset.FromUnixTimeSeconds(stat.accesstime).DateTime,
                FileType = stat.filetype,
                IsReadOnly = stat._readonly != 0
            };
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_stat", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_stat(byte* fname, out PHYSFS_FileStat stat);
    }

    public static partial IEnumerable<PhysFsArchiveInfo> SupportedArchiveTypes()
    {
        IntPtr ptr = PHYSFS_supportedArchiveTypes();
        IntPtr archiveInfos = ptr;

        if (archiveInfos == IntPtr.Zero)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }
        else
        {
            while (true)
            {
                IntPtr info = Marshal.ReadIntPtr(archiveInfos);
                if (info == IntPtr.Zero) break;

                var archiveInfo = Marshal.PtrToStructure<PHYSFS_ArchiveInfo>(info);
                yield return PhysFsArchiveInfo.FromNative(archiveInfo);
                archiveInfos += IntPtr.Size;
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_supportedArchiveTypes", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe IntPtr PHYSFS_supportedArchiveTypes();
    }

    public static partial short SwapSBE16(short val)
    {
        return PHYSFS_swapSBE16(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern short PHYSFS_swapSBE16(short val);
    }

    public static partial int SwapSBE32(int val)
    {
        return PHYSFS_swapSBE32(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern int PHYSFS_swapSBE32(int val);
    }

    public static partial long SwapSBE64(long val)
    {
        return PHYSFS_swapSBE64(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_swapSBE64(long val);
    }

    public static partial short SwapSLE16(short val)
    {
        return PHYSFS_swapSLE16(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSLE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern short PHYSFS_swapSLE16(short val);
    }

    public static partial int SwapSLE32(int val)
    {
        return PHYSFS_swapSLE32(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSLE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern int PHYSFS_swapSLE32(int val);
    }

    public static partial long SwapSLE64(long val)
    {
        return PHYSFS_swapSLE64(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapSLE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_swapSLE64(long val);
    }

    public static partial ushort SwapUBE16(ushort val)
    {
        return PHYSFS_swapUBE16(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapUBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern ushort PHYSFS_swapUBE16(ushort val);
    }

    public static partial uint SwapUBE32(uint val)
    {
        return PHYSFS_swapUBE32(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapUBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern uint PHYSFS_swapUBE32(uint val);
    }

    public static partial ulong SwapUBE64(ulong val)
    {
        return PHYSFS_swapUBE64(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapUBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern ulong PHYSFS_swapUBE64(ulong val);
    }

    public static partial ushort SwapULE16(ushort val)
    {
        return PHYSFS_swapULE16(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapULE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern ushort PHYSFS_swapULE16(ushort val);
    }

    public static partial uint SwapULE32(uint val)
    {
        return PHYSFS_swapULE32(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapULE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern uint PHYSFS_swapULE32(uint val);
    }

    public static partial ulong SwapULE64(ulong val)
    {
        return PHYSFS_swapULE64(val);

        [DllImport("physfs", EntryPoint = "PHYSFS_swapULE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern ulong PHYSFS_swapULE64(ulong val);
    }

    public static partial ulong Tell(PhysFsFileHandle handle)
    {
        long result = PHYSFS_tell(handle.DangerousGetHandle());
        if (result == -1)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return (ulong)result;

        [DllImport("physfs", EntryPoint = "PHYSFS_tell", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_tell(IntPtr handle);
    }

    public static partial int Ucs4stricmp(ReadOnlySpan<uint> str1, ReadOnlySpan<uint> str2)
    {
        unsafe
        {
            fixed (uint* str1_native = str1)
            {
                fixed (uint* str2_native = str2)
                {
                    return PHYSFS_ucs4stricmp(str1_native, str2_native);
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_ucs4stricmp", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe int PHYSFS_ucs4stricmp(uint* str1, uint* str2);
    }

    public static partial void Unmount(string oldDir)
    {
        unsafe
        {
            byte* dir = Utf8StringMarshaller.ConvertToUnmanaged(oldDir);
            nint result = PHYSFS_unmount(dir);

            Utf8StringMarshaller.Free(dir);

            if (result == 0)
            {
                PhysFsErrorCode code = GetLastErrorCode();
                string? text = GetErrorByCode(code);
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_unmount", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe nint PHYSFS_unmount(byte* oldDir);
    }

    public static partial int Utf16stricmp(ReadOnlySpan<ushort> str1, ReadOnlySpan<ushort> str2)
    {
        unsafe
        {
            fixed (ushort* str1_native = str1)
            {
                fixed (ushort* str2_native = str2)
                {
                    return PHYSFS_utf16stricmp(str1_native, str2_native);
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf16stricmp", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe int PHYSFS_utf16stricmp(ushort* str1, ushort* str2);
    }

    public static partial byte[] Utf8FromLatin1(byte[] src, ulong len)
    {
        unsafe
        {
            fixed (byte* source = src)
            {
                byte[] dest = new byte[len];
                fixed (byte* dest_native = dest)
                {
                    PHYSFS_utf8FromLatin1(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8FromLatin1", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8FromLatin1(byte* src, byte* dst, ulong len);
    }

    public static partial byte[] Utf8FromUcs2(ushort[] src, ulong len)
    {
        unsafe
        {
            fixed (ushort* source = src)
            {
                byte[] dest = new byte[len];
                fixed (byte* dest_native = dest)
                {
                    PHYSFS_utf8FromUcs2(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8FromUcs2", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8FromUcs2(ushort* src, byte* dst, ulong len);
    }

    public static partial byte[] Utf8FromUcs4(uint[] src, ulong len)
    {
        unsafe
        {
            fixed (uint* source = src)
            {
                byte[] dest = new byte[len];
                fixed (byte* dest_native = dest)
                {
                    PHYSFS_utf8FromUcs2(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8FromUcs2", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8FromUcs2(uint* src, byte* dst, ulong len);
    }

    public static partial byte[] Utf8FromUtf16(ushort[] src, ulong len)
    {
        unsafe
        {
            fixed (ushort* source = src)
            {
                byte[] dest = new byte[len];
                fixed (byte* dest_native = dest)
                {
                    PHYSFS_utf8FromUtf16(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8FromUtf16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8FromUtf16(ushort* src, byte* dst, ulong len);
    }

    public static partial int Utf8stricmp(string str1, string str2)
    {
        unsafe
        {
            byte* str1_native = Utf8StringMarshaller.ConvertToUnmanaged(str1);
            byte* str2_native = Utf8StringMarshaller.ConvertToUnmanaged(str1);
            int result = PHYSFS_utf8stricmp(str1_native, str2_native);

            Utf8StringMarshaller.Free(str1_native);
            Utf8StringMarshaller.Free(str2_native);

            return result;
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8stricmp", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe int PHYSFS_utf8stricmp(byte* str1, byte* str2);
    }

    public static partial ushort[] Utf8ToUcs2(byte[] src, ulong len)
    {
        unsafe
        {
            fixed (byte* source = src)
            {
                ushort[] dest = new ushort[len];
                fixed (ushort* dest_native = dest)
                {
                    PHYSFS_utf8ToUcs2(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8ToUcs2", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8ToUcs2(byte* src, ushort* dst, ulong len);
    }

    public static partial uint[] Utf8ToUcs4(byte[] src, ulong len)
    {
        unsafe
        {
            fixed (byte* source = src)
            {
                uint[] dest = new uint[len];
                fixed (uint* dest_native = dest)
                {
                    PHYSFS_utf8ToUcs2(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8ToUcs2", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8ToUcs2(byte* src, uint* dst, ulong len);
    }

    public static partial ushort[] Utf8ToUtf16(byte[] src, ulong len)
    {
        unsafe
        {
            fixed (byte* source = src)
            {
                ushort[] dest = new ushort[len];
                fixed (ushort* dest_native = dest)
                {
                    PHYSFS_utf8ToUcs2(source, dest_native, len);
                    return dest;
                }
            }
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_utf8ToUcs2", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern unsafe void PHYSFS_utf8ToUcs2(byte* src, ushort* dst, ulong len);
    }

    public static partial long Write(PhysFsFileHandle handle, IntPtr buffer,
        uint objSize, uint objCount)
    {
        IntPtr fileHandle = handle.DangerousGetHandle();
        long result = PHYSFS_write(fileHandle, buffer, objSize, objCount);

        if (result < objCount)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);

            if (result == -1)
            {
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
            else
            {
                PrintWarning($"{code}: {text}");
            }
        }

        return result;

        [Conditional("DEBUG")]
        static void PrintWarning(string? text) => Console.WriteLine(text);

        [DllImport("physfs", EntryPoint = "PHYSFS_write", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_write(IntPtr handle, IntPtr buffer,
            uint objSize, uint objCount);
    }

    public static partial long WriteBytes(PhysFsFileHandle handle, IntPtr buffer, ulong len)
    {
        IntPtr fileHandle = handle.DangerousGetHandle();
        long result = PHYSFS_writeBytes(fileHandle, buffer, len);

        if (result < 0 || (ulong)result < len)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);

            if (result == -1)
            {
                throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
            }
            else
            {
                PrintWarning($"{code}: {text}");
            }
        }

        return result;

        [Conditional("DEBUG")]
        static void PrintWarning(string? text) => Console.WriteLine(text);

        [DllImport("physfs", EntryPoint = "PHYSFS_writeBytes", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern long PHYSFS_writeBytes(IntPtr handle, IntPtr buffer, ulong len);
    }

    public static partial void WriteSBE16(PhysFsFileHandle file, short val)
    {
        nint result = PHYSFS_writeSBE16(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSBE16(IntPtr file, short val);
    }

    public static partial void WriteSBE32(PhysFsFileHandle file, int val)
    {
        nint result = PHYSFS_writeSBE32(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSBE32(IntPtr file, int val);
    }

    public static partial void WriteSBE64(PhysFsFileHandle file, long val)
    {
        nint result = PHYSFS_writeSBE64(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSBE64(IntPtr file, long val);
    }

    public static partial void WriteSLE16(PhysFsFileHandle file, short val)
    {
        nint result = PHYSFS_writeSLE16(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSLE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSLE16(IntPtr file, short val);
    }

    public static partial void WriteSLE32(PhysFsFileHandle file, int val)
    {
        nint result = PHYSFS_writeSLE32(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSLE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSLE32(IntPtr file, int val);
    }

    public static partial void WriteSLE64(PhysFsFileHandle file, long val)
    {
        nint result = PHYSFS_writeSLE64(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeSLE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeSLE64(IntPtr file, long val);
    }

    public static partial void WriteUBE16(PhysFsFileHandle file, ushort val)
    {
        nint result = PHYSFS_writeUBE16(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeUBE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeUBE16(IntPtr file, ushort val);
    }

    public static partial void WriteUBE32(PhysFsFileHandle file, uint val)
    {
        nint result = PHYSFS_writeUBE32(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeUBE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeUBE32(IntPtr file, uint val);
    }

    public static partial void WriteUBE64(PhysFsFileHandle file, ulong val)
    {
        nint result = PHYSFS_writeUBE64(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeUBE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeUBE64(IntPtr file, ulong val);
    }

    public static partial void WriteULE16(PhysFsFileHandle file, ushort val)
    {
        nint result = PHYSFS_writeULE16(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeULE16", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeULE16(IntPtr file, ushort val);
    }

    public static partial void WriteULE32(PhysFsFileHandle file, uint val)
    {
        nint result = PHYSFS_writeULE32(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeULE32", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeULE32(IntPtr file, uint val);
    }

    public static partial void WriteULE64(PhysFsFileHandle file, ulong val)
    {
        nint result = PHYSFS_writeULE64(file.DangerousGetHandle(), val);
        if (result == 0)
        {
            PhysFsErrorCode code = GetLastErrorCode();
            string? text = GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        [DllImport("physfs", EntryPoint = "PHYSFS_writeULE64", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_writeULE64(IntPtr file, ulong val);
    }
}
