
namespace Icculus.PhysFS.NET;

public static partial class PhysicsFS
{
    internal struct PHYSFS_AndroidInit
    {
        public IntPtr jnienv;
        public IntPtr context;
    }

    internal struct PHYSFS_FileStat
    {
        public long filesize;
        public long modtime;
        public long createtime;
        public long accesstime;
        public PhysFsFileType filetype;
        public nint _readonly;
    }

    internal struct PHYSFS_Version
    {
        public byte major;
        public byte minor;
        public byte patch;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate nint PHYSFS_EnumerateCallback(void* data, byte* origdir, byte* fname);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate void PHYSFS_EnumFilesCallback(void* data, byte* origdir, byte* fname);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate void PHYSFS_StringCallback(void* data, byte* str);

    [DllImport("physfs", EntryPoint = "PHYSFS_freeList", ExactSpelling = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static extern void PHYSFS_freeList(IntPtr dir);

    [DllImport("physfs", EntryPoint = "PHYSFS_isInit", ExactSpelling = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static extern nint PHYSFS_isInit();

    [DllImport("physfs", EntryPoint = "PHYSFS_permitSymbolicLinks", ExactSpelling = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static extern void PHYSFS_permitSymbolicLinks(int allow);

    [DllImport("physfs", EntryPoint = "PHYSFS_symbolicLinksPermitted", ExactSpelling = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static extern nint PHYSFS_symbolicLinksPermitted();
}
