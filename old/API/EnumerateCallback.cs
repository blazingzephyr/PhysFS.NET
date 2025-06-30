
namespace Old.Icculus.PhysFS.NET;

/// <summary>
/// Possible return values from <see cref="EnumerateCallback{T}" />.<br/>
/// 
/// These values dictate if an enumeration callback should continue to fire,
/// or stop (and why it is stopping).<br/><br/>
/// 
/// See also:<br/>
/// <seealso cref="EnumerateCallback{T}" /><br/>
/// <seealso cref="PhysFS.Enumerate{T}(string, EnumerateCallback{T}, ref T)" /><br/>
/// </summary>
public enum EnumerateCallbackResult : sbyte
{
    /// <summary>
    /// Stop enumerating, report error to app.
    /// </summary>
    Error = -1,

    /// <summary>
    /// Stop enumerating, report success to app.
    /// </summary>
    Stop = 0,

    /// <summary>
    /// Keep enumerating, no problems encountered.
    /// </summary>
    Continue = 1
}

/// <summary>
/// Function signature for callbacks that enumerate and return results.
/// </summary>
/// <remarks>
/// See also:<br/>
/// <seealso cref="PhysFS.Enumerate{T}(string, EnumerateCallback{T}, ref T)"/><br/>
/// <seealso cref="EnumerateCallbackResult"/>
/// </remarks>
/// <typeparam name="T">
/// Type of data reference passed.
/// </typeparam>
/// <param name="data">
/// User-defined data, passed through from the API
/// that eventually called the callback.
/// </param>
/// <param name="directory">
/// A string containing the full path, in platform-independent
/// notation, of the directory containing this file. In most
/// cases, this is the directory on which you requested
/// enumeration, passed in the callback for your convenience.
/// </param>
/// <param name="fileName">
/// The filename that is being enumerated. It may not be in
/// alphabetical order compared to other callbacks that have
/// fired, and it will not contain the full path.You can
/// recreate the fullpath with directory/fileName...The file
/// can be a subdirectory, a file, a symlink, etc.
/// </param>
/// <returns>
/// A value from PHYSFS_EnumerateCallbackResult.
/// </returns>
public delegate EnumerateCallbackResult EnumerateCallback<T>(ref T data, string directory, string fileName);
