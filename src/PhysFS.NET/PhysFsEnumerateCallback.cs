namespace Icculus.PhysFS.NET;

/// <summary>
/// Possible return values from <see cref="PhysFsEnumerateCallback"/>
/// or <see cref="PhysFsEnumerateCallback{T}"/>.
/// </summary>
/// <remarks>
/// These values dictate if an enumeration callback should continue to fire,
/// or stop (and why it is stopping).<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysFsEnumerateCallback"/><br/>
/// <seealso cref="PhysFsEnumerateCallback{T}"/><br/>
/// <seealso cref="PhysicsFS.Enumerate"/><br/>
/// <seealso cref="PhysicsFS.Enumerate{T}"/>
/// </remarks>
public enum PhysFsEnumerateCallbackResult
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
    /// Keep enumerating, no problems
    /// </summary>
    OK = 1
}

/// <summary>
/// Function signature for callbacks that enumerate and return results.
/// </summary>
/// <remarks>
/// This is the same thing as <see cref="PhysFsEnumFilesCallback"/> from PhysicsFS 2.0,
/// except it can return a result from the callback: namely:
/// if you're looking for something specific, once you find it,
/// you can tell PhysicsFS to stop enumerating further. This is used with
/// <see cref="PhysicsFS.Enumerate{T}"/>,
/// which we hopefully got right this time. :)<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.Enumerate{T}"/><br/>
/// <seealso cref="PhysFsEnumerateCallbackResult"/>
/// </remarks>
/// <typeparam name="T">Type of the data, passed to the callback.</typeparam>
/// <param name="data">
/// User-defined data reference, passed through from the API that eventually called the callback.
/// </param>
/// <param name="fileDir">
/// A string containing the full path, in platform-independent notation,
/// of the directory containing this file. In most cases, this is the directory
/// on which you requested enumeration, passed in the callback for your convenience.
/// </param>
/// <param name="fileName">
/// The filename that is being enumerated.
/// It may not be in alphabetical order compared to other callbacks that have fired,
/// and it will not contain the full path. You can recreate the fullpath with 
/// <paramref name="fileDir"/>/<paramref name="fileName"/> ...
/// The file can be a subdirectory, a file, a symlink, etc.
/// </param>
/// <returns>A value from <see cref="PhysFsEnumerateCallbackResult"/></returns>
public delegate PhysFsEnumerateCallbackResult PhysFsEnumerateCallback<T>
(
    ref T data,
    string fileDir,
    string fileName
);

/// <summary>
/// Function signature for callbacks that enumerate and return results.
/// </summary>
/// <remarks>
/// This is the same thing as <see cref="PhysFsEnumFilesCallback"/> from PhysicsFS 2.0,
/// except it can return a result from the callback: namely:
/// if you're looking for something specific, once you find it,
/// you can tell PhysicsFS to stop enumerating further. This is used with
/// <see cref="PhysicsFS.Enumerate"/>,
/// which we hopefully got right this time. :)<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.Enumerate"/><br/>
/// <seealso cref="PhysFsEnumerateCallbackResult"/>
/// </remarks>
/// <param name="fileDir">
/// A string containing the full path, in platform-independent notation,
/// of the directory containing this file. In most cases, this is the directory
/// on which you requested enumeration, passed in the callback for your convenience.
/// </param>
/// <param name="fileName">
/// The filename that is being enumerated.
/// It may not be in alphabetical order compared to other callbacks that have fired,
/// and it will not contain the full path. You can recreate the fullpath with 
/// <paramref name="fileDir"/>/<paramref name="fileName"/> ...
/// The file can be a subdirectory, a file, a symlink, etc.
/// </param>
/// <returns>A value from <see cref="PhysFsEnumerateCallbackResult"/></returns>
public delegate PhysFsEnumerateCallbackResult PhysFsEnumerateCallback
(
    string fileDir,
    string fileName
);
