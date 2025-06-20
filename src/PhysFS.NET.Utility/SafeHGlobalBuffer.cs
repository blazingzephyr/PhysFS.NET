
namespace System.Runtime.InteropServices;

/// <summary>
/// Basic SafeBuffer implementation.
/// </summary>
public sealed class SafeHGlobalBuffer : SafeBuffer
{
    public SafeHGlobalBuffer(long length) : base(true)
    {
        IntPtr handle = Marshal.AllocHGlobal((int)length);
        SetHandle(handle);
        Initialize((ulong)length);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.FreeHGlobal(handle);
        return true;
    }
}
