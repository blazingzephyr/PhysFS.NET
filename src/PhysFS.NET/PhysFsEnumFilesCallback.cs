namespace Icculus.PhysFS.NET;

/// <summary>
/// Function signature for callbacks that enumerate files.
/// </summary>
/// <remarks>
/// These are used to report a list of directory entries to an original caller,
/// one file/dir/symlink per callback. All strings are UTF-8 encoded.
/// Functions should not try to modify or free any string's memory.<br/><br/>
/// These callbacks are used, starting in PhysicsFS 1.1, as an alternative to functions that
/// would return lists that need to be cleaned up internally. The callback means that the
/// library doesn't need to allocate an entire list and all the strings up front.<br/><br/>
/// Be aware that promised data ordering in the list versions are not necessarily so in the callback versions.
/// Check the documentation on specific APIs,
/// but strings may not be sorted as you expect and you might get duplicate strings.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.EnumerateFilesCallback{T}"/>
/// </remarks>
/// <typeparam name="T">Type of the data, passed to the callback.</typeparam>
/// <param name="data">
/// User-defined data reference, passed through from the API that eventually called the callback.
/// </param>
/// <param name="fileDir">
/// A string containing the full path, in platform-independent notation,
/// of the directory containing this file. In most cases,
/// this is the directory on which you requested enumeration, passed in the callback for your convenience.
/// </param>
/// <param name="fileName">
/// The filename that is being enumerated. It may not be in alphabetical order compared to
/// other callbacks that have fired, and it will not contain the full path.
/// You can recreate the fullpath with <paramref name="fileDir"/>/<paramref name="fileName"/> ...
/// ... The file can be a subdirectory, a file, a symlink, etc.
/// </param>
[Obsolete("As of PhysicsFS 2.1, Use PhysFsEnumerateCallback with PhysicsFS.Enumerate() instead; " +
    "it gives you more control over the process.\r\n", false)]
public delegate void PhysFsEnumFilesCallback<T>
(
    ref T data,
    string fileDir,
    string fileName
);

/// <summary>
/// Function signature for callbacks that enumerate files.
/// </summary>
/// <remarks>
/// These are used to report a list of directory entries to an original caller,
/// one file/dir/symlink per callback. All strings are UTF-8 encoded.
/// Functions should not try to modify or free any string's memory.<br/><br/>
/// These callbacks are used, starting in PhysicsFS 1.1, as an alternative to functions that
/// would return lists that need to be cleaned up internally. The callback means that the
/// library doesn't need to allocate an entire list and all the strings up front.<br/><br/>
/// Be aware that promised data ordering in the list versions are not necessarily so in the callback versions.
/// Check the documentation on specific APIs,
/// but strings may not be sorted as you expect and you might get duplicate strings.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.EnumerateFilesCallback"/>
/// </remarks>
/// <param name="fileDir">
/// A string containing the full path, in platform-independent notation,
/// of the directory containing this file. In most cases,
/// this is the directory on which you requested enumeration, passed in the callback for your convenience.
/// </param>
/// <param name="fileName">
/// The filename that is being enumerated. It may not be in alphabetical order compared to
/// other callbacks that have fired, and it will not contain the full path.
/// You can recreate the fullpath with <paramref name="fileDir"/>/<paramref name="fileName"/> ...
/// ... The file can be a subdirectory, a file, a symlink, etc.
/// </param>
[Obsolete("As of PhysicsFS 2.1, Use PhysFsEnumerateCallback with PhysicsFS.Enumerate() instead; " +
    "it gives you more control over the process.\r\n", false)]
public delegate void PhysFsEnumFilesCallback
(
    string fileDir,
    string fileName
);
