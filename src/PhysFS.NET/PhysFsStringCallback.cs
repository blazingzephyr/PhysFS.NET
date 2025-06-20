namespace Icculus.PhysFS.NET;

/// <summary>
/// Function signature for callbacks that report strings.
/// </summary>
/// <remarks>
/// These are used to report a list of strings to an original caller, one
/// string per callback. All strings are UTF-8 encoded. Functions should not
/// try to modify or free the string's memory.<br/><br/>
/// These callbacks are used, starting in PhysicsFS 1.1, as an alternative to
/// functions that would return lists that need to be cleaned up with
/// <see cref="PhysicsFS.PHYSFS_freeList"/>. The callback means that the library
/// doesn't need to allocate an entire list and all the strings up front.<br/><br/>
/// Be aware that promises data ordering in the list versions are not
/// necessarily so in the callback versions. Check the documentation on
/// specific APIs, but strings may not be sorted as you expect.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.GetCdRomDirsCallback{T}"/><br/>
/// <seealso cref="PhysicsFS.GetSearchPathCallback{T}"/>
/// </remarks>
/// <typeparam name="T">Type of the data passed to the callback.</typeparam>
/// <param name="data">
/// User-defined data reference, passed through from the API
/// that eventually called the callback.
/// </param>
/// <param name="str">The string data about which the callback is meant to inform.</param>
public delegate void PhysFsStringCallback<T>(ref T data, string str);

/// <summary>
/// Function signature for callbacks that report strings.
/// </summary>
/// <remarks>
/// These are used to report a list of strings to an original caller, one
/// string per callback. All strings are UTF-8 encoded. Functions should not
/// try to modify or free the string's memory.<br/><br/>
/// These callbacks are used, starting in PhysicsFS 1.1, as an alternative to
/// functions that would return lists that need to be cleaned up with
/// <see cref="PhysicsFS.PHYSFS_freeList"/>. The callback means that the library
/// doesn't need to allocate an entire list and all the strings up front.<br/><br/>
/// Be aware that promises data ordering in the list versions are not
/// necessarily so in the callback versions. Check the documentation on
/// specific APIs, but strings may not be sorted as you expect.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.GetCdRomDirsCallback"/><br/>
/// <seealso cref="PhysicsFS.GetSearchPathCallback"/>
/// </remarks>
/// <param name="str">The string data about which the callback is meant to inform.</param>
public delegate void PhysFsStringCallback(string str);
