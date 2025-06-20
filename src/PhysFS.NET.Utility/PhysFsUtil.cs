
using System.Text;

namespace Icculus.PhysFS.NET;

/// <summary>
/// Set of utility functions related to PhysicsFS but not included
/// to the official API you may find useful.
/// </summary>
/// <remarks>
/// This class was deliberately made partial so you can add any additional functionality you want here.
/// </remarks>
public static partial class PhysFsUtil
{
    /// <summary>
    /// Reads all data from a PhysicsFS filehandle to a safe buffer.
    /// Usually used with <see cref="UnmanagedMemoryStream"/>.
    /// </summary>
    /// <param name="handle">
    /// handle returned from <see cref="PhysicsFS.OpenRead"/>.
    /// </param>
    /// <returns>Contents of the file allocated in a <see cref="SafeHGlobalBuffer"/> utility.</returns>
    public static SafeHGlobalBuffer ReadToBuffer(PhysFsFileHandle handle)
    {
        long len = PhysicsFS.FileLength(handle);
        SafeHGlobalBuffer buffer = new SafeHGlobalBuffer(len);
        IntPtr ptr = buffer.DangerousGetHandle();
        PhysicsFS.ReadBytes(handle, ptr, (ulong)len);
        return buffer;
    }

    /// <summary>
    /// Reads all data from a PhysicsFS filehandle to a byte array.
    /// </summary>
    /// <param name="handle">
    /// handle returned from <see cref="PhysicsFS.OpenRead"/>.
    /// </param>
    /// <returns>Retrieved file contents.</returns>
    public static byte[] ReadAllBytes(PhysFsFileHandle handle)
    {
        long len = PhysicsFS.FileLength(handle);
        byte[] buffer = new byte[len];
        GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr ptr = gcHandle.AddrOfPinnedObject();
        PhysicsFS.ReadBytes(handle, ptr, (ulong)len);
        gcHandle.Free();
        return buffer;
    }

    /// <summary>
    /// Writes all data from a byte array to a PhysicsFS filehandle.
    /// </summary>
    /// <param name="handle">
    /// handle returned from <see cref="PhysicsFS.OpenWrite"/> or <see cref="PhysicsFS.OpenAppend"/>.
    /// </param>
    /// <param name="buffer"></param>
    /// <returns>number of bytes written.</returns>
    public static long WriteAllBytes(PhysFsFileHandle handle, byte[] buffer)
    {
        GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr ptr = gcHandle.AddrOfPinnedObject();
        long result = PhysicsFS.WriteBytes(handle, ptr, (ulong)buffer.LongLength);
        gcHandle.Free();
        return result;
    }

    /// <summary>
    /// Convert a Latin1 string to a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// Latin1 strings are 8-bits per character: a popular "high ASCII" encoding.<br/><br/>
    /// UTF-8 expands latin1 codepoints over 127 from 1 to 2 bytes, so the string
    /// may grow in some cases.<br/><br/>
    /// Please note that we do not supply a UTF-8 to Latin1 converter, since Latin1
    /// can't express most Unicode codepoints. It's a legacy encoding; you should
    /// be converting away from it at all times.
    /// </remarks>
    /// <param name="source">Null-terminated source string in Latin1 format.</param>
    /// <returns>
    /// converted UTF-8 string.
    /// </returns>
    public static string? Utf8FromLatin1(string source)
    {
        byte[] src = Encoding.Latin1.GetBytes(source);
        byte[] dest = PhysicsFS.Utf8FromLatin1(src, (ulong)src.Length + 1);
        return Encoding.UTF8.GetString(dest);
    }


    /// <summary>
    /// Convert a UTF-16 string to a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// This function will not report an error if there are invalid UTF-16
    /// sequences in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UTF-16 strings are 16-bits per character (except some chars, which are
    /// 32-bits): <c>TCHAR</c> on Windows, when building with Unicode support. Modern
    /// Windows releases use UTF-16. Windows releases before 2000 used TCHAR, but
    /// only handled UCS-2. UTF-16 _is_ UCS-2, except for the characters that
    /// are 4 bytes, which aren't representable in UCS-2 at all anyhow. If you
    /// aren't sure, you should be using UTF-16 at this point on Windows.<br/><br/>
    /// UTF-8 never uses more than 32-bits per character, so while it may shrink
    /// a UTF-16 string, it may also expand it.
    /// </remarks>
    /// <param name="source">Null-terminated source string in UTF-16 format.</param>
    /// <returns>Converted UTF-8 string.</returns>
    public static string? Utf8FromUtf16(string source)
    {
        ushort[] src = Array.ConvertAll(source.ToCharArray(), c => (ushort)c);
        byte[] dest = PhysicsFS.Utf8FromUtf16(src, (ulong)src.Length + 1);
        return Encoding.UTF8.GetString(dest);
    }
}
