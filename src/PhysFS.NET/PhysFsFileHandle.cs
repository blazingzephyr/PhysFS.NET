namespace Icculus.PhysFS.NET;

/// <summary>
/// A PhysicsFS file handle.
/// </summary>
/// <remarks>
/// You get one of these when you open a file for reading,
/// writing, or appending via PhysicsFS.<br/><br/>
/// As you can see from the lack of meaningful fields, you should treat this
/// as opaque data. Don't try to manipulate the file handle, just pass the
/// pointer you got, unmolested, to various PhysicsFS APIs.<br/><br/>
/// Unlike the original API, this handle is automatically closed
/// after <see cref="SafeHandle.Dispose()"/> is called,
/// so you don't have to look for a Close method.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.OpenRead"/><br/>
/// <seealso cref="PhysicsFS.OpenWrite"/><br/>
/// <seealso cref="PhysicsFS.OpenAppend"/><br/>
/// <seealso cref="PhysicsFS.Read"/><br/>
/// <seealso cref="PhysicsFS.Write"/><br/>
/// <seealso cref="PhysicsFS.Seek"/><br/>
/// <seealso cref="PhysicsFS.Tell"/><br/>
/// <seealso cref="PhysicsFS.EOF"/><br/>
/// <seealso cref="PhysicsFS.SetBuffer"/><br/>
/// <seealso cref="PhysicsFS.Flush"/>
/// </remarks>
public class PhysFsFileHandle : SafeHandle
{
    /// <summary>
    /// Determines whether the handle is invalid.
    /// </summary>
    public override bool IsInvalid => handle == IntPtr.Zero;

    internal PhysFsFileHandle(IntPtr handle) : base(IntPtr.Zero, true)
    {
        SetHandle(handle);
    }

    /// <summary>
    /// Automatically closes the file when the handle is disposed.
    /// </summary>
    protected override bool ReleaseHandle()
    {
        nint result = PHYSFS_close(handle);
        if (result == 0)
        {
            PhysFsErrorCode code = PhysicsFS.GetLastErrorCode();
            string? text = PhysicsFS.GetErrorByCode(code);
            throw PhysFsExceptionUtility.GetExceptionForPhysFsErr(code, text);
        }

        return true;

        [DllImport("physfs", EntryPoint = "PHYSFS_close", ExactSpelling = true)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        static extern nint PHYSFS_close(IntPtr handle);
    }
}
