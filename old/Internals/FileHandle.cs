
namespace Old.Icculus.PhysFS.NET.Internals;

[NativeMarshalling(typeof(SafeHandleMarshaller<FileHandle>))]
public class FileHandle() : SafeHandle(IntPtr.Zero, true)
{
    public static readonly FileHandle Invalid = new FileHandle();
    public override bool IsInvalid => handle == IntPtr.Zero;
    protected override bool ReleaseHandle() => physfs.PHYSFS_close(this);
}
