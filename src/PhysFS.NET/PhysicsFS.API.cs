
namespace Icculus.PhysFS.NET;

/// <summary>
/// A callback that triggers upon unmount of memory mounted with
/// <see cref="PhysicsFS.MountMemory"/>.
/// </summary>
/// <remarks>
/// <paramref name="ptr"/> must remain until the archive is unmounted. When the archive is
/// unmounted, the system will call this function, which will notify you that
/// the system is done with the buffer, and give you a chance to free your
/// resources. Can be <see langword="null"/>, in which case the system will make no
/// attempt to free the buffer.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PhysFsMountMemoryDel(IntPtr ptr);

/// <summary>
/// PhysicsFS
/// </summary>
/// <remarks>
/// The latest version of PhysicsFS can be found at:
///    https://icculus.org/physfs/<br/><br/>
/// PhysicsFS; a portable, flexible file i/o abstraction.<br/><br/>
/// This API gives you access to a system file system in ways superior to the
/// stdio or system i/o calls. The brief benefits:<br/><br/>
///  - It's portable.
///  - It's safe. No file access is permitted outside the specified dirs.
///  - It's flexible. Archives (.ZIP files) can be used transparently as
///     directory structures.<br/><br/>
/// With PhysicsFS, you have a single writing directory and multiple
/// directories (the "search path") for reading. You can think of this as a
/// filesystem within a filesystem. If (on Windows) you were to set the
/// writing directory to "C:\MyGame\MyWritingDirectory", then no PHYSFS calls
/// could touch anything above this directory, including the "C:\MyGame" and
/// "C:\" directories. This prevents an application's internal scripting
/// language from piddling over c:\\config.sys, for example. If you'd rather
/// give PHYSFS full access to the system's REAL file system, set the writing
/// dir to "C:\", but that's generally A Bad Thing for several reasons.<br/><br/>
/// Drive letters are hidden in PhysicsFS once you set up your initial paths.
/// The search path creates a single, hierarchical directory structure.
/// Not only does this lend itself well to general abstraction with archives,
/// it also gives better support to operating systems like MacOS and Unix.
/// Generally speaking, you shouldn't ever hardcode a drive letter; not only
/// does this hurt portability to non-Microsoft OSes, but it limits your win32
/// users to a single drive, too. Use the PhysicsFS abstraction functions and
/// allow user-defined configuration options, too. When opening a file, you
/// specify it like it was on a Unix filesystem: if you want to write to
/// "C:\MyGame\MyConfigFiles\game.cfg", then you might set the write dir to
/// "C:\MyGame" and then open "MyConfigFiles/game.cfg". This gives an
/// abstraction across all platforms. Specifying a file in this way is termed
/// "platform-independent notation" in this documentation. Specifying a
/// a filename in a form such as "C:\mydir\myfile" or
/// "MacOS hard drive:My Directory:My File" is termed "platform-dependent
/// notation". The only time you use platform-dependent notation is when
/// setting up your write directory and search path; after that, all file
/// access into those directories are done with platform-independent notation.<br/><br/>
/// All files opened for writing are opened in relation to the write directory,
/// which is the root of the writable filesystem. When opening a file for
/// reading, PhysicsFS goes through the search path. This is NOT the
/// same thing as the PATH environment variable. An application using
/// PhysicsFS specifies directories to be searched which may be actual
/// directories, or archive files that contain files and subdirectories of
/// their own. See the end of these docs for currently supported archive
/// formats.<br/><br/>
/// Once the search path is defined, you may open files for reading. If you've
/// got the following search path defined (to use a win32 example again):<br/><br/>
/// - C:\\mygame
/// - C:\\mygame\\myuserfiles
/// - D:\\mygamescdromdatafiles
/// - C:\\mygame\\installeddatafiles.zip<br/><br/>
/// Then a call to PHYSFS_openRead("textfiles/myfile.txt") (note the directory
/// separator, lack of drive letter, and lack of dir separator at the start of
/// the string; this is platform-independent notation) will check for
/// C:\\mygame\\textfiles\\myfile.txt, then
/// C:\\mygame\\myuserfiles\\textfiles\\myfile.txt, then
/// D:\\mygamescdromdatafiles\\textfiles\\myfile.txt, then, finally, for
/// textfiles\\myfile.txt inside of C:\\mygame\\installeddatafiles.zip.
/// Remember that most archive types and platform filesystems store their
/// filenames in a case-sensitive manner, so you should be careful to specify
/// it correctly.<br/><br/>
/// Files opened through PhysicsFS may NOT contain "." or ".." or ":" as dir
/// elements. Not only are these meaningless on MacOS Classic and/or Unix,
/// they are a security hole. Also, symbolic links (which can be found in
/// some archive types and directly in the filesystem on Unix platforms) are
/// NOT followed until you call <see cref="SymbolicLinksPermitted"/>. That's left
/// to your own discretion, as following a symlink can allow for access outside
/// the write dir and search paths. For portability, there is no mechanism for
/// creating new symlinks in PhysicsFS.<br/><br/>
/// The write dir is not included in the search path unless you specifically
/// add it. While you CAN change the write dir as many times as you like,
/// you should probably set it once and stick to it. Remember that your
/// program will not have permission to write in every directory on Unix and
/// NT systems.<br/><br/>
/// All files are opened in binary mode; there is no endline conversion for
/// textfiles. Other than that, PhysicsFS has some convenience functions for
/// platform-independence. There is a function to tell you the current
/// platform's dir separator ("\\" on windows, "/" on Unix, ":" on MacOS),
/// which is needed only to set up your search/write paths. There is a
/// function to tell you what CD-ROM drives contain accessible discs, and a
/// function to recommend a good search path, etc.<br/><br/>
/// A recommended order for the search path is the write dir, then the base dir,
/// then the cdrom dir, then any archives discovered. Quake 3 does something
/// like this, but moves the archives to the start of the search path. Build
/// Engine games, like Duke Nukem 3D and Blood, place the archives last, and
/// use the base dir for both searching and writing. There is a helper
/// function (<see cref="SetSaneConfig"/>) that puts together a basic configuration
/// for you, based on a few parameters. Also see the comments on
/// <see cref="GetBaseDir"/>, and <see cref="GetPrefDir"/> for info on what those
/// are and how they can help you determine an optimal search path.<br/><br/>
/// PhysicsFS 2.0 adds the concept of "mounting" archives to arbitrary points
/// in the search path. If a zipfile contains "maps/level.map" and you mount
/// that archive at "mods/mymod", then you would have to open
/// "mods/mymod/maps/level.map" to access the file, even though "mods/mymod"
/// isn't actually specified in the .zip file. Unlike the Unix mentality of
/// mounting a filesystem, "mods/mymod" doesn't actually have to exist when
/// mounting the zipfile. It's a "virtual" directory. The mounting mechanism
/// allows the developer to seperate archives in the tree and avoid trampling
/// over files when added new archives, such as including mod support in a
/// game...keeping external content on a tight leash in this manner can be of
/// utmost importance to some applications.<br/><br/>
/// PhysicsFS is mostly thread safe. The errors returned by
/// <see cref="GetLastErrorCode"/> are unique by thread, and library-state-setting
/// functions are mutex'd. For efficiency, individual file accesses are
/// not locked, so you can not safely read/write/seek/close/etc the same
/// file from two threads at the same time. Other race conditions are bugs
/// that should be reported/patched.<br/><br/>
/// While you CAN use stdio/syscall file access in a program that has PHYSFS_*
/// calls, doing so is not recommended, and you can not directly use system
/// filehandles with PhysicsFS and vice versa (but as of PhysicsFS 2.1, you can
/// wrap them in a <see cref="PhysFsIo"/> interface yourself if you wanted to).<br/><br/>
/// Note that archives need not be named as such: if you have a ZIP file and
/// rename it with a .PKG extension, the file will still be recognized as a
/// ZIP archive by PhysicsFS; the file's contents are used to determine its
/// type where possible.<br/><br/>
/// Currently supported archive types:
///  - .ZIP (pkZip/WinZip/Info-ZIP compatible)
///  - .7Z  (7zip archives)
///  - .ISO (ISO9660 files, CD-ROM images)
///  - .GRP (Build Engine groupfile archives)
///  - .PAK (Quake I/II archive format)
///  - .HOG (Descent I/II/III HOG file archives)
///  - .MVL (Descent II movielib archives)
///  - .WAD (DOOM engine archives)
///  - .BIN (Chasm: The Rift engine archives)
///  - .VDF (Gothic I/II engine archives)
///  - .SLB (Independence War archives)<br/><br/>
/// String policy for PhysicsFS 2.0 and later:<br/><br/>
/// PhysicsFS 1.0 could only deal with null-terminated ASCII strings. All high
/// ASCII chars resulted in undefined behaviour, and there was no Unicode
/// support at all. PhysicsFS 2.0 supports Unicode without breaking binary
/// compatibility with the 1.0 API by using UTF-8 encoding of all strings
/// passed in and out of the library.<br/><br/>
/// All strings passed through PhysicsFS are in null-terminated UTF-8 format.
/// This means that if all you care about is English (ASCII characters &lt;= 127)
/// then you just use regular C strings. If you care about Unicode (and you
/// should!) then you need to figure out what your platform wants, needs, and
/// offers. If you are on Windows before Win2000 and build with Unicode
/// support, your <c>TCHAR</c> strings are two bytes per character (this is called
/// "UCS-2 encoding"). Any modern Windows uses UTF-16, which is two bytes
/// per character for most characters, but some characters are four. You
/// should convert them to UTF-8 before handing them to PhysicsFS with
/// <see cref="Utf8FromUtf16"/>, which handles both UTF-16 and UCS-2. If you're
/// using Unix or Mac OS X, your <c>wchar_t</c> strings are four bytes per character
/// ("UCS-4 encoding", sometimes called "UTF-32"). Use PHYSFS_utf8FromUcs4().
/// Mac OS X can give you UTF-8 directly from a CFString or NSString, and many
/// Unixes generally give you C strings in UTF-8 format everywhere. If you
/// have a single-byte high ASCII charset, like so-many European "codepages"
/// you may be out of luck. We'll convert from "Latin1" to UTF-8 only, and
/// never back to Latin1. If you're above ASCII 127, all bets are off: move
/// to Unicode or use your platform's facilities. Passing a C string with
/// high-ASCII data that isn't UTF-8 encoded will NOT do what you expect!<br/><br/>
/// Naturally, there's also <see cref="Utf8ToUcs2"/>, <see cref="Utf8ToUtf16"/>, and
/// <see cref="Utf8ToUcs4"/> to get data back into a format you like. Behind the
/// scenes, PhysicsFS will use Unicode where possible: the UTF-8 strings on
/// Windows will be converted and used with the multibyte Windows APIs, for
/// example.<br/><br/>
/// PhysicsFS offers basic encoding conversion support, but not a whole string
/// library. Get your stuff into whatever format you can work with.<br/><br/>
/// Most platforms supported by PhysicsFS 2.1 and later fully support Unicode.
/// Some older platforms have been dropped (Windows 95, Mac OS 9). Some, like
/// OS/2, might be able to convert to a local codepage or will just fail to
/// open/create the file. Modern OSes (macOS, Linux, Windows, etc) should all
/// be fine.<br/><br/>
/// Many game-specific archivers are seriously unprepared for Unicode (the
/// Descent HOG/MVL and Build Engine GRP archivers, for example, only offer a
/// DOS 8.3 filename, for example). Nothing can be done for these, but they
/// tend to be legacy formats for existing content that was all ASCII (and
/// thus, valid UTF-8) anyhow. Other formats, like .ZIP, don't explicitly
/// offer Unicode support, but unofficially expect filenames to be UTF-8
/// encoded, and thus Just Work. Most everything does the right thing without
/// bothering you, but it's good to be aware of these nuances in case they
/// don't.<br/><br/><br/><br/>
/// Other stuff:<br/><br/>
/// Please see the file LICENSE.txt in the source's root directory for
/// licensing and redistribution rights.<br/><br/>
/// Please see the file CREDITS.txt in the source's "docs" directory for
/// a more or less complete list of who's responsible for this.<br/><br/>
/// Ryan C. Gordon.
/// </remarks>
public static partial class PhysicsFS
{
    /// <summary>
    /// Platform-dependent dir separator string.
    /// </summary>
    /// <remarks>
    /// This returns "\\" on win32, "/" on Unix, and ":" on MacOS. It may be more
    /// than one character, depending on the platform, and your code should take
    /// that into account. Note that this is only useful for setting up the
    /// search/write paths, since access into those dirs always use '/'
    /// (platform-independent notation) to separate directories. This is also
    /// handy for getting platform-independent access when using stdio calls.
    /// </remarks>
    public static readonly string DirSeparator = GetDirSeparator();

    /// <summary>
    /// Get the version of PhysicsFS that is linked against your program.
    /// </summary>
    /// <remarks>
    /// If you are using a shared library (DLL) version of PhysFS, then it is
    /// possible that it will be different than the version you compiled against.<br/><br/>
    /// This may be called safely at any time, even before
    /// <see cref="Init"/>.
    /// </remarks>
    public static readonly Version LinkedVersion = GetLinkedVersion();

    /// <summary>
    /// Get the path where the application resides.
    /// </summary>
    /// <remarks>
    /// Get the "base dir". This is the directory where the application was run
    /// from, which is probably the installation directory, and may or may not
    /// be the process's current working directory.<br/><br/>
    /// You should probably use the base dir in your search path.<br/><br/>
    /// On most platforms, this is a directory; on Android, this gives
    /// you the path to the app's package (APK) file. As APK files are
    /// just .zip files, you can mount them in PhysicsFS like regular
    /// directories. You'll probably want to call
    /// <c>PhysicsFS.SetRoot(<see cref="BaseDir"/>, "/assets")</c>
    /// after mounting to make your
    /// app's actual data available directly without all the Android
    /// metadata and directory offset.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetPrefDir"/>
    /// </remarks>
    public static string? BaseDir => GetBaseDir();

    /// <summary>
    /// Determine if the PhysicsFS library is initialized.
    /// </summary>
    /// <remarks>
    /// Once <see cref="Init"/> returns successfully,
    /// this will return <see langword="true"/>.
    /// Before a successful <see cref="Init"/> and after
    /// <see cref="Deinit"/> returns successfully,
    /// this will return <see langword="false"/>.
    /// This function is safe to call at any time.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Init"/><br/>
    /// <seealso cref="Deinit"/>
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if library is initialized,
    /// <see langword="false"/> if library is not.
    /// </returns>
    public static bool IsInit => PHYSFS_isInit() != 0;

    /// <summary>
    /// Determine if the symbolic links are permitted.
    /// </summary>
    /// <remarks>
    /// Some physical filesystems and archives contain files that are just pointers
    /// to other files. On the physical filesystem, opening such a link will
    /// (transparently) open the file that is pointed to.<br/><br/>
    /// By default, PhysicsFS will check if a file is really a symlink during open
    /// calls and fail if it is. Otherwise, the link could take you outside the
    /// write and search paths, and compromise security.<br/><br/>
    /// If you want to take that risk, set this to <see langword="true"/>.
    /// Note that this is more for sandboxing a program's scripting language, in
    /// case untrusted scripts try to compromise the system. Generally speaking,
    /// a user could very well have a legitimate reason to set up a symlink, so
    /// unless you feel there's a specific danger in allowing them, you should
    /// permit them.<br/><br/>
    /// Symlinks are only explicitly checked when dealing with filenames
    /// in platform-independent notation. That is, when setting up your
    /// search and write paths, etc, symlinks are never checked for.<br/><br/>
    /// Please note that <see cref="Stat"/> will always check the path specified; if
    /// that path is a symlink, it will not be followed in any case. If symlinks
    /// aren't permitted through this function, <see cref="Stat"/> ignores them, and
    /// would treat the query as if the path didn't exist at all.<br/><br/>
    /// Symbolic link permission can be enabled or disabled at any time after
    /// you've called <see cref="Init"/>, and is disabled by default.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if symlinks are permitted, <see langword="false"/> if not.
    /// </returns>
    public static bool SymbolicLinksPermitted
    {
        get => PHYSFS_symbolicLinksPermitted() != 0;
        set => PHYSFS_permitSymbolicLinks(value ? 1 : 0);
    }

    /// <summary>
    /// Add an archive or directory to the search path.
    /// </summary>
    /// <remarks>
    /// This function is equivalent to:<br/><br/>
    /// <code>
    /// PhysicsFS.Mount(newDir, null, appendToPath);
    /// </code>
    /// You must use this and not <see cref="Mount"/> if binary compatibility with
    /// PhysicsFS 1.0 is important (which it may not be for many people).<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Mount"/><br/>
    /// <seealso cref="RemoveFromSearchPath"/><br/>
    /// <seealso cref="GetSearchPath"/>
    /// </remarks>
    [Obsolete("As of PhysicsFS 2.0, use PhysicsFS.Mount() instead. This " +
            "function just wraps it anyhow.", false)]
    public static partial void AddToSearchPath(string newDir, bool appendToPath);

    /// <summary>
    /// "Fold" a Unicode codepoint to a lowercase equivalent.
    /// </summary>
    /// <remarks>
    /// (This is for limited, hardcore use. If you don't immediately see a need
    /// for it, you can probably ignore this forever.)<br/><br/>
    /// This will convert a Unicode codepoint into its lowercase equivalent.
    /// Bogus codepoints and codepoints without a lowercase equivalent will
    /// be returned unconverted.<br/><br/>
    /// Note that you might get multiple codepoints in return! The German Eszett,
    /// for example, will fold down to two lowercase latin 's' codepoints. The
    /// theory is that if you fold two strings, one with an Eszett and one with
    /// "SS" down, they will match.<br/><br/>
    /// Anyone that is a student of Unicode knows about the "Turkish I"
    /// problem. This API does not handle it. Assume this one letter
    /// in all of Unicode will definitely fold sort of incorrectly. If
    /// you don't know what this is about, you can probably ignore this
    /// problem for most of the planet, but perfection is impossible.
    /// </remarks>
    /// <param name="from">The codepoint to fold.</param>
    /// <returns>The codepoints the folding produced. Between 1 and 3.</returns>
    public static partial char[] CaseFold(char from);

    /// <summary>
    /// Deinitialize the PhysicsFS library.
    /// </summary>
    /// <remarks>
    /// This closes any files opened via PhysicsFS, blanks the search/write paths,
    /// frees memory, and invalidates all of your file handles.<br/><br/>
    /// Note that this call can FAIL if there's a file open for writing that
    /// refuses to close (for example, the underlying operating system was
    /// buffering writes to network filesystem, and the fileserver has crashed,
    /// or a hard drive has failed, etc). It is usually best to close all write
    /// handles yourself before calling this function, so that you can gracefully
    /// handle a specific failure.<br/><br/>
    /// Once successfully deinitialized, <see cref="Init"/> can be called again to
    /// restart the subsystem. All default API states are restored at this
    /// point, with the exception of any custom allocator you might have
    /// specified, which survives between initializations.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Init"/><br/>
    /// <seealso cref="IsInit"/>
    /// </remarks>
    public static partial void Deinit();

    /// <summary>
    /// Delete a file or directory.
    /// </summary>
    /// <remarks>
    /// <paramref name="fileName"/>
    /// is specified in platform-independent notation in relation to the
    /// write dir.<br/><br/>
    /// A directory must be empty before this call can delete it.<br/><br/>
    /// Deleting a symlink will remove the link, not what it points to, regardless
    /// of whether you <see cref="SymbolicLinksPermitted"/> or not.<br/><br/>
    /// So if you've got the write dir set to "C:\mygame\writedir" and call
    /// <c>PhysicsFS.Delete("downloads/maps/level1.map")</c> then the file
    /// "C:\mygame\writedir\downloads\maps\level1.map" is removed from the
    /// physical filesystem, if it exists and the operating system permits the
    /// deletion.<br/><br/>
    /// Note that on Unix systems, deleting a file may be successful, but the
    /// actual file won't be removed until all processes that have an open
    /// filehandle to it (including your program) close their handles.<br/><br/>
    /// Chances are, the bits that make up the file still exist, they are just
    /// made available to be written over at a later point. Don't consider this
    /// a security method or anything.  :)
    /// </remarks>
    /// <param name="fileName">Filename to delete.</param>
    public static partial void Delete(string fileName);

    /// <summary>
    /// Remove an archiver from the system.
    /// </summary>
    /// <remarks>
    /// If for some reason, you only need your previously-registered archiver to
    /// live for a portion of your app's lifetime, you can remove it from the
    /// system once you're done with it through this function.<br/><br/>
    /// This fails if there are any archives still open that use this archiver.<br/><br/>
    /// This function can also remove internally-supplied archivers, like .zip
    /// support or whatnot. This could be useful in some situations, like
    /// disabling support for them outright or overriding them with your own
    /// implementation. Once an internal archiver is disabled like this,
    /// PhysicsFS provides no mechanism to recover them, short of calling
    /// <see cref="Deinit"/> and <see cref="Init"/> again.<br/><br/>
    /// <see cref="Deinit"/> will automatically deregister all archivers, so you don't
    /// need to explicitly deregister yours if you otherwise shut down cleanly.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsArchiver"/><br/>
    /// <seealso cref="RegisterArchiver"/>
    /// </remarks>
    /// <param name="fileExt">Filename extension that the archiver handles.</param>
    public static partial void DeregisterArchiver(string fileExt);

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined
    /// callback, with errors reported.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="EnumerateFiles"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="EnumerateFiles"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// element of the search path:<br/><br/>
    /// <code><br/><br/>
    /// static PhysFsEnumerateCallbackResult printDir(ref int data, string fileDir, string fileName)
    /// {
    ///     Console.WriteLine(" * We've got {0} in {1}.", fileDir, fileName);
    ///     return PhysFsEnumerateCallbackResult.OK;  // give me more data, please. 
    /// }
    /// // ...
    /// int data = 5;
    /// PhysicsFS.Enumerate("/some/path", printDir, ref data);
    /// </code><br/>
    /// Items sent to the callback are not guaranteed to be in any order whatsoever.
    /// There is no sorting done at this level, and if you need that, you should
    /// probably use <see cref="EnumerateFiles"/> instead, which guarantees
    /// alphabetical sorting. This form reports whatever is discovered in each
    /// archive before moving on to the next. Even within one archive, we can't
    /// guarantee what order it will discover data. <em>Any sorting you find in
    /// these callbacks is just pure luck. Do not rely on it.</em> As this walks
    /// the entire list of archives, you may receive duplicate filenames.<br/><br/>
    /// This API and the callbacks themselves are capable of reporting errors.
    /// Prior to this API, callbacks had to accept every enumerated item, even if
    /// they were only looking for a specific thing and wanted to stop after that,
    /// or had a serious error and couldn't alert anyone. Furthermore, if
    /// PhysicsFS itself had a problem (disk error or whatnot), it couldn't report
    /// it to the calling app, it would just have to skip items or stop
    /// enumerating outright, and the caller wouldn't know it had lost some data
    /// along the way.<br/><br/>
    /// Now the caller can be sure it got a complete data set, and its callback has
    /// control if it wants enumeration to stop early. See the documentation for
    /// <see cref="PhysFsEnumerateCallback{T}"/> for details on how your callback
    /// should behave.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsEnumerateCallback{T}"/><br/>
    /// <seealso cref="EnumerateFiles"/>
    /// </remarks>
    /// <typeparam name="T">Type of the data passed to the callback</typeparam>
    /// <param name="directory">Directory, in platform-independent notation, to enumerate.</param>
    /// <param name="callback">Callback function to notify about search path elements.</param>
    /// <param name="data">Application-defined data passed to callback.</param>
    public static partial void Enumerate<T>(string directory, PhysFsEnumerateCallback<T> callback, ref T data);

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined
    /// callback, with errors reported.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="EnumerateFiles"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="EnumerateFiles"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// element of the search path:<br/><br/>
    /// <code><br/><br/>
    /// static PhysFsEnumerateCallbackResult printDir(string fileDir, string fileName)
    /// {
    ///     Console.WriteLine(" * We've got {0} in {1}.", fileDir, fileName);
    ///     return PhysFsEnumerateCallbackResult.OK;  // give me more data, please. 
    /// }
    /// // ...
    /// PhysicsFS.Enumerate("/some/path", printDir);
    /// </code><br/>
    /// Items sent to the callback are not guaranteed to be in any order whatsoever.
    /// There is no sorting done at this level, and if you need that, you should
    /// probably use <see cref="EnumerateFiles"/> instead, which guarantees
    /// alphabetical sorting. This form reports whatever is discovered in each
    /// archive before moving on to the next. Even within one archive, we can't
    /// guarantee what order it will discover data. <em>Any sorting you find in
    /// these callbacks is just pure luck. Do not rely on it.</em> As this walks
    /// the entire list of archives, you may receive duplicate filenames.<br/><br/>
    /// This API and the callbacks themselves are capable of reporting errors.
    /// Prior to this API, callbacks had to accept every enumerated item, even if
    /// they were only looking for a specific thing and wanted to stop after that,
    /// or had a serious error and couldn't alert anyone. Furthermore, if
    /// PhysicsFS itself had a problem (disk error or whatnot), it couldn't report
    /// it to the calling app, it would just have to skip items or stop
    /// enumerating outright, and the caller wouldn't know it had lost some data
    /// along the way.<br/><br/>
    /// Now the caller can be sure it got a complete data set, and its callback has
    /// control if it wants enumeration to stop early. See the documentation for
    /// <see cref="PhysFsEnumerateCallback"/> for details on how your callback
    /// should behave.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsEnumerateCallback"/><br/>
    /// <seealso cref="EnumerateFiles(string)"/>
    /// </remarks>
    /// <param name="directory">Directory, in platform-independent notation, to enumerate.</param>
    /// <param name="callback">Callback function to notify about search path elements.</param>
    public static partial void Enumerate(string directory, PhysFsEnumerateCallback callback);

    /// <summary>
    /// Get a file listing of a search path's directory.
    /// </summary>
    /// <remarks>
    /// In PhysicsFS versions prior to 2.1, this function would return
    /// as many items as it could in the face of a failure condition
    /// (out of memory, disk i/o error, etc). Since this meant apps
    /// couldn't distinguish between complete success and partial failure,
    /// and since the function could always return IntPtr.Zero to report
    /// catastrophic failures anyway, in PhysicsFS 2.1 this function's
    /// policy changed: it will either return a list of complete results
    /// or it will throw an exception for any failure of any kind, so we can
    /// guarantee that the enumeration ran to completion and has no gaps
    /// in its results.<br/><br/>
    /// Matching directories are interpolated. That is, if "C:\mydir" is in the
    /// search path and contains a directory "savegames" that contains "x.sav",
    /// "y.sav", and "z.sav", and there is also a "C:\userdir" in the search path
    /// that has a "savegames" subdirectory with "w.sav", then the following code:<br/><br/>
    /// <code>
    /// IEnumerable&lt;string&gt; rc = PhysicsFS.EnumerateFiles("savegames");
    /// foreach (string i in rc)
    /// {
    ///     Console.WriteLine(" * We've got {0}.", i);
    /// }
    /// </code><br/><br/><br/>
    /// ...will print:<br/><br/>
    /// <code>
    /// We've got [x.sav].<br/>
    /// We've got [y.sav].<br/>
    /// We've got [z.sav].<br/>
    /// We've got [w.sav].
    /// </code>
    /// Feel free to sort the list however you like. However, the returned data
    /// will always contain no duplicates, and will be always sorted in alphabetic
    /// (rather: case-sensitive Unicode) order for you.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Enumerate"/><br/>
    /// <seealso cref="Enumerate{T}"/><br/>
    /// </remarks>
    /// <param name="directory">directory in platform-independent notation to enumerate.</param>
    /// <returns>Null-terminated strings.</returns>
    public static partial IEnumerable<string> EnumerateFiles(string directory);

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// As of PhysicsFS 2.1, this function just wraps
    /// <see cref="Enumerate{T}"/> and ignores errors. Consider using
    /// <see cref="Enumerate{T}"/> or <see cref="EnumerateFiles"/>
    /// instead.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Enumerate{T}"/><br/>
    /// <seealso cref="Enumerate"/><br/>
    /// <seealso cref="EnumerateFiles"/><br/>
    /// <seealso cref="PhysFsEnumFilesCallback{T}"/>
    /// </remarks>
    /// <typeparam name="T">Type of the data passed to the callback.</typeparam>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Enumerate() instead. This" +
            "function has no way to report errors (or to have the callback signal an" +
            "error or request a stop), so if data will be lost, your callback has no" +
            "way to direct the process, and your calling app has no way to know.", false)]
    public static partial void EnumerateFilesCallback<T>(
        string directory,
        PhysFsEnumFilesCallback<T> callback,
        ref T data);

    /// <summary>
    /// Get a file listing of a search path's directory, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// As of PhysicsFS 2.1, this function just wraps
    /// <see cref="Enumerate"/> and ignores errors. Consider using
    /// <see cref="Enumerate"/> or <see cref="EnumerateFiles"/>
    /// instead.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Enumerate"/><br/>
    /// <seealso cref="Enumerate"/><br/>
    /// <seealso cref="EnumerateFiles"/><br/>
    /// <seealso cref="PhysFsEnumFilesCallback"/>
    /// </remarks>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Enumerate() instead. This" +
            "function has no way to report errors (or to have the callback signal an" +
            "error or request a stop), so if data will be lost, your callback has no" +
            "way to direct the process, and your calling app has no way to know.", false)]
    public static partial void EnumerateFilesCallback(string directory, PhysFsEnumFilesCallback callback);

    /// <summary>
    /// Check for end-of-file state on a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// Determine if the end of file has been reached in a PhysicsFS filehandle.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Read"/><br/>
    /// <seealso cref="ReadBytes"/><br/>
    /// <seealso cref="Tell"/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead"/>, <see cref="OpenWrite"/>
    /// or <see cref="OpenAppend"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if EOF, <see langword="false"/> if not.
    /// </returns>
    public static partial bool EOF(PhysFsFileHandle handle);

    /// <summary>
    /// Determine if a file exists in the search path.
    /// </summary>
    /// <remarks>
    /// Reports true if there is an entry anywhere in the search path by the
    /// name of <paramref name="fileName"/>.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> is not true, so you
    /// might end up further down in the search path than expected.
    /// </remarks>
    /// <param name="fileName">filename in platform-independent notation.</param>
    /// <returns>true if filename exists. false otherwise.</returns>
    public static partial bool Exists(string fileName);

    /// <summary>
    /// Get total length of a file in bytes.
    /// </summary>
    /// <remarks>
    /// Note that if another process/thread is writing to this file at the same
    /// time, then the information this function supplies could be incorrect
    /// before you get it. Use with caution, or better yet, don't use at all.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Tell"/><br/>
    /// <seealso cref="Seek"/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead"/>, <see cref="OpenWrite"/>
    /// or <see cref="OpenAppend"/>.
    /// </param>
    /// <returns>size in bytes of the file. -1 if can't be determined.</returns>
    public static partial long FileLength(PhysFsFileHandle handle);

    /// <summary>
    /// Flush a buffered PhysicsFS file handle.
    /// </summary>
    /// <remarks>
    /// For buffered files opened for writing, this will put the current contents
    /// of the buffer to disk and flag the buffer as empty if possible.<br/><br/>
    /// For buffered files opened for reading or unbuffered files, this is a safe
    /// no-op, and will report success.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="SetBuffer"/><br/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead(string)"/>, <see cref="OpenWrite(string)"/>
    /// or <see cref="OpenAppend(string)"/>.
    /// </param>
    public static partial void Flush(PhysFsFileHandle handle);

    /// <summary>
    /// Discover the current allocator.
    /// </summary>
    /// <remarks>
    /// (This is for limited, hardcore use. If you don't immediately see a need
    /// for it, you can probably ignore this forever.)<br/><br/>
    /// This function exposes the function pointers that make up the currently used
    /// allocator. This can be useful for apps that want to access PhysicsFS's
    /// internal, default allocation routines, as well as for external code that
    /// wants to share the same allocator, even if the application specified their
    /// own.<br/><br/>
    /// This call is only valid between <see cref="Init(string?)"/>
    /// and <see cref="Deinit"/> calls; it will return
    /// <see langword="null"/> if the library isn't initialized. As we can't
    /// guarantee the state of the internal allocators unless the library is
    /// initialized, you shouldn't use any allocator returned here after a call
    /// to <see cref="Deinit"/>.<br/><br/>
    /// Do not call the returned allocator's Init() or Deinit() methods under any
    /// circumstances.<br/><br/>
    /// If you aren't immediately sure what to do with this function, you can
    /// safely ignore it altogether.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsAllocator"/><br/>
    /// <seealso cref="SetAllocator"/>
    /// </remarks>
    /// <returns>
    /// Current allocator, as set by <see cref="SetAllocator"/>,
    /// or PhysicsFS's internal, default allocator if no application defined allocator
    /// is currently set. Will return <see langword="null"/> if the library is not
    /// initialized.
    /// </returns>
    public static partial NullableRef<PhysFsAllocator.AllocatorHandle> GetAllocator();

    /// <summary>
    /// Get an array of paths to available CD-ROM drives.
    /// </summary>
    /// <remarks>
    /// The dirs returned are platform-dependent ("D:\" on Win32, "/cdrom" or
    /// whatnot on Unix). Dirs are only returned if there is a disc ready and
    /// accessible in the drive. So if you've got two drives (D: and E:), and only
    /// E: has a disc in it, then that's all you get. If the user inserts a disc
    /// in D: and you call this function again, you get both drives. If, on a
    /// Unix box, the user unmounts a disc and remounts it elsewhere, the next
    /// call to this function will reflect that change.<br/><br/>
    /// This function refers to "CD-ROM" media, but it really means "inserted disc
    /// media," such as DVD-ROM, HD-DVD, CDRW, and Blu-Ray discs. It looks for
    /// filesystems, and as such won't report an audio CD, unless there's a
    /// mounted filesystem track on it.<br/><br/>
    /// The returned value is string enumerator:<br/><br/>
    /// <code>
    /// IEnumerable&lt;string&gt; cds = PhysicsFS.GetCdRomDirs();
    /// foreach (string i in cds)
    /// {
    ///     Console.WriteLine("cdrom dir {0} is available.", i);
    /// }
    /// </code><br/>
    /// This call may block while drives spin up. Be forewarned.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetCdRomDirsCallback{T}"/><br/>
    /// <seealso cref="GetCdRomDirsCallback"/>
    /// </remarks>
    /// <returns>Null-terminated strings.</returns>
    public static partial IEnumerable<string> GetCdRomDirs();

    /// <summary>
    /// Enumerate CD-ROM directories, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="GetCdRomDirs"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="GetCdRomDirs"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// detected disc:<br/><br/>
    /// <code>
    /// static void foundDisc(ref int data, string cddir)
    /// {
    ///     Console.WriteLine("cdrom dir {0} is available.", cddir);
    /// }
    /// // ...
    /// int data = 5;
    /// PhysicsFS.GetCdRomDirsCallback(foundDisc, ref data);
    /// </code><br/>
    /// This call may block while drives spin up. Be forewarned.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsStringCallback{T}"/><br/>
    /// <seealso cref="GetCdRomDirs"/>
    /// </remarks>
    /// <typeparam name="T">Type of the data passed to the callback.</typeparam>
    /// <param name="callback">Callback function to notify about detected drives.</param>
    /// <param name="data">Application-defined data passed to callback.</param>
    public static partial void GetCdRomDirsCallback<T>(PhysFsStringCallback<T> callback, ref T data);

    /// <summary>
    /// Enumerate CD-ROM directories, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="GetCdRomDirs"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="GetCdRomDirs"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// detected disc:<br/><br/>
    /// <code>
    /// static void foundDisc(string cddir)
    /// {
    ///     Console.WriteLine("cdrom dir {0} is available.", cddir);
    /// }
    /// // ...
    /// PhysicsFS.GetCdRomDirsCallback(foundDisc);
    /// </code><br/>
    /// This call may block while drives spin up. Be forewarned.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsStringCallback"/><br/>
    /// <seealso cref="GetCdRomDirs"/>
    /// </remarks>
    /// <param name="callback">Callback function to notify about detected drives.</param>
    public static partial void GetCdRomDirsCallback(PhysFsStringCallback callback);

    /// <summary>
    /// Get human-readable description string for a given error code.
    /// </summary>
    /// <remarks>
    /// Get a static string, in UTF-8 format, that represents an English
    /// description of a given error code.<br/><br/>
    /// This string is guaranteed to never change (although we may add new strings
    /// for new error codes in later versions of PhysicsFS), so you can use it
    /// for keying a localization dictionary.<br/><br/>
    /// It is safe to call this function at anytime, even before
    /// <see cref="Init"/>.<br/><br/>
    /// These strings are meant to be passed on directly to the user.
    /// Generally, applications should only concern themselves with whether a
    /// given function failed, but not care about the specifics much.<br/><br/>
    /// Do not attempt to free the returned strings; they are read-only and you
    /// don't own their memory pages.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetLastErrorCode"/>
    /// </remarks>
    /// <param name="code">Error code to convert to a string.</param>
    /// <returns>
    /// String of requested error message, <see langword="null"/> if this
    /// is not a valid PhysicsFS error code. Always check for <see langword="null"/> if
    /// you might be looking up an error code that didn't exist in an
    /// earlier version of PhysicsFS.
    /// </returns>
    public static partial string? GetErrorByCode(PhysFsErrorCode code);

    /// <summary>
    /// Get human-readable error information.
    /// </summary>
    /// <remarks>
    /// As of PhysicsFS 2.1, this function has been nerfed.
    /// Before PhysicsFS 2.1, this function was the only way to get
    /// error details beyond a given function's basic return value.
    /// This was meant to be a human-readable string in one of several
    /// languages, and was not useful for application parsing. This was
    /// a problem, because the developer and not the user chose the
    /// language at compile time, and the PhysicsFS maintainers had
    /// to (poorly) maintain a significant amount of localization work.
    /// The app couldn't parse the strings, even if they counted on a
    /// specific language, since some were dynamically generated.
    /// In 2.1 and later, this always returns a static string in
    /// English; you may use it as a key string for your own
    /// localizations if you like, as we'll promise not to change
    /// existing error strings. Also, if your application wants to
    /// look at specific errors, we now offer a better option:
    /// use <see cref="GetLastErrorCode"/> instead.<br/><br/>
    /// Get the last PhysicsFS error message as a human-readable, null-terminated
    /// string. This will return <see langword="null"/>
    /// if there's been no error since the last call
    /// to this function. The pointer returned by this call points to an internal
    /// buffer. Each thread has a unique error state associated with it, but each
    /// time a new error message is set, it will overwrite the previous one
    /// associated with that thread. It is safe to call this function at anytime,
    /// even before <see cref="Init"/>.<br/><br/>
    /// <see cref="GetLastError"/> and <see cref="GetLastErrorCode"/> both reset the same
    /// thread-specific error state. Calling one will wipe out the other's
    /// data. If you need both, call <see cref="GetLastErrorCode"/>, then pass that
    /// value to <see cref="GetErrorByCode"/>.<br/><br/>
    /// As of PhysicsFS 2.1, this function only presents text in the English
    /// language, but the strings are static, so you can use them as keys into
    /// your own localization dictionary. These strings are meant to be passed on
    /// directly to the user.<br/><br/>
    /// Generally, applications should only concern themselves with whether a
    /// given function failed; however, if your code require more specifics, you
    /// should use <see cref="GetLastErrorCode"/> instead of this function.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetLastErrorCode"/><br/>
    /// <seealso cref="GetErrorByCode"/>
    /// </remarks>
    /// <returns>String of last error message.</returns>
    [Obsolete("Use PhysicsFS.GetLastErrorCode() and PhysicsFS.GetErrorByCode() instead.", false)]
    public static partial string? GetLastError();

    /// <summary>
    /// Get machine-readable error information.
    /// </summary>
    /// <remarks>
    /// Get the last PhysicsFS error message as a enumeration value. This will return
    /// <see cref="PhysFsErrorCode.PHYSFS_ERR_OK"/> if there's been no error
    /// since the last call to this function.
    /// Each thread has a unique error state associated with it, but
    /// each time a new error message is set, it will overwrite the previous one
    /// associated with that thread. It is safe to call this function at anytime,
    /// even before <see cref="Init"/>.<br/><br/>
    /// <see cref="GetLastError"/> and <see cref="GetLastErrorCode"/> both reset the same
    /// thread-specific error state. Calling one will wipe out the other's
    /// data. If you need both, call <see cref="GetLastErrorCode"/>, then pass that
    /// value to <see cref="GetErrorByCode"/>.<br/><br/>
    /// Generally, applications should only concern themselves with whether a
    /// given function failed; however, if you require more specifics, you can
    /// try this function to glean information, if there's some specific problem
    /// you're expecting and plan to handle. But with most things that involve
    /// file systems, the best course of action is usually to give up, report the
    /// problem to the user, and let them figure out what should be done about it.
    /// For that, you might prefer <see cref="GetErrorByCode"/>
    /// instead.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetErrorByCode"/>
    /// </remarks>
    /// <returns>Enumeration value that represents last reported error.</returns>
    public static partial PhysFsErrorCode GetLastErrorCode();

    /// <summary>
    /// Get the last modification time of a file.
    /// </summary>
    /// <remarks>
    /// The modtime is returned as a number of seconds since the Unix epoch
    /// (midnight, Jan 1, 1970). The exact derivation and accuracy of this time
    /// depends on the particular archiver. If there is no reasonable way to
    /// obtain this information for a particular archiver, or there was some sort
    /// of error, this function returns (-1).<br/><br/>
    /// You must use this and not <see cref="Stat"/> if binary compatibility with
    /// PhysicsFS 2.0 is important (which it may not be for many people).<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Stat"/>
    /// </remarks>
    /// <param name="fileName">filename to check, in platform-independent notation.</param>
    /// <returns>
    /// Last modified time of the file.
    /// -1 to Unix if it can't be determined.
    /// </returns>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Stat() instead. This" +
            "function just wraps it anyhow.", false)]
    public static partial DateTime GetLastModTime(string fileName);

    /// <summary>
    /// Determine a mounted archive's mountpoint.
    /// </summary>
    /// <remarks>
    /// You give this function the name of an archive or dir you successfully
    /// added to the search path, and it reports the location in the interpolated
    /// tree where it is mounted. Files mounted with a <see langword="null"/>
    /// mountpoint or through <see cref="AddToSearchPath"/> will report "/".
    /// The return value is valid until the archive is removed from the search path.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="RemoveFromSearchPath"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// </remarks>
    /// <param name="directory">
    /// directory or archive previously added to the path, in
    /// platform-dependent notation. This must match the string
    /// used when adding, even if your string would also reference
    /// the same file with a different string of characters.
    /// </param>
    /// <returns>String of mount point if added to path.</returns>
    public static partial string GetMountPoint(string directory);

    /// <summary>
    /// Get the user-and-app-specific path where files can be written.
    /// </summary>
    /// <remarks>
    /// Helper function.<br/><br/>
    /// Get the "pref dir". This is meant to be where users can write personal
    /// files (preferences and save games, etc) that are specific to your
    /// application. This directory is unique per user, per application.<br/><br/>
    /// This function will decide the appropriate location in the native filesystem,
    /// create the directory if necessary, and return a string in
    /// platform-dependent notation, suitable for passing to PHYSFS_setWriteDir().<br/><br/>
    /// On Windows, this might look like:
    /// "C:\\Users\\bob\\AppData\\Roaming\\My Company\\My Program Name"<br/><br/>
    /// On Linux, this might look like:
    /// "/home/bob/.local/share/My Program Name"<br/><br/>
    /// On Mac OS X, this might look like:
    /// "/Users/bob/Library/Application Support/My Program Name"<br/><br/>
    /// (etc.)<br/><br/>
    /// You should probably use the pref dir for your write dir, and also put it
    /// near the beginning of your search path. Older versions of PhysicsFS
    /// offered only <see cref="GetUserDir"/> and left you to figure out where the
    /// files should go under that tree. This finds the correct location
    /// for whatever platform, which not only changes between operating systems,
    /// but also versions of the same operating system.<br/><br/>
    /// You specify the name of your organization (if it's not a real organization,
    /// your name or an Internet domain you own might do) and the name of your
    /// application. These should be proper names.<br/><br/>
    /// Both the <paramref name="organization"/> and <paramref name="app"/>
    /// strings may become part of a directory name, so
    /// please follow these rules:<br/><br/>
    ///   - Try to use the same org string (including case-sensitivity) for
    ///     all your applications that use this function.
    ///   - Always use a unique app string for each one, and make sure it never
    ///     changes for an app once you've decided on it.
    ///   - Unicode characters are legal, as long as it's UTF-8 encoded, but...
    ///   - ...only use letters, numbers, and spaces. Avoid punctuation like
    ///     "Game Name 2: Bad Guy's Revenge!" ... "Game Name 2" is sufficient.<br/><br/>
    /// The pointer returned by this function remains valid until you call this
    /// function again, or call <see cref="Deinit"/>. This is not necessarily a fast
    /// call, though, so you should call this once at startup and copy the string
    /// if you need it.<br/><br/>
    /// You should assume the path returned by this function is the only safe
    /// place to write files (and that <see cref="GetUserDir"/> and <see cref="GetBaseDir"/>,
    /// while they might be writable, or even parents of the returned path, aren't
    /// where you should be writing things).<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetBaseDir"/><br/>
    /// <seealso cref="GetUserDir"/>
    /// </remarks>
    /// <param name="organization">The name of your organization.</param>
    /// <param name="app">The name of your application.</param>
    /// <returns>String of user dir in platform-dependent notation.</returns>
    public static partial string GetPrefDir(string organization, string app);

    /// <summary>
    /// Figure out where in the search path a file resides.
    /// </summary>
    /// <remarks>
    /// The file is specified in platform-independent notation. The returned
    /// filename will be the element of the search path where the file was found,
    /// which may be a directory, or an archive. Even if there are multiple
    /// matches in different parts of the search path, only the first one found
    /// is used, just like when opening a file.<br/><br/>
    /// So, if you look for "maps/level1.map", and C:\\mygame is in your search
    /// path and C:\\mygame\\maps\\level1.map exists, then "C:\mygame" is returned.<br/><br/>
    /// If a any part of a match is a symbolic link, and you've not explicitly
    /// permitted symlinks, then it will be ignored, and the search for a match
    /// will continue.<br/><br/>
    /// If you specify a fake directory that only exists as a mount point, it'll
    /// be associated with the first archive mounted there, even though that
    /// directory isn't necessarily contained in a real archive.<br/><br/>
    /// This will return <see langword="null"/> if there is no real directory associated
    /// with <paramref name="fileName"/>. Specifically, <see cref="MountIo"/>,
    /// <see cref="MountMemory"/>, and <see cref="MountHandle"/> will return
    /// <see langword="null"/>
    /// even if the filename is found in the search path. Plan accordingly.
    /// </remarks>
    /// <param name="fileName">file to look for.</param>
    /// <returns>
    /// String of element of search path containing the
    ///  the file in question. <see langword="null"/> if not found.
    /// </returns>
    public static partial string? GetRealDir(string fileName);

    /// <summary>
    /// Get the current search path.
    /// </summary>
    /// <remarks>
    /// The default search path is an empty list.<br/><br/>
    /// The returned value is a enumeration of strings:<br/>
    /// <code>
    /// IEnumerable&lt;string&gt; paths = PhysicsFS.GetSearchPath();
    /// foreach (string i in paths)
    /// {
    ///     Console.WriteLine("{0} is in the search path.", i);
    /// }
    /// </code><br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetSearchPathCallback{T}(PhysFsStringCallback{T}, ref T)"/><br/>
    /// <seealso cref="GetSearchPathCallback(PhysFsStringCallback)"/><br/>
    /// <seealso cref="AddToSearchPath(string, bool)"/><br/>
    /// <seealso cref="RemoveFromSearchPath"/>
    /// </remarks>
    /// <returns>Null-terminated strings.</returns>
    public static partial IEnumerable<string> GetSearchPath();

    /// <summary>
    /// Enumerate the search path, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="GetSearchPath"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="GetSearchPath"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// element of the search path:<br/><br/>
    /// <code><br/><br/>
    /// static void printSearchPath(ref int data, string pathItem)
    /// {
    ///     Console.WriteLine("{0} is in the search path.", pathItem);
    /// }
    /// // ..
    /// int data = 5;
    /// PhysicsFS.GetSearchPathCallback(printSearchPath, ref data);
    /// </code><br/><br/>
    /// Elements of the search path are reported in order search priority, so the
    /// first archive/dir that would be examined when looking for a file is the
    /// first element passed through the callback.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsStringCallback{T}"/><br/>
    /// <seealso cref="GetSearchPath"/>
    /// </remarks>
    /// <param name="callback">Callback function to notify about search path elements.</param>
    /// <param name="data">Application-defined data passed to callback.</param>
    public static partial void GetSearchPathCallback<T>(PhysFsStringCallback<T> callback, ref T data);

    /// <summary>
    /// Enumerate the search path, using an application-defined callback.
    /// </summary>
    /// <remarks>
    /// Internally, <see cref="GetSearchPath"/> just calls this function and then builds
    /// a list before returning to the application, so functionality is identical
    /// except for how the information is represented to the application.<br/><br/>
    /// Unlike <see cref="GetSearchPath"/>, this function does not return an array.
    /// Rather, it calls a function specified by the application once per
    /// element of the search path:<br/><br/>
    /// <code><br/><br/>
    /// static void printSearchPath(string pathItem)
    /// {
    ///     Console.WriteLine("{0} is in the search path.", pathItem);
    /// }
    /// // ..
    /// PhysicsFS.GetSearchPathCallback(printSearchPath);
    /// </code><br/><br/>
    /// Elements of the search path are reported in order search priority, so the
    /// first archive/dir that would be examined when looking for a file is the
    /// first element passed through the callback.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsStringCallback"/><br/>
    /// <seealso cref="GetSearchPath"/>
    /// </remarks>
    /// <param name="callback">Callback function to notify about search path elements.</param>
    public static partial void GetSearchPathCallback(PhysFsStringCallback callback);

    /// <summary>
    /// Get the path where user's home directory resides.
    /// </summary>
    /// <remarks>
    /// Helper function.<br/><br/>
    /// Get the "user dir". This is meant to be a suggestion of where a specific
    /// user of the system can store files. On Unix, this is her home directory.
    /// On systems with no concept of multiple home directories (MacOS, win95),
    /// this will default to something like "C:\mybasedir\users\username"
    /// where "username" will either be the login name, or "default" if the
    /// platform doesn't support multiple users, either.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetBaseDir"/><br/>
    /// <seealso cref="GetPrefDir"/>
    /// </remarks>
    /// <returns>String of user dir in platform-dependent notation.</returns>
    [Obsolete("As of PhysicsFS 2.1, you probably want PhysicsFS.GetPrefDir().", false)]
    public static partial string GetUserDir();

    /// <summary>
    /// Get path where PhysicsFS will allow file writing.
    /// </summary>
    /// <remarks>
    /// Get the current write dir. The default write dir is <see langword="null"/>.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="SetWriteDir"/>
    /// </remarks>
    /// <returns>
    /// String of write dir in platform-dependent notation,
    /// OR <see langword="null"/> IF NO WRITE PATH IS CURRENTLY SET.
    /// </returns>
    public static partial string? GetWriteDir();

    /// <summary>
    /// Initialize the PhysicsFS library.
    /// </summary>
    /// <remarks>
    /// This must be called before any other PhysicsFS function.<br/><br/>
    /// This should be called prior to any attempts to change your process's
    /// current working directory.<br/><br/>
    /// On Android, argv0 should be omitted as PhysFS.NET handles
    /// initialization of PhysicsFS, passing the JNIEnv handle and 
    /// application Context handle to the underlying native API. PhysicsFS
    /// uses these objects to query some system details. PhysicsFS does
    /// not hold a reference to the JNIEnv or Context past the call to
    /// <see cref="Init"/>. If the passed handles are incorrect,
    /// <see cref="Init"/> can still succeed, but
    /// <see cref="BaseDir"/> and <see cref="GetPrefDir"/>
    /// will be incorrect.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Deinit"/><br/>
    /// <seealso cref="IsInit"/>
    /// </remarks>
    /// <param name="argv0">
    /// the argv[0] string passed to your program's mainline.
    /// This may be <see langword="null"/> on most platforms (such as ones without a
    /// standard main() function), but you should always try to pass
    /// something in here. Many Unix-like systems _need_ to pass argv[0]
    /// from main() in here.
    /// </param>
    public static partial void Init(string? argv0 = null);

    /// <summary>
    /// Determine if a file in the search path is really a directory.
    /// </summary>
    /// <remarks>
    /// Determine if the first occurence of <paramref name="fileName"/>
    /// in the search path is really a directory entry.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> hasn't been set, so you
    /// might end up further down in the search path than expected.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Stat"/><br/>
    /// <seealso cref="Exists"/>
    /// </remarks>
    /// <param name="fileName">filename in platform-independent notation.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="fileName"/> exists and is a directory.
    /// <see langword="false"/> otherwise.
    /// </returns>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Stat() instead. This" +
            "function just wraps it anyhow.", false)]
    public static partial bool IsDirectory(string fileName);

    /// <summary>
    /// Determine if a file in the search path is really a symbolic link.
    /// </summary>
    /// <remarks>
    /// Determine if the first occurence of <paramref name="fileName"/>
    /// in the search path is really a symbolic link.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> hasn't been called, and as such,
    /// this function will always return <see langword="false"/> in that case.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Stat"/><br/>
    /// <seealso cref="Exists"/>
    /// </remarks>
    /// <param name="fileName">filename in platform-independent notation.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="fileName"/> exists and is a symlink.
    /// <see langword="false"/> otherwise.
    /// </returns>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Stat() instead. This" +
            "function just wraps it anyhow.", false)]
    public static partial bool IsSymbolicLink(string fileName);

    /// <summary>
    /// Create a directory.
    /// </summary>
    /// <remarks>
    /// This is specified in platform-independent notation in relation to the
    /// write dir. All missing parent directories are also created if they
    /// don't exist.<br/><br/>
    /// So if you've got the write dir set to "C:\mygame\writedir" and call
    /// <c>PhysicsFS.Mkdir("downloads/maps")</c> then the directories
    /// "C:\mygame\writedir\downloads" and "C:\mygame\writedir\downloads\maps"
    /// will be created if possible. If the creation of "maps" fails after we
    /// have successfully created "downloads", then the function leaves the
    /// created directory behind and reports failure.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Delete"/>
    /// </remarks>
    /// <param name="dirName">New dir to create.</param>
    public static partial void Mkdir(string dirName);

    /// <summary>
    /// Add an archive or directory to the search path.
    /// </summary>
    /// <remarks>
    /// If this is a duplicate, the entry is not added again, even though the
    /// function succeeds. You may not add the same archive to two different
    /// mountpoints: duplicate checking is done against the archive and not the
    /// mountpoint.<br/><br/>
    /// When you mount an archive, it is added to a virtual file system...all files
    /// in all of the archives are interpolated into a single hierachical file
    /// tree. Two archives mounted at the same place (or an archive with files
    /// overlapping another mountpoint) may have overlapping files: in such a case,
    /// the file earliest in the search path is selected, and the other files are
    /// inaccessible to the application. This allows archives to be used to
    /// override previous revisions; you can use the mounting mechanism to place
    /// archives at a specific point in the file tree and prevent overlap; this
    /// is useful for downloadable mods that might trample over application data
    /// or each other, for example.<br/><br/>
    /// The mountpoint does not need to exist prior to mounting, which is different
    /// than those familiar with the Unix concept of "mounting" may expect.
    /// As well, more than one archive can be mounted to the same mountpoint, or
    /// mountpoints and archive contents can overlap...the interpolation mechanism
    /// still functions as usual.<br/><br/>
    /// Specifying a symbolic link to an archive or directory is allowed here,
    /// regardless of the state of <seealso cref="SymbolicLinksPermitted"/>. That function
    /// only deals with symlinks inside the mounted directory or archive.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="RemoveFromSearchPath"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="GetMountPoint"/><br/>
    /// <seealso cref="MountIo"/>
    /// </remarks>
    /// <param name="newDir">
    /// directory or archive to add to the path, in
    ///   platform-dependent notation.
    /// </param>
    /// <param name="mountPoint">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// <see langword="null"/> or "" is equivalent to "/".
    /// </param>
    /// <param name="appendToPath">
    /// <see langword="true"/> to append to search path, <see langword="false"/> to prepend.
    /// </param>
    public static partial void Mount(string newDir, string? mountPoint, bool appendToPath);

    /// <summary>
    /// Add an archive, contained in a <see cref="PhysFsFileHandle"/>
    /// handle, to the search path.
    /// </summary>
    /// <remarks>
    /// Unless you have some special, low-level need, you should be using
    /// <see cref="Mount"/> instead of this.<br/><br/>
    /// Archives-in-archives may be very slow! While a <see cref="PhysFsFileHandle"/>
    /// can seek even when the data is compressed, it may do so by rewinding
    /// to the start and decompressing everything before the seek point.
    /// Normal archive usage may do a lot of seeking behind the scenes.
    /// As such, you might find normal archive usage extremely painful
    /// if mounted this way. Plan accordingly: if you, say, have a
    /// self-extracting .zip file, and want to mount something in it,
    /// compress the contents of the inner archive and make sure the outer
    /// .zip file doesn't compress the inner archive too.<br/><br/>
    /// This function operates just like <see cref="Mount"/>,
    /// but takes a <see cref="PhysFsFileHandle"/>
    /// handle instead of a pathname. This handle contains all the data of the
    /// archive, and is used instead of a real file in the physical filesystem.
    /// The <see cref="PhysFsFileHandle"/> may be backed by a real file in the physical filesystem,
    /// but isn't necessarily. The most popular use for this is likely to mount
    /// archives stored inside other archives.<br/><br/>
    /// <paramref name="newDir"/> must be a unique string to identify this archive.
    /// It is used to optimize archiver selection (if you name it XXXXX.zip, we might try
    /// the ZIP archiver first, for example, or directly choose an archiver that
    /// can only trust the data is valid by filename extension). It doesn't
    /// need to refer to a real file at all. If the filename extension isn't
    /// helpful, the system will try every archiver until one works or none
    /// of them do. This filename must be unique, as the system won't allow you
    /// to have two archives with the same name.<br/><br/>
    /// <paramref name="file"/> must remain until the archive is unmounted.
    /// When the archive is unmounted, the system will call
    /// <seealso cref="PhysFsFileHandle.ReleaseHandle"/>. If you need this
    /// handle to survive, you will have to wrap this in a 
    /// <see cref="PhysFsIo"/> and use <see cref="MountIo"/> instead.<br/><br/>
    /// If this function fails,
    /// <seealso cref="PhysFsFileHandle.ReleaseHandle"/> is not called.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Unmount"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="GetMountPoint"/>
    /// </remarks>
    /// <param name="file">
    /// The <see cref="PhysFsFileHandle"/> containing archive data.
    /// </param>
    /// <param name="newDir">Filename that can represent this stream.</param>
    /// <param name="mountPoint">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// <see langword="null"/> or "" is equivalent to "/".
    /// </param>
    /// <param name="appendToPath">
    /// <see langword="true"/> to append to search path,
    /// <see langword="false"/> to prepend.</param>
    public static partial void MountHandle(
        PhysFsFileHandle file,
        string newDir,
        string? mountPoint,
        bool appendToPath);

    /// <summary>
    /// Add an archive, built on a <see cref="PhysFsIo"/>, to the search path.
    /// </summary>
    /// <remarks>
    /// Unless you have some special, low-level need, you should be using
    /// <see cref="Mount"/> instead of this.<br/><br/>
    /// This function operates just like <see cref="Mount"/>,
    /// but takes a <see cref="PhysFsIo"/> instead of a pathname. Behind the scenes,
    /// <see cref="Mount"/> calls this function with a
    /// physical-filesystem-based <see cref="PhysFsIo"/>.<br/><br/>
    /// <paramref name="newDir"/> must be a unique string to identify this archive.
    /// It is used to optimize archiver selection (if you name it XXXXX.zip, we might
    /// try the ZIP archiver first, for example, or directly choose an archiver that
    /// can only trust the data is valid by filename extension). It doesn't
    /// need to refer to a real file at all. If the filename extension isn't
    /// helpful, the system will try every archiver until one works or none
    /// of them do. This filename must be unique, as the system won't allow you
    /// to have two archives with the same name.<br/><br/>
    /// <paramref name="io"/> must remain until the archive is unmounted.
    /// When the archive is unmounted, the system will call <see cref="PhysFsIo.Destroy"/>,
    /// which will give you a chance to free your resources.<br/><br/>
    /// If this function fails, <see cref="PhysFsIo.Destroy"/> is not called.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Unmount"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="GetMountPoint(string)"/>
    /// </remarks>
    /// <param name="io">i/o instance for archive to add to the path.</param>
    /// <param name="newDir">Filename that can represent this stream.</param>
    /// <param name="mountPoint">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// NULL or "" is equivalent to "/".
    /// </param>
    /// <param name="appendToPath">nonzero to append to search path, zero to prepend.</param>
    public static partial void MountIo(
        PhysFsIo.IoHandle io,
        string newDir,
        string? mountPoint,
        bool appendToPath);

    /// <summary>
    /// Add an archive, contained in a memory buffer, to the search path.
    /// </summary>
    /// <remarks>
    /// Unless you have some special, low-level need, you should be using
    /// <see cref="Mount"/> instead of this.<br/><br/>
    /// This function operates just like <see cref="Mount"/>,
    /// but takes a memory buffer instead of a pathname.
    /// This buffer contains all the data of the archive,
    /// and is used instead of a real file in the physical filesystem.<br/><br/>
    /// <paramref name="newDir"/> must be a unique string to identify this archive.
    /// It is used to optimize archiver selection (if you name it XXXXX.zip, we might try
    /// the ZIP archiver first, for example, or directly choose an archiver that
    /// can only trust the data is valid by filename extension). It doesn't
    /// need to refer to a real file at all. If the filename extension isn't
    /// helpful, the system will try every archiver until one works or none
    /// of them do. This filename must be unique, as the system won't allow you
    /// to have two archives with the same name.<br/><br/>
    /// <see cref="nint"/> must remain until the archive is unmounted. When the archive is
    /// unmounted, the system will call <paramref name="del"/>, which will notify you that
    /// the system is done with the buffer, and give you a chance to free your
    /// resources. <paramref name="del"/>  can be <see langword="null"/>,
    /// in which case the system will make no attempt to free the buffer.<br/><br/>
    /// If this function fails, <paramref name="del"/> is not called.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Unmount"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="GetMountPoint"/>
    /// </remarks>
    /// <param name="buf">Address of the memory buffer containing the archive data.</param>
    /// <param name="len">Size of memory buffer, in bytes.</param>
    /// <param name="del">
    /// A callback that triggers upon unmount. Can be <see langword="null"/>.
    /// </param>
    /// <param name="newDir">Filename that can represent this stream.</param>
    /// <param name="mountPoint">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// <see langword="null"/> or "" is equivalent to "/".
    /// </param>
    /// <param name="appendToPath">nonzero to append to search path, zero to prepend.</param>
    public static partial void MountMemory(
        IntPtr buf,
        ulong len,
        PhysFsMountMemoryDel? del,
        string newDir,
        string? mountPoint,
        bool appendToPath);

    /// <summary>
    /// Open a file for appending.
    /// </summary>
    /// <remarks>
    /// Open a file for writing, in platform-independent notation and in relation
    /// to the write dir as the root of the writable filesystem. The specified
    /// file is created if it doesn't exist. If it does exist, the writing offset
    /// is set to the end of the file, so the first write will be the byte after
    /// the end.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> hasn't been set, and opening a
    /// symlink with this function will fail in such a case.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="OpenRead"/><br/>
    /// <seealso cref="OpenWrite"/><br/>
    /// <seealso cref="Write"/><br/>
    /// </remarks>
    /// <param name="fileName">File to open.</param>
    /// <returns>
    /// A valid <see cref="PhysFsFileHandle"/> on success, exception on error.
    /// </returns>
    public static partial PhysFsFileHandle OpenAppend(string fileName);

    /// <summary>
    /// Open a file for reading.
    /// </summary>
    /// <remarks>
    /// Open a file for reading, in platform-independent notation. The search path
    /// is checked one at a time until a matching file is found, in which case an
    /// abstract filehandle is associated with it, and reading may be done.
    /// The reading offset is set to the first byte of the file.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> hasn't been set, and opening a
    /// symlink with this function will fail in such a case.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="OpenWrite(string)"/><br/>
    /// <seealso cref="OpenAppend(string)"/><br/>
    /// <seealso cref="Read"/><br/>
    /// </remarks>
    /// <param name="fileName">File to open.</param>
    /// <returns>
    /// A valid <see cref="PhysFsFileHandle"/> on success, exception on error.
    /// </returns>
    public static partial PhysFsFileHandle OpenRead(string fileName);

    /// <summary>
    /// Open a file for writing.
    /// </summary>
    /// <remarks>
    /// Open a file for writing, in platform-independent notation and in relation
    /// to the write dir as the root of the writable filesystem. The specified
    /// file is created if it doesn't exist. If it does exist, it is truncated to
    /// zero bytes, and the writing offset is set to the start.<br/><br/>
    /// Note that entries that are symlinks are ignored if
    /// <see cref="SymbolicLinksPermitted"/> hasn't been set, and opening a
    /// symlink with this function will fail in such a case.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="OpenRead(string)"/><br/>
    /// <seealso cref="OpenAppend(string)"/><br/>
    /// <seealso cref="Write"/><br/>
    /// </remarks>
    /// <param name="fileName">File to open.</param>
    /// <returns>
    /// A valid <see cref="PhysFsFileHandle"/> on success, exception on error.
    /// </returns>
    public static partial PhysFsFileHandle OpenWrite(string fileName);

    /// <summary>
    /// Read data from a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="ReadBytes"/><br/>
    /// <seealso cref="EOF"/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead"/>.
    /// </param>
    /// <param name="buffer">buffer to store read data into.</param>
    /// <param name="objSize">
    /// size in bytes of objects being read from <paramref name="handle"/>.
    /// </param>
    /// <param name="objCount">
    /// number of <paramref name="objSize"/> objects to read from <paramref name="handle"/>.
    /// </param>
    /// <returns>
    /// number of objects read.
    /// </returns>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.ReadBytes() instead. This" +
            "function just wraps it anyhow. This function never clarified" +
            "what would happen if you managed to read a partial object, so" +
            "working at the byte level makes this cleaner for everyone," +
            "especially now that PhysFsIo interfaces can be supplied by the" +
            "application.", false)]
    public static partial long Read(PhysFsFileHandle handle, IntPtr buffer, uint objSize, uint objCount);

    /// <summary>
    /// Read bytes from a PhysicsFS filehandle
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="EOF(PhysFsFileHandle)"/>
    /// </remarks>
    /// <param name="handle">handle returned from <see cref="OpenRead"/>.</param>
    /// <param name="buffer">
    /// buffer of at least <paramref name="len"/> bytes to store read data into.
    /// </param>
    /// <param name="len">
    /// number of bytes being read from <paramref name="handle"/>.
    /// </param>
    /// <returns>
    /// number of bytes read. This may be less than <paramref name="len"/>; this does not
    /// signify an error, necessarily (a short read may mean EOF).
    /// </returns>
    public static partial long ReadBytes(PhysFsFileHandle handle, IntPtr buffer, ulong len);

    /// <summary>
    /// Read and convert a signed 16-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 16-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial short ReadSBE16(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert a signed 32-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 32-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial int ReadSBE32(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert a signed 64-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 64-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial long ReadSBE64(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert a signed 16-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 16-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial short ReadSLE16(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert a signed 32-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 32-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial int ReadSLE32(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert a signed 64-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read a signed 64-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial long ReadSLE64(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 16-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 16-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial short ReadUBE16(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 32-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 32-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial int ReadUBE32(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 64-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 64-bit bigendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial long ReadUBE64(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 16-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 16-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial short ReadULE16(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 32-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 32-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial int ReadULE32(PhysFsFileHandle file);

    /// <summary>
    /// Read and convert an unsigned 64-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Read an unsigned 64-bit littleendian value from a
    /// file and convert it to the platform's native byte order.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle from which to read.</param>
    /// <returns>
    /// the result.
    /// </returns>
    public static partial long ReadULE64(PhysFsFileHandle file);

    /// <summary>
    /// Add a new archiver to the system.
    /// </summary>
    /// <remarks>
    /// This is advanced, hardcore stuff. You don't need this unless you
    /// really know what you're doing. Most apps will not need this.<br/><br/>
    /// If you want to provide your own archiver (for example, a custom archive
    /// file format, or some virtual thing you want to make look like a filesystem
    /// that you can access through the usual PhysicsFS APIs), this is where you
    /// start. Once an archiver is successfully registered, then you can use
    /// <see cref="Mount"/> to add archives that your archiver
    /// supports to the search path, or perhaps use it as the write dir. Internally,
    /// PhysicsFS uses this function to register its own built-in archivers, like .zip
    /// support, etc.<br/><br/>
    /// You may not have two archivers that handle the same extension. If you are
    /// going to have a clash, you can deregister the other archiver (including
    /// built-in ones) with <see cref="DeregisterArchiver"/>.<br/><br/>
    /// The data in (archiver) is copied; you may free this pointer when this
    /// function returns.<br/><br/>
    /// Once this function returns successfully, PhysicsFS will be able to support
    /// archives of this type until you deregister the archiver again.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsArchiver"/><br/>
    /// <seealso cref="PhysFsArchiver.ArchiverHandle"/><br/>
    /// <seealso cref="DeregisterArchiver"/>
    /// </remarks>
    /// <param name="archiver">The archiver to register.</param>
    public static partial void RegisterArchiver(ref PhysFsArchiver.ArchiverHandle archiver);

    /// <summary>
    /// Remove a directory or archive from the search path.
    /// </summary>
    /// <remarks>
    /// This function is equivalent to: <seealso cref="Unmount"/><br/><br/>
    /// You must use this and not <see cref="Unmount"/> if binary compatibility with
    /// PhysicsFS 1.0 is important (which it may not be for many people).<br/><br/><br/><br/>
    /// See also:<br/>
    /// <seealso cref="AddToSearchPath"/><br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="Unmount"/>
    /// </remarks>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.Unmount() instead. This" +
            "function just wraps it anyhow. There's no functional difference" +
            "except the vocabulary changed from \"adding to the search path\"" +
            "to \"mounting\" when that functionality was extended, and thus" +
            "the preferred way to accomplish this function's work is now" +
            "called \"unmounting.\"", false)]
    public static partial void RemoveFromSearchPath(string oldDir);

    /// <summary>
    /// Seek to a new position within a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// The next read or write will occur at that place. Seeking past the
    /// beginning or end of the file is not allowed, and causes an error.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Tell"/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead(string)"/>, <see cref="OpenWrite(string)"/>
    /// or <see cref="OpenAppend(string)"/>.
    /// </param>
    /// <param name="pos">number of bytes from start of file to seek to.</param>
    public static partial void Seek(PhysFsFileHandle handle, ulong pos);

    /// <summary>
    /// Hook your own allocation routines into PhysicsFS.
    /// </summary>
    /// <remarks>
    /// (This is for limited, hardcore use. If you don't immediately see a need
    /// for it, you can probably ignore this forever.)<br/><br/>
    /// By default, PhysicsFS will use whatever is reasonable for a platform
    /// to manage dynamic memory (usually ANSI C malloc/realloc/free, but
    /// some platforms might use something else), but in some uncommon cases, the
    /// app might want more control over the library's memory management. This
    /// lets you redirect PhysicsFS to use your own allocation routines instead.
    /// You can only call this function before <see cref="Init"/>; if the library is
    /// initialized, it'll reject your efforts to change the allocator mid-stream.
    /// You may call this function after <see cref="Deinit"/> if you are willing to
    /// shut down the library and restart it with a new allocator; this is a safe
    /// and supported operation. The allocator remains intact between deinit/init
    /// calls. If you want to return to the platform's default allocator, pass a
    /// <see langword="null"/> in here.<br/><br/>
    /// If you aren't immediately sure what to do with this function, you can
    /// safely ignore it altogether.
    /// </remarks>
    /// <param name="allocator">Structure containing your allocator's entry points.</param>
    public static partial void SetAllocator(NullableRef<PhysFsAllocator.AllocatorHandle> allocator);

    /// <summary>
    /// Set up buffering for a PhysicsFS file handle.
    /// </summary>
    /// <remarks>
    /// Define an i/o buffer for a file handle. A memory block of
    /// <paramref name="bufsize"/> bytes will be allocated and associated
    /// with <paramref name="handle"/>.<br/><br/>
    /// For files opened for reading, up to <paramref name="bufsize"/>
    /// bytes are read from <paramref name="handle"/> and stored in the internal buffer.
    /// Calls to <see cref="Read"/> will pull from this buffer until it is empty,
    /// and then refill it for more reading.
    /// Note that compressed files, like ZIP archives, will decompress while
    /// buffering, so this can be handy for offsetting CPU-intensive operations.
    /// The buffer isn't filled until you do your next read.<br/><br/>
    /// For files opened for writing, data will be buffered to memory until the
    /// buffer is full or the buffer is flushed. Closing a handle implicitly
    /// causes a flush...check your return values!<br/><br/>
    /// Seeking, etc transparently accounts for buffering.<br/><br/>
    /// You can resize an existing buffer by calling this function more than once
    /// on the same file. Setting the buffer size to zero will free an existing
    /// buffer.<br/><br/>
    /// PhysicsFS file handles are unbuffered by default.<br/><br/>
    /// Please check the return value of this function! Failures can include
    /// not being able to seek backwards in a read-only file when removing the
    /// buffer, not being able to allocate the buffer, and not being able to
    /// flush the buffer to disk, among other unexpected problems.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Flush"/><br/>
    /// <seealso cref="Read"/><br/>
    /// <seealso cref="Write"/><br/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead"/>, <see cref="OpenWrite"/>
    /// or <see cref="OpenAppend"/>.
    /// </param>
    /// <param name="bufsize">size, in bytes, of buffer to allocate.</param>
    public static partial void SetBuffer(PhysFsFileHandle handle, ulong bufsize);

    /// <summary>
    /// Set the current thread's error code.
    /// </summary>
    /// <remarks>
    /// This lets you set the value that will be returned by the next call to
    /// <see cref="GetLastErrorCode"/>. This will replace any existing error code,
    /// whether set by your application or internally by PhysicsFS.<br/><br/>
    /// Error codes are stored per-thread; what you set here will not be
    /// accessible to another thread.<br/><br/>
    /// Any call into PhysicsFS may change the current error code, so any code you
    /// set here is somewhat fragile, and thus you shouldn't build any serious
    /// error reporting framework on this function. The primary goal of this
    /// function is to allow <see cref="PhysFsIo"/> implementations to set the error state,
    /// which generally will be passed back to your application when PhysicsFS
    /// makes a <see cref="PhysFsIo"/> call that fails internally.<br/><br/>
    /// This function doesn't care if the error code is a value known to PhysicsFS
    /// or not (but <see cref="GetErrorByCode"/> will return
    /// <see langword="null"/> for unknown values).
    /// The value will be reported unmolested by <see cref="GetLastErrorCode"/>.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetLastErrorCode"/><br/>
    /// <seealso cref="GetErrorByCode"/>
    /// </remarks>
    /// <param name="code">Error code to become the current thread's new error state.</param>
    public static partial void SetErrorCode(PhysFsErrorCode code);

    /// <summary>
    /// Make a subdirectory of an archive its root directory.
    /// </summary>
    /// <remarks>
    /// This lets you narrow down the accessible files in a specific archive. For
    /// example, if you have x.zip with a file in y/z.txt, mounted to /a, if you
    /// call <c>PhysicsFS.SetRoot("x.zip", "/y")</c>, then the call
    /// <c>PhysicsFS.OpenRead("/a/z.txt")</c> will succeed.<br/><br/>
    /// You can change an archive's root at any time, altering the interpolated
    /// file tree (depending on where paths shift, a different archive may be
    /// providing various files). If you set the root to NULL or "/", the
    /// archive will be treated as if no special root was set (as if the archive
    /// was just mounted normally).<br/><br/>
    /// Changing the root only affects future operations on pathnames; a file
    /// that was opened from a path that changed due to a setRoot will not be
    /// affected.<br/><br/>
    /// Setting a new root is not limited to archives in the search path; you may
    /// set one on the write dir, too, which might be useful if you have files
    /// open for write and thus can't change the write dir at the moment.<br/><br/>
    /// It is not an error to set a subdirectory that does not exist to be the
    /// root of an archive; however, no files will be visible in this case. If
    /// the missing directories end up getting created (a mkdir to the physical
    /// filesystem, etc) then this will be reflected in the interpolated tree.
    /// </remarks>
    /// <param name="archive">dir/archive on which to change root.</param>
    /// <param name="subdir">new subdirectory to make the root of this archive.</param>
    public static partial void SetRoot(string archive, string? subdir);

    /// <summary>
    /// Set up sane, default paths.
    /// </summary>
    /// <remarks>
    /// Helper function.<br/><br/>
    /// The write dir will be set to the pref dir returned by
    /// <see cref="GetPrefDir"/>, which is
    /// created if it doesn't exist.<br/><br/>
    /// The above is sufficient to make sure your program's configuration directory
    /// is separated from other clutter, and platform-independent.<br/><br/>
    /// The search path will be:<br/>
    ///   - The Write Dir (created if it doesn't exist)<br/>
    ///   - The Base Dir (<see cref="BaseDir"/>)<br/>
    ///   - All found CD-ROM dirs (optionally)<br/><br/>
    /// These directories are then searched for files ending with the extension
    /// <paramref name="archiveExt"/>, which, if they are valid and supported archives,
    /// will also be added to the search path. If you specified "PKG" for
    /// <paramref name="archiveExt"/>, and there's a file named data.PKG in the base dir,
    /// it'll be checked. Archives can either be appended or prepended to the search path
    /// in alphabetical order, regardless of which directories they were found in. All archives
    /// are mounted in the root of the virtual file system ("/").<br/><br/>
    /// All of this can be accomplished from the application, but this just does it
    /// all for you. Feel free to add more to the search path manually, too.
    /// </remarks>
    /// <param name="organization">
    /// Name of your company/group/etc to be used as a
    ///  dirname, so keep it small, and no-frills.
    /// </param>
    /// <param name="appName">
    /// Program-specific name of your program, to separate it
    /// from other programs using PhysicsFS.
    /// </param>
    /// <param name="archiveExt">
    /// File extension used by your program to specify an
    /// archive. For example, Quake 3 uses "pk3", even though
    /// they are just zipfiles. Specify <see langword="null"/> to not dig out
    /// archives automatically. Do not specify the '.' char;
    /// If you want to look for ZIP files, specify "ZIP" and
    /// not ".ZIP" ... the archive search is case-insensitive.
    /// </param>
    /// <param name="includeCdRoms">
    /// <see langword="true"/> to include CD-ROMs in the search path, and
    /// (if <paramref name="archiveExt"/> != <see langword="null"/>)
    /// search them for archives. This may cause a significant amount
    /// of blocking while discs are accessed, and if there are no
    /// discs in the drive (or even not mounted on Unix systems),
    /// then they may not be made available anyhow. You may
    /// want to specify <see langword="false"/> and handle the disc setup
    /// yourself.
    /// </param>
    /// <param name="archivesFirst">
    /// <see langword="true"/> to prepend the archives to the search path.
    /// <see langword="false"/> to append them. Ignored if
    /// <paramref name="archiveExt"/> is <see langword="null"/>.
    /// </param>
    public static partial void SetSaneConfig(string organization,
        string appName, string? archiveExt, bool includeCdRoms, bool archivesFirst);

    /// <summary>
    /// Tell PhysicsFS where it may write files.
    /// </summary>
    /// <remarks>
    /// Set a new write dir. This will override the previous setting.<br/><br/>
    /// This call will fail (and fail to change the write dir) if the current
    /// write dir still has files open in it.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetWriteDir"/>
    /// </remarks>
    /// <param name="newDir">
    /// The new directory to be the root of the write dir,
    /// specified in platform-dependent notation. Setting to
    /// <see langword="null"/> disables the write dir, so no
    /// files can be opened for writing via PhysicsFS.
    /// </param>
    public static partial void SetWriteDir(string? newDir);

    /// <summary>
    /// Get various information about a directory or a file.
    /// </summary>
    /// <remarks>
    /// Obtain various information about a file or directory from the meta data.<br/><br/>
    /// This function will never follow symbolic links. If you haven't enabled
    /// symlinks with <see cref="SymbolicLinksPermitted"/>, stat'ing a symlink will be
    /// treated like stat'ing a non-existant file. If symlinks are enabled,
    /// stat'ing a symlink will give you information on the link itself and not
    /// what it points to.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="PhysFsStat"/>
    /// </remarks>
    /// <param name="fileName">filename to check, in platform-indepedent notation.</param>
    /// <returns>
    /// structure filled in with data about <paramref name="fileName"/>.
    /// </returns>
    public static partial PhysFsStat Stat(string fileName);

    /// <summary>
    /// Get a list of supported archive types.
    /// </summary>
    /// <remarks>
    /// Get a list of archive types supported by this implementation of PhysicFS.
    /// These are the file formats usable for search path entries. This is for
    /// informational purposes only. Note that the extension listed is merely
    /// convention: if we list "ZIP", you can open a PkZip-compatible archive
    /// with an extension of "XYZ", if you like.<br/><br/>
    /// The returned value is a enumeration of
    /// <see cref="PhysFsArchiveInfo"/> structures:<br/><br/>
    /// <code>
    /// IEnumerable&lt;PhysFsArchiveInfo&gt; archives = PhysicsFS.SupportedArchiveTypes();
    /// 
    /// foreach (PhysFsArchiveInfo i in archives)
    /// {
    ///    Console.WriteLine("Supported archive: {0}, which is {1}.",
    ///             i.Extension, i.Description);
    /// }
    /// </code><br/><br/><br/>
    /// The returned values are valid until the next call to <see cref="Deinit"/>,
    /// <see cref="RegisterArchiver(ref PhysFsArchiver.ArchiverHandle)"/>,
    /// or <see cref="DeregisterArchiver(string)"/>.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="RegisterArchiver(ref PhysFsArchiver.ArchiverHandle)"/><br/>
    /// <seealso cref="DeregisterArchiver(string)"/>
    /// </remarks>
    /// <returns>Structures describing archives.</returns>
    public static partial IEnumerable<PhysFsArchiveInfo> SupportedArchiveTypes();

    /// <summary>
    /// Swap bigendian signed 16 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 16-bit signed value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial short SwapSBE16(short val);

    /// <summary>
    /// Swap bigendian signed 32 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 32-bit signed value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial int SwapSBE32(int val);

    /// <summary>
    /// Swap bigendian signed 64 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 64-bit signed value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial long SwapSBE64(long val);

    /// <summary>
    /// Swap littleendian signed 16 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 16-bit signed value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial short SwapSLE16(short val);

    /// <summary>
    /// Swap littleendian signed 32 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 32-bit signed value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial int SwapSLE32(int val);

    /// <summary>
    /// Swap littleendian signed 64 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 64-bit signed value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial long SwapSLE64(long val);

    /// <summary>
    /// Swap bigendian unsigned 16 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 16-bit unsigned value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial ushort SwapUBE16(ushort val);

    /// <summary>
    /// Swap bigendian unsigned 32 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 32-bit unsigned value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial uint SwapUBE32(uint val);

    /// <summary>
    /// Swap bigendian unsigned 64 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 64-bit unsigned value in bigendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial ulong SwapUBE64(ulong val);

    /// <summary>
    /// Swap littleendian unsigned 16 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 16-bit unsigned value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial ushort SwapULE16(ushort val);

    /// <summary>
    /// Swap littleendian unsigned 32 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 32-bit unsigned value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial uint SwapULE32(uint val);

    /// <summary>
    /// Swap littleendian unsigned 64 to platform's native byte order.
    /// </summary>
    /// <remarks>
    /// Take a 64-bit unsigned value in littleendian format and convert it to
    /// the platform's native byte order.
    /// </remarks>
    /// <param name="val">value to convert</param>
    /// <returns>converted value.</returns>
    public static partial ulong SwapULE64(ulong val);

    /// <summary>
    /// Determine current position within a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// See also:<br/>
    /// <seealso cref="Seek"/>
    /// </remarks>
    /// <param name="handle">
    /// handle returned from <see cref="OpenRead"/>, <see cref="OpenWrite"/>
    /// or <see cref="OpenAppend"/>.
    /// </param>
    /// <returns>
    /// offset in bytes from start of file.
    /// </returns>
    public static partial ulong Tell(PhysFsFileHandle handle);

    /// <summary>
    /// Case-insensitive compare of two UCS-4 strings.
    /// </summary>
    /// <remarks>
    /// This is a strcasecmp/stricmp replacement that expects both strings
    /// to be in UCS-4 (aka UTF-32) encoding. It will do "case folding" to decide
    /// if the Unicode codepoints in the strings match.<br/><br/>
    /// It will report which string is "greater than" the other, but be aware that
    /// this doesn't necessarily mean anything: 'a' may be "less than" 'b', but
    /// a Japanese kuten has no meaningful alphabetically relationship to
    /// a Greek lambda, but being able to assign a reliable "value" makes sorting
    /// algorithms possible, if not entirely sane. Most cases should treat the
    /// return value as "equal" or "not equal".<br/><br/>
    /// Like stricmp, this expects both strings to be null-terminated.
    /// </remarks>
    /// <param name="str1">First string to compare.</param>
    /// <param name="str2">Second string to compare.</param>
    /// <returns>-1 if str1 is "less than" str2, 1 if "greater than", 0 if equal.</returns>
    public static partial int Ucs4stricmp(ReadOnlySpan<uint> str1, ReadOnlySpan<uint> str2);

    /// <summary>
    /// Remove a directory or archive from the search path.
    /// </summary>
    /// <remarks>
    /// This is functionally equivalent to
    /// <see cref="RemoveFromSearchPath"/>, but that function is
    /// deprecated to keep the vocabulary paired with <see cref="Mount"/>.<br/><br/>
    /// This must be a (case-sensitive) match to a dir or archive already in the
    /// search path, specified in platform-dependent notation.<br/><br/>
    /// This call will fail (and fail to remove from the path) if the element still
    /// has files open in it.<br/><br/>
    /// This function wants the path to the archive or directory that was
    /// mounted (the same string used for the "newDir" argument of
    /// <see cref="AddToSearchPath"/> or any of the mount functions),
    /// not the path where it is mounted in the tree (the "mountPoint" argument
    /// to any of the mount functions).<br/><br/>
    /// See also:<br/>
    /// <seealso cref="GetSearchPath"/><br/>
    /// <seealso cref="Mount"/>
    /// </remarks>
    /// <param name="oldDir">dir/archive to remove.</param>
    public static partial void Unmount(string oldDir);

    /// <summary>
    /// Case-insensitive compare of two UTF-16 strings.
    /// </summary>
    /// <remarks>
    /// This is a strcasecmp/stricmp replacement that expects both strings
    /// to be in UTF-16 encoding. It will do "case folding" to decide if the
    /// Unicode codepoints in the strings match.<br/><br/>
    /// It will report which string is "greater than" the other, but be aware that
    /// this doesn't necessarily mean anything: 'a' may be "less than" 'b', but
    /// a Japanese kuten has no meaningful alphabetically relationship to
    /// a Greek lambda, but being able to assign a reliable "value" makes sorting
    /// algorithms possible, if not entirely sane. Most cases should treat the
    /// return value as "equal" or "not equal".<br/><br/>
    /// Like stricmp, this expects both strings to be null-terminated.
    /// </remarks>
    /// <param name="str1">First string to compare.</param>
    /// <param name="str2">Second string to compare.</param>
    /// <returns>-1 if str1 is "less than" str2, 1 if "greater than", 0 if equal.</returns>
    public static partial int Utf16stricmp(ReadOnlySpan<ushort> str1, ReadOnlySpan<ushort> str2);

    /// <summary>
    /// Convert a Latin1 to a UTF-8 string string.
    /// </summary>
    /// <remarks>
    /// Latin1 strings are 8-bits per character: a popular "high ASCII" encoding.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is double the size of the source buffer.
    /// UTF-8 expands latin1 codepoints over 127 from 1 to 2 bytes, so the string
    /// may grow in some cases.<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UTF-8
    /// sequence at the end. If the buffer length is 0, this function does nothing.<br/><br/>
    /// Please note that we do not supply a UTF-8 to Latin1 converter, since Latin1
    /// can't express most Unicode codepoints. It's a legacy encoding; you should
    /// be converting away from it at all times.
    /// </remarks>
    /// <param name="src">Null-terminated source string in Latin1 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>
    /// converted UTF-8 string.
    /// </returns>
    public static partial byte[] Utf8FromLatin1(byte[] src, ulong len);

    /// <summary>
    /// Convert a UCS-2 string to a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// you almost certainly should use <see cref="Utf8FromUtf16"/>,
    /// which became available in PhysicsFS 2.1, unless you know what you're doing.<br/><br/>
    /// This function will not report an error if there are invalid UCS-2
    /// values in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UCS-2 strings are 16-bits per character: <c>TCHAR</c> on Windows, when building
    /// with Unicode support. Please note that modern versions of Windows use
    /// UTF-16, which is an extended form of UCS-2, and not UCS-2 itself. You
    /// almost certainly want <see cref="Utf8FromUtf16"/> instead.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is double the size of the source buffer.
    /// UTF-8 never uses more than 32-bits per character, so while it may shrink
    /// a UCS-2 string, it may also expand it.<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UTF-8
    /// sequence at the end. If the buffer length is 0, this function does nothing.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Utf8FromUtf16"/>
    /// </remarks>
    /// <param name="src">Null-terminated source string in UCS-2 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>converted UTF-8 string.</returns>
    public static partial byte[] Utf8FromUcs2(ushort[] src, ulong len);

    /// <summary>
    /// Convert a UCS-4 string to a UTF-8 string.
    /// </summary>
    /// <remarks>
    /// This function will not report an error if there are invalid UCS-4
    /// values in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UCS-4 (aka UTF-32) strings are 32-bits per character:
    /// <c>wchar_t</c> on Unix.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is the same size as the source buffer. UTF-8
    /// never uses more than 32-bits per character, so while it may shrink a UCS-4
    /// string, it will never expand it.<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UTF-8
    /// sequence at the end. If the buffer length is 0, this function does nothing.
    /// </remarks>
    /// <param name="src">Null-terminated source string in UCS-4 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>Converted UTF-8 string.</returns>
    public static partial byte[] Utf8FromUcs4(uint[] src, ulong len);

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
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is double the size of the source buffer.
    /// UTF-8 never uses more than 32-bits per character, so while it may shrink
    /// a UTF-16 string, it may also expand it.<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UTF-8
    /// sequence at the end. If the buffer length is 0, this function does nothing.
    /// </remarks>
    /// <param name="src">Null-terminated source string in UTF-16 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>Converted UTF-8 string.</returns>
    public static partial byte[] Utf8FromUtf16(ushort[] src, ulong len);

    /// <summary>
    /// Case-insensitive compare of two UTF-8 strings.
    /// </summary>
    /// <remarks>
    /// This is a strcasecmp/stricmp replacement that expects both strings
    /// to be in UTF-8 encoding. It will do "case folding" to decide if the
    /// Unicode codepoints in the strings match.<br/><br/>
    /// If both strings are exclusively low-ASCII characters, this will do the
    /// right thing, as that is also valid UTF-8. If there are any high-ASCII
    /// chars, this will not do what you expect!<br/><br/>
    /// It will report which string is "greater than" the other, but be aware that
    /// this doesn't necessarily mean anything: 'a' may be "less than" 'b', but
    /// a Japanese kuten has no meaningful alphabetically relationship to
    /// a Greek lambda, but being able to assign a reliable "value" makes sorting
    /// algorithms possible, if not entirely sane. Most cases should treat the
    /// return value as "equal" or "not equal".<br/><br/>
    /// Like stricmp, this expects both strings to be <see langword="null"/>-terminated.
    /// </remarks>
    /// <param name="str1">First string to compare.</param>
    /// <param name="str2">Second string to compare.</param>
    /// <returns>-1 if str1 is "less than" str2, 1 if "greater than", 0 if equal.</returns>
    public static partial int Utf8stricmp(string str1, string str2);

    /// <summary>
    /// Convert a UTF-8 string to a UCS-2 string.
    /// </summary>
    /// <remarks>
    /// you almost certainly should use <see cref="Utf8ToUtf16"/>, which
    /// became available in PhysicsFS 2.1, unless you know what you're doing.<br/><br/>
    /// This function will not report an error if there are invalid UTF-8
    /// sequences in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UCS-2 strings are 16-bits per character: \c TCHAR on Windows, when building
    /// with Unicode support. Please note that modern versions of Windows use
    /// UTF-16, which is an extended form of UCS-2, and not UCS-2 itself. You
    /// almost certainly want <see cref="Utf8ToUtf16"/> instead, but you need to
    /// understand how that changes things, too.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is double the size of the source buffer.
    /// UTF-8 uses from one to four bytes per character, but UCS-2 always uses
    /// two, so an entirely low-ASCII string will double in size!<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UCS-2
    /// sequence at the end. If the buffer length is 0, this function does nothing.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Utf8ToUtf16"/>
    /// </remarks>
    /// <param name="src">Null-terminated source string in UTF-8 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>Converted UCS-2 string.</returns>
    public static partial ushort[] Utf8ToUcs2(byte[] src, ulong len);

    /// <summary>
    /// Convert a UTF-8 string to a UCS-4 string.
    /// </summary>
    /// <remarks>
    /// This function will not report an error if there are invalid UTF-8
    /// sequences in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UCS-4 (aka UTF-32) strings are 32-bits per character: <c>wchar_t</c> on
    /// Unix.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is four times the size of the source buffer.
    /// UTF-8 uses from one to four bytes per character, but UCS-4 always uses
    /// four, so an entirely low-ASCII string will quadruple in size!<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UCS-4
    /// sequence at the end. If the buffer length is 0, this function does nothing.
    /// </remarks>
    /// <param name="src">Null-terminated source string in UTF-8 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>Converted UCS-4 string.</returns>
    public static partial uint[] Utf8ToUcs4(byte[] src, ulong len);

    /// <summary>
    /// Convert a UTF-8 string to a UTF-16 string.
    /// </summary>
    /// <remarks>
    /// This function will not report an error if there are invalid UTF-8
    /// sequences in the source string. It will replace them with a '?'
    /// character and continue on.<br/><br/>
    /// UTF-16 strings are 16-bits per character (except some chars, which are
    /// 32-bits): <c>TCHAR</c> on Windows, when building with Unicode support. Modern
    /// Windows releases use UTF-16. Windows releases before 2000 used TCHAR, but
    /// only handled UCS-2. UTF-16 _is_ UCS-2, except for the characters that
    /// are 4 bytes, which aren't representable in UCS-2 at all anyhow. If you
    /// aren't sure, you should be using UTF-16 at this point on Windows.<br/><br/>
    /// To ensure that the destination buffer is large enough for the conversion,
    /// please allocate a buffer that is double the size of the source buffer.
    /// UTF-8 uses from one to four bytes per character, but UTF-16 always uses
    /// two to four, so an entirely low-ASCII string will double in size! The
    /// UTF-16 characters that would take four bytes also take four bytes in UTF-8,
    /// so you don't need to allocate 4x the space just in case: double will do.<br/><br/>
    /// Strings that don't fit in the destination buffer will be truncated, but
    /// will always be null-terminated and never have an incomplete UTF-16
    /// surrogate pair at the end. If the buffer length is 0, this function does
    /// nothing.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="Utf8ToUtf16"/>
    /// </remarks>
    /// <param name="src">Null-terminated source string in UTF-8 format.</param>
    /// <param name="len">Size, in bytes, of destination buffer.</param>
    /// <returns>Converted UTF-16 string.</returns>
    public static partial ushort[] Utf8ToUtf16(byte[] src, ulong len);

    /// <summary>
    /// Write data to a PhysicsFS filehandle
    /// </summary>
    /// <remarks>
    /// The file must be opened for writing.<br/><br/>
    /// See also:<br/>
    /// <seealso cref="WriteBytes"/>
    /// </remarks>
    /// <param name="handle">
    /// retval from <see cref="OpenWrite"/> or <see cref="OpenAppend"/>.
    /// </param>
    /// <param name="buffer">
    /// buffer of bytes to write to <paramref name="handle"/>.
    /// </param>
    /// <param name="objSize">
    /// size in bytes of objects being written to <paramref name="handle"/>.
    /// </param>
    /// <param name="objCount">
    /// number of <paramref name="objSize"/> objects to write to <paramref name="handle"/>.
    /// </param>
    /// <returns>
    /// number of objects written.
    /// </returns>
    [Obsolete("As of PhysicsFS 2.1, use PhysicsFS.WriteBytes() instead. This" +
            "function just wraps it anyhow. This function never clarified" +
            "what would happen if you managed to write a partial object, so" +
            "working at the byte level makes this cleaner for everyone," +
            "especially now that PhysFsIo interfaces can be supplied by the" +
            "application.", false)]
    public static partial long Write(PhysFsFileHandle handle, IntPtr buffer,
        uint objSize, uint objCount);

    /// <summary>
    /// Write data to a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// The file must be opened for writing.<br/><br/>
    /// Please note that while <paramref name="len"/> is an unsigned 64-bit integer,
    /// you are limited to 63 bits (9223372036854775807 bytes), so we can return
    /// a negative value on error. If length is greater than 0x7FFFFFFFFFFFFFFF,
    /// this function will immediately fail. For systems without a 64-bit datatype,
    /// you are limited to 31 bits (0x7FFFFFFF, or 2147483647 bytes). We trust
    /// most things won't need to do multiple gigabytes of i/o in one call anyhow,
    /// but why limit things?
    /// </remarks>
    /// <param name="handle">
    /// retval from <see cref="OpenWrite"/> or <see cref="OpenAppend"/>.
    /// </param>
    /// <param name="buffer">
    /// buffer of <paramref name="len"/> bytes to write to <paramref name="handle"/>.
    /// </param>
    /// <param name="len">
    /// number of bytes being written to <paramref name="handle"/>.
    /// </param>
    /// <returns>
    /// number of bytes written. This may be less than <paramref name="len"/>;
    /// in the case of an error, the system may try to write as many bytes as possible,
    /// so an incomplete write might occur.
    /// failure.
    /// </returns>
    public static partial long WriteBytes(PhysFsFileHandle handle, IntPtr buffer, ulong len);

    /// <summary>
    /// Convert and write a signed 16-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 16-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSBE16(PhysFsFileHandle file, short val);

    /// <summary>
    /// Convert and write a signed 32-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 32-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSBE32(PhysFsFileHandle file, int val);

    /// <summary>
    /// Convert and write a signed 64-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 64-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSBE64(PhysFsFileHandle file, long val);

    /// <summary>
    /// Convert and write a signed 16-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 16-bit value from the platform's
    /// native byte order to littleendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSLE16(PhysFsFileHandle file, short val);

    /// <summary>
    /// Convert and write a signed 32-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 32-bit value from the platform's
    /// native byte order to littleendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSLE32(PhysFsFileHandle file, int val);

    /// <summary>
    /// Convert and write a signed 64-bit littleendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a signed 64-bit value from the platform's
    /// native byte order to littleendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteSLE64(PhysFsFileHandle file, long val);

    /// <summary>
    /// Convert and write a unsigned 16-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 16-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteUBE16(PhysFsFileHandle file, ushort val);

    /// <summary>
    /// Convert and write a unsigned 32-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 32-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteUBE32(PhysFsFileHandle file, uint val);

    /// <summary>
    /// Convert and write a unsigned 64-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 64-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteUBE64(PhysFsFileHandle file, ulong val);

    /// <summary>
    /// Convert and write a unsigned 16-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 16-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteULE16(PhysFsFileHandle file, ushort val);

    /// <summary>
    /// Convert and write a unsigned 32-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 32-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteULE32(PhysFsFileHandle file, uint val);

    /// <summary>
    /// Convert and write a unsigned 64-bit bigendian value.
    /// </summary>
    /// <remarks>
    /// Convenience function. Convert a unsigned 64-bit value from the platform's
    /// native byte order to bigendian and write it to a file.
    /// </remarks>
    /// <param name="file">PhysicsFS file handle to which to write.</param>
    /// <param name="val">Value to convert and write.</param>
    public static partial void WriteULE64(PhysFsFileHandle file, ulong val);
}
