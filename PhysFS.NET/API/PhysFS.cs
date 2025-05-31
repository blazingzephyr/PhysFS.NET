
namespace Icculus.PhysFS.NET;

/// <summary>
/// PhysicsFS<br/>
/// The latest version of PhysicsFS can be found at:<br/>
/// <see cref="https://icculus.org/physfs/"/>
/// </summary>
/// <remarks>
/// PhysicsFS; a portable, flexible file i/o abstraction.<br/><br/>
///
/// This API gives you access to a system file system in ways superior to the
///  stdio or system i/o calls. The brief benefits:<br/>
///
///   * It's portable.<br/>
///   * It's safe. No file access is permitted outside the specified dirs.<br/>
///   * It's flexible. Archives (.ZIP files) can be used transparently as
///      directory structures.<br/><br/>
///
/// With PhysicsFS, you have a single writing directory and multiple
///  directories (the "search path") for reading. You can think of this as a
///  filesystem within a filesystem. If (on Windows) you were to set the
///  writing directory to "C:\MyGame\MyWritingDirectory", then no PHYSFS calls
///  could touch anything above this directory, including the "C:\MyGame" and
///  "C:\" directories. This prevents an application's internal scripting
///  language from piddling over c:\\config.sys, for example. If you'd rather
///  give PHYSFS full access to the system's REAL file system, set the writing
///  dir to "C:\", but that's generally A Bad Thing for several reasons.
/// </remarks>
public static class PhysFS
{
    /// <summary>
    /// The version of PhysicsFS that is linked against your program.<br/>
    /// If you are using a shared library (DLL) version of PhysFS, then it is
    /// possible that it will be different than the version you compiled against.
    /// </summary>
    public static readonly Version LinkedVersion = GetLinkedVersion();

    /// <summary>
    /// The dir separator; '/' on unix, '\\' on win32, ":" on MacOS
    /// </summary>
    public static readonly string DirSeparator = physfs.PHYSFS_getDirSeparator();

    /// <summary>
    /// The path where the application resides (the installation directory).<br/>
    /// This is calculated in <seealso cref="Init(string?)"/>.
    /// </summary>
    public static string BasePath => physfs.PHYSFS_getBaseDir();

    /// <summary>
    /// The path where PhysicsFS will allow file writing.<br/>
    /// The default write dir is null.
    /// </summary>
    /// <remarks>
    /// See also:<br/>
    /// <seealso cref="SetWriteDirectory(string)"/>
    /// </remarks>
    public static string WriteDirectory => physfs.PHYSFS_getWriteDir();

    /// <summary>
    /// Determines if the PhysicsFS library is initialized.<br/>
    /// This is safe to call at any time.
    /// </summary>
    public static bool IsInit => physfs.PHYSFS_isInit();

    /// <summary>
    /// Gets or sets whether the symbolic links are permitted.<br/><br/>
    /// Some physical filesystems and archives contain files that are just pointers
    /// to other files. On the physical filesystem, opening such a link will
    /// (transparently) open the file that is pointed to.<br/><br/>
    /// By default, PhysicsFS will check if a file is really a symlink during open
    /// calls and fail if it is. Otherwise, the link could take you outside the
    /// write and search paths, and compromise security.
    /// </summary>
    public static bool AllowSymbolicLinks
    {
        get => physfs.PHYSFS_symbolicLinksPermitted();
        set
        {
            physfs.PHYSFS_permitSymbolicLinks(value);
#if DEBUG
            Log(
                FG_LGREEN,
                $"{{0}}SUCC{{1}} Allow symbolic links set to {{0}}{value}{{1}}",
                REVERSE,
                NOREVERSE
            );
#endif
        }
    }

    /// <summary>
    /// Initialize the PhysicsFS library.
    /// </summary>
    /// <remarks>
    /// This must be called before any other PhysicsFS function.<br/>
    /// This should be called prior to any attempts to change your process's
    /// current working directory.<br/><br/>
    /// 
    /// PHYSFS_AndroidInit struct. This struct must hold a valid JNIEnv *
    /// and a JNI jobject of a Context (either the application context or
    /// the current Activity is fine). Both are cast to a void* so we
    /// don't need jni.h included wherever physfs.h is. PhysicsFS
    /// uses these objects to query some system details. PhysicsFS does
    /// not hold a reference to the JNIEnv or Context past the call to
    /// <seealso cref="Init(string?)"/>. If you pass a null here,
    /// <seealso cref="Init(string?)"/> can still succeed,
    /// but <seealso cref="BasePath"/> and
    /// <seealso cref="GetPrefDirectory(string, string)"/> will be incorrect.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Deinit"/><br/>
    /// <seealso cref="IsInit"/>
    /// </remarks>
    /// <param name="arg">
    /// the argv[0] string passed to your program's mainline.
    /// This may be null on most platforms (such as ones without a
    /// standard main() function), but you should always try to pass
    /// something in here.
    /// Many Unix-like systems _need_ to pass argv[0]
    /// from main() in here. See warning about Android, too!
    /// </param>
    /// <exception cref="InvalidOperationException"/>
    public static void Init(string? arg = null)
    {
        Assert("Initializing PhysFS", !physfs.PHYSFS_init(arg));
    }

    /// <summary>
    /// Deinitialize the PhysicsFS library.
    /// </summary>
    /// <remarks>
    /// This closes any files opened via PhysicsFS, blanks the search/write paths,
    /// frees memory, and invalidates all of your file handles.<br/><br/>
    /// 
    /// Note that this call can FAIL if there's a file open for writing that
    /// refuses to close(for example, the underlying operating system was
    /// buffering writes to network filesystem, and the fileserver has crashed,
    /// or a hard drive has failed, etc). It is usually best to close all write
    /// handles yourself before calling this function, so that you can gracefully
    /// handle a specific failure.<br/><br/>
    ///
    /// Once successfully deinitialized, <seealso cref="Init(string?)"/> can be called again to
    /// restart the subsystem. All default API states are restored at this
    /// point, with the exception of any custom allocator you might have
    /// specified, which survives between initializations.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Init(string?)"/><br/>
    /// <seealso cref="IsInit"/>
    /// </remarks>
    /// <exception cref="IOException"/>
    /// <exception cref="InvalidOperationException"/>
    public static void Deinit()
    {
        Assert("Deinitializing PhysFS", !physfs.PHYSFS_deinit());
    }

    /// <summary>
    /// Get a list of supported archive types.
    /// </summary>
    /// <remarks>
    /// Get a list of archive types supported by this implementation of PhysicFS.<br/>
    /// These are the file formats usable for search path entries. This is for
    /// informational purposes only. Note that the extension listed is merely
    /// convention: if we list "ZIP", you can open a PkZip-compatible archive
    /// with an extension of "XYZ", if you like.
    /// </remarks>
    /// <returns>Array of READ ONLY structures.</returns>
    /// <exception cref="InvalidOperationException"/>
    public static IEnumerable<ArchiveInfo> GetSupportedArchiveTypes()
    {
        IntPtr archives = physfs.PHYSFS_supportedArchiveTypes();
        Assert($"Getting supported archive types", archives == IntPtr.Zero);

        while (true)
        {
            IntPtr archive = Marshal.ReadIntPtr(archives);
            if (archive == IntPtr.Zero) break;

            var info = Marshal.PtrToStructure<PHYSFS_ArchiveInfo>(archive);
            yield return new ArchiveInfo
            {
                Extension = info.extension,
                Description = info.description,
                Author = info.author,
                Url = info.url,
                SupportsSymlinks = info.supportsSymlinks
            };

            archives += IntPtr.Size;
        }
    }

    /// <summary>
    /// Get a collection of paths to available CD-ROM drives.
    /// </summary>
    /// <remarks>
    /// The dirs returned are platform-dependent ("D:\" on Win32, "/cdrom" or
    /// whatnot on Unix). Dirs are only returned if there is a disc ready and
    /// accessible in the drive. So if you've got two drives (D: and E:), and only
    /// E: has a disc in it, then that's all you get. If the user inserts a disc
    /// in D: and you call this function again, you get both drives. If, on a
    /// Unix box, the user unmounts a disc and remounts it elsewhere, the next
    /// call to this function will reflect that change.<br/><br/>
    /// 
    /// This function refers to "CD-ROM" media, but it really means "inserted disc
    /// media," such as DVD-ROM, HD-DVD, CDRW, and Blu-Ray discs. It looks for
    /// filesystems, and as such won't report an audio CD, unless there's a
    /// mounted filesystem track on it.
    /// </remarks>
    /// <returns>Array of null-terminated strings</returns>
    /// <exception cref="OutOfMemoryException"/>
    public static IEnumerable<string> GetCdRomDirs()
    {
        IntPtr dirs = physfs.PHYSFS_getCdRomDirs();
        IntPtr i = dirs;
        Assert($"Getting CD-ROM directories", dirs == IntPtr.Zero);

        while (true)
        {
            IntPtr dir = Marshal.ReadIntPtr(i);
            if (dir == IntPtr.Zero) break;

            yield return Marshal.PtrToStringUTF8(dir)!;
            i += IntPtr.Size;
        }

        physfs.PHYSFS_freeList(dirs);
    }

    /// <summary>
    /// Tell PhysicsFS where it may write files.
    /// </summary>
    /// <remarks>
    /// Set a new write dir. This will override the previous setting.<br/><br/>
    /// 
    /// This call will fail (and fail to change the write dir) if the current
    /// write dir still has files open in it.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="WriteDirectory"/>
    /// </remarks>
    /// <param name="dir">
    /// The new directory to be the root of the write dir,
    /// specified in platform-dependent notation. Setting to null
    /// disables the write dir, so no files can be opened for
    /// writing via PhysicsFS.
    /// </param>
    /// <exception cref="IOException"/>
    public static void SetWriteDirectory(string dir)
    {
        Assert(
            $"Setting writing directory to {{0}}{dir}{{1}}",
            !physfs.PHYSFS_setWriteDir(dir)
        );
    }

    /// <summary>
    /// Get the current search path.
    /// </summary>
    /// <remarks>
    /// The default search path is an empty list.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Mount(string, bool, string, bool)"/><br/>
    /// <seealso cref="Unmount(string, bool)"/>
    /// </remarks>
    /// <returns>Array of null-terminated strings</returns>
    /// <exception cref="OutOfMemoryException"/>
    public static IEnumerable<string> GetSearchPaths()
    {
        IntPtr paths = physfs.PHYSFS_getSearchPath();
        IntPtr i = paths;
        Assert("Getting available search paths", paths == IntPtr.Zero);

        while (true)
        {
            IntPtr path = Marshal.ReadIntPtr(i);
            if (path == IntPtr.Zero) break;

            yield return Marshal.PtrToStringUTF8(path)!;
            i += IntPtr.Size;
        }

        physfs.PHYSFS_freeList(paths);
    }

    /// <summary>
    /// Helper function.<br/>
    /// Set up sane, default paths.
    /// </summary>
    /// <remarks>
    /// The write dir will be set to the pref dir returned by
    /// <seealso cref="GetPrefDirectory(string, string)"/><br/><br/>
    /// 
    /// The above is sufficient to make sure your program's
    /// configuration directory
    /// is separated from other clutter, and platform-independent.
    /// which is created if it doesn't exist.<br/><br/>
    /// 
    /// The search path will be:<br/>
    /// * The Write Dir (created if it doesn't exist)<br/>
    /// * The Base Dir (<seealso cref="BasePath"/>)<br/>
    /// * All found CD-ROM dirs (optionally)<br/><br/>
    /// 
    /// These directories are then searched for files ending with the extension
    /// (archiveExt), which, if they are valid and supported archives, will also
    /// be added to the search path.If you specified "PKG" for (archiveExt), and
    /// there's a file named data.PKG in the base dir, it'll be checked. Archives
    /// can either be appended or prepended to the search path in alphabetical
    /// order, regardless of which directories they were found in. All archives
    ///  are mounted in the root of the virtual file system("/").<br/><br/>
    ///
    /// All of this can be accomplished from the application, but this just does it
    /// all for you.Feel free to add more to the search path manually, too.
    /// </remarks>
    /// <param name="organization">
    /// Name of your company/group/etc to be used as a
    /// dirname, so keep it small, and no-frills.
    /// </param>
    /// <param name="appName">
    /// Program-specific name of your program, to separate it
    /// from other programs using PhysicsFS.
    /// </param>
    /// <param name="archiveExtension">
    /// File extension used by your program to specify an
    /// archive. For example, Quake 3 uses "pk3", even though
    /// they are just zipfiles. Specify null to not dig out
    /// archives automatically. Do not specify the '.' char;
    /// If you want to look for ZIP files, specify "ZIP" and
    /// not ".ZIP" ... the archive search is case-insensitive.
    /// </param>
    /// <param name="includeCdRoms">
    /// Non-zero to include CD-ROMs in the search path, and
    /// (if (archiveExt) != null) search them for archives.
    /// This may cause a significant amount of blocking
    /// while discs are accessed, and if there are no discs
    /// in the drive (or even not mounted on Unix systems),
    /// then they may not be made available anyhow. You may
    /// want to specify false and handle the disc setup
    /// yourself.
    /// </param>
    /// <param name="prependArchives">
    /// true to prepend the archives to the search path.
    /// false to append them. Ignored if !(archiveExt).
    /// </param>
    /// <exception cref="InvalidOperationException"/>
    public static void SetSaneConfig(
        string organization,
        string appName,
        string? archiveExt,
        bool includeCdRoms,
        bool prependArchives)
    {
        Assert(
            $"Setting default config for {{0}}{organization}{{1}}, {{0}}{appName}{{1}} " +
            $"with CD-ROMS support set to {{0}}{includeCdRoms}{{1}}, " +
            $"default archive extension set to {{0}}{archiveExt}{{1}} "+
            $"and prepend archives set to {{0}}{prependArchives}{{1}}",
            !physfs.PHYSFS_setSaneConfig(
                organization,
                appName,
                archiveExt,
                includeCdRoms,
                prependArchives
            )
        );
    }

    /// <summary>
    /// Create a directory.
    /// </summary>
    /// <remarks>
    /// This is specified in platform-independent notation in relation to the
    /// write dir.All missing parent directories are also created if they
    /// don't exist.<br/><br/>
    ///
    /// So if you've got the write dir set to "C:\mygame\writedir" and call
    /// PHYSFS_mkdir("downloads/maps") then the directories
    /// "C:\mygame\writedir\downloads" and "C:\mygame\writedir\downloads\maps"
    /// will be created if possible.If the creation of "maps" fails after we
    /// have successfully created "downloads", then the function leaves the
    /// created directory behind and reports failure.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Delete(string)"/>
    /// </remarks>
    /// <param name="directoryName">New directory to create</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="OutOfMemoryException"/>
    public static void Mkdir(string directoryName)
    {
        Assert(
            $"Creating {{0}}{directoryName}{{1}}",
            !physfs.PHYSFS_mkdir(directoryName)
        );
    }

    /// <summary>
    /// Delete a file or directory.
    /// </summary>
    /// <remarks>
    /// (fileName) is specified in platform-independent notation in
    /// relation to the write dir.<br/><br/>
    /// 
    /// A directory must be empty before this call can delete it.<br/><br/>
    /// 
    /// Deleting a symlink will remove the link, not what it points to, regardless
    /// of whether you <seealso cref="AllowSymbolicLinks"/> or not.<br/><br/>
    /// 
    /// So if you've got the write dir set to "C:\mygame\writedir" and call
    /// Delete("downloads/maps/level1.map") then the file
    /// "C:\mygame\writedir\downloads\maps\level1.map" is removed from the
    /// physical filesystem, if it exists and the operating system permits the
    /// deletion.<br/><br/>
    /// 
    /// Note that on Unix systems, deleting a file may be successful, but the
    /// actual file won't be removed until all processes that have an open
    /// filehandle to it (including your program) close their handles.<br/><br/>
    /// 
    /// Chances are, the bits that make up the file still exist, they are just
    /// made available to be written over at a later point. Don't consider this
    /// a security method or anything.  :)
    /// </remarks>
    /// <param name="fileName"></param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="OutOfMemoryException"/>
    public static void Delete(string fileName)
    {
        Assert(
            $"Deleting {{0}}{fileName}{{1}}",
            !physfs.PHYSFS_delete(fileName)
        );
    }

    /// <summary>
    /// Figure out where in the search path a file resides.
    /// </summary>
    /// <remarks>
    /// The file is specified in platform-independent notation.The returned
    /// filename will be the element of the search path where the file was found,
    /// which may be a directory, or an archive.Even if there are multiple
    /// matches in different parts of the search path, only the first one found
    ///  is used, just like when opening a file.<br/><br/>
    ///
    /// So, if you look for "maps/level1.map", and C:\\mygame is in your search
    ///  path and C:\\mygame\\maps\\level1.map exists, then "C:\mygame" is returned.<br/><br/>
    ///
    /// If a any part of a match is a symbolic link, and you've not explicitly
    ///  permitted symlinks, then it will be ignored, and the search for a match
    ///  will continue.<br/><br/>
    ///
    /// If you specify a fake directory that only exists as a mount point, it'll
    ///  be associated with the first archive mounted there, even though that
    /// directory isn't necessarily contained in a real archive.
    /// </remarks>
    /// <param name="fileName">file to look for.</param>
    /// <returns>
    /// READ ONLY string of element of search path containing the
    /// the file in question.
    /// </returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="OutOfMemoryException"/>
    public static string GetRealDir(string fileName)
    {
        string dir = physfs.PHYSFS_getRealDir(fileName);
        Assert(
            $"Directory for {{0}}{fileName}{{1}}: {{0}}{dir}{{1}}",
            dir == null
        );

        return dir!;
    }

    /// <summary>
    /// Get a file listing of a search path's directory.
    /// </summary>
    /// <remarks>
    /// Matching directories are interpolated. That is, if "C:\mydir" is in the
    /// search path and contains a directory "savegames" that contains "x.sav",
    /// "y.sav", and "z.sav", and there is also a "C:\userdir" in the search path
    /// that has a "savegames" subdirectory with "w.sav"<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Enumerate{T}(string, EnumerateCallback{T}, ref T)"/>
    /// </remarks>
    /// <param name="directory">
    /// directory in platform-independent notation to enumerate.
    /// </param>
    /// <returns>
    /// array of null-terminated strings.
    /// </returns>
    /// <exception cref="OutOfMemoryException"/>
    /// <exception cref="InvalidOperationException"/>
    public static IEnumerable<string> EnumerateFiles(string directory)
    {
        IntPtr files = physfs.PHYSFS_enumerateFiles(directory);
        IntPtr i = files;
        Assert($"Enumerating files within {directory}", files == IntPtr.Zero, false);

        while (true)
        {
            IntPtr file = Marshal.ReadIntPtr(i);
            if (file == IntPtr.Zero) break;

            yield return Marshal.PtrToStringUTF8(file)!;
            i += IntPtr.Size;
        }

        physfs.PHYSFS_freeList(files);
    }

    /// <summary>
    /// Determine if a file exists in the search path.
    /// </summary>
    /// <remarks>
    /// Reports true if there is an entry anywhere in the search path by the
    /// name of (fname).<br/><br/>
    /// 
    /// Note that entries that are symlinks are ignored if
    /// <seealso cref="AllowSymbolicLinks"/> isn't true, so you
    /// might end up further down in the search path than expected.
    /// </remarks>
    /// <param name="fileName">filename in platform-independent notation.</param>
    /// <returns>true if fileName exists. false otherwise.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="OutOfMemoryException"/>
    public static bool Exists(string fileName)
    {
        return physfs.PHYSFS_exists(fileName);
    }

    /// <summary>Open a file for reading or writing, in platform-independent notation.
    /// </summary>
    /// <remarks>
    /// If you are attempting to read the file, the search path will be
    /// checked one at a time until a matching file is found, in which case an
    /// abstract filehandle is associated with it, and reading will be done.
    /// The reading offset is set to the first byte of the file.<br/><br/>
    /// 
    /// If you are attempting to write the file, it is in relation
    /// to the write dir as the root of the writable filesystem. The specified
    /// file is created if it doesn't exist.<br/>
    /// If it does exist, it is either truncated to zero bytes, and the writing
    /// offset is set to the start, or the writing offset is set to the end of
    /// the file, so the first write will be the byte after the end.<br/><br/>
    /// 
    /// Note that entries that are symlinks are ignored if
    /// <seealso cref="AllowSymbolicLinks"/> isn't true, and opening a
    /// symlink with this function will fail in such a case.
    /// </remarks>
    /// <param name="fileName">File to open.</param>
    /// <param name="mode">File access</param>
    /// <returns>PhysicsFS filehandle</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="OutOfMemoryException"/>
    public static FileSystemObject OpenFile(string fileName, FileSystemObjectAccess mode)
    {
        FileHandle handle = mode switch
        {
            FileSystemObjectAccess.Read => physfs.PHYSFS_openRead(fileName),
            FileSystemObjectAccess.Truncate => physfs.PHYSFS_openWrite(fileName),
            FileSystemObjectAccess.Append => physfs.PHYSFS_openAppend(fileName),
            _ => throw new InvalidOperationException($"Cannot open {fileName} with {mode}")
        };

        Assert(
            $"Opening {{0}}{fileName}{{1}} for {{0}}{mode}{{1}}",
            handle.IsInvalid
        );

        return new FileSystemObject
        {
            FullName = fileName,
            Stats = GetStatsFor(fileName),
            Access = mode,
            Handle = handle
        };
    }

    /// <summary>
    /// Close a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// This call is capable of failing if the operating system was buffering
    /// writes to the physical media, and, now forced to write those changes to
    /// physical media, can not store the data for some reason.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    public static void CloseFile(FileSystemObject file)
    {
        Assert(
            $"Closing {{0}}{file.FullName}{{1}}",
            !physfs.PHYSFS_close(file.Handle)
        );
    }

    /// <summary>
    /// Check for end-of-file state on a PhysicsFS file.
    /// </summary>
    /// <params>
    /// Determine if the end of file has been reached in a PhysicsFS file.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="ReadBytes(FileSystemObject)"/><br/>
    /// <seealso cref="Tell(FileSystemObject)"/><br/>
    /// </params>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <returns>true if EOF, false if not</returns>
    public static bool IsEndOfFile(FileSystemObject file)
    {
        return physfs.PHYSFS_eof(file.Handle);
    }

    /// <summary>
    /// Determine current position within a PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// See also:<br/>
    /// <seealso cref="Seek(FileSystemObject, ulong)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <returns>offset in bytes from start of file</returns>
    /// <exception cref="IOException"/>
    public static long Tell(FileSystemObject file)
    {
        long position = physfs.PHYSFS_tell(file.Handle);
        Assert(
            $"Attempting to determine current position within {file.FullName}.",
            position == -1
        );

        return position;
    }

    /// <summary>
    /// Seek to a new position within a PhysicsFS filehandle.
    /// </summary>
    /// <remarks>
    /// The next read or write will occur at that place. Seeking past the
    /// beginning or end of the file is not allowed, and causes an error.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Tell(FileSystemObject)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <param name="offset">
    /// number of bytes from start of file to seek to.
    /// </param>
    /// <exception cref="IOException"/>
    public static void Seek(FileSystemObject file, ulong offset)
    {
        Assert(
            $"Seeking to {offset} within {file.FullName}",
            !physfs.PHYSFS_seek(file.Handle, offset)
        );
    }

    /// <summary>
    /// Get total length of a file in bytes.
    /// </summary>
    /// <remarks>
    /// Note that if another process/thread is writing to this file at the same
    /// time, then the information this function supplies could be incorrect
    /// before you get it. Use with caution, or better yet,
    /// don't use at all.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Tell(FileSystemObject)"/>
    /// <seealso cref="Seek(FileSystemObject, ulong)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <returns>
    /// size in bytes of the file. -1 if can't be determined.
    /// </returns>
    public static ulong FileLength(FileSystemObject file)
    {
        return physfs.PHYSFS_fileLength(file.Handle);
    }

    /// <summary>
    /// Set up buffering for a PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// Define an i/o buffer for a file handle. A memory block of (bufsize) bytes
    /// will be allocated and associated with(handle).<br/><br/>
    /// 
    /// For files opened for reading, up to (bufsize) bytes are read from (handle)
    /// and stored in the internal buffer. Calls to
    /// <seealso cref="ReadBytes(FileSystemObject)"/> will pull
    /// from this buffer until it is empty, and then refill it for more reading.
    /// Note that compressed files, like ZIP archives, will decompress while
    /// buffering, so this can be handy for offsetting CPU-intensive operations.
    /// The buffer isn't filled until you do your next read.<br/><br/>
    ///
    /// For files opened for writing, data will be buffered to memory until the
    /// buffer is full or the buffer is flushed. Closing a handle implicitly
    /// causes a flush...check your return values!<br/><br/>
    ///
    /// Seeking, etc transparently accounts for buffering.<br/><br/>
    ///
    /// You can resize an existing buffer by calling this function more than once
    /// on the same file. Setting the buffer size to zero will free an existing
    /// buffer.<br/><br/>
    ///
    /// PhysicsFS file handles are unbuffered by default.<br/><br/>
    ///
    /// Please check the return value of this function! Failures can include
    /// not being able to seek backwards in a read-only file when removing the
    /// buffer, not being able to allocate the buffer, and not being able to
    /// flush the buffer to disk, among other unexpected problems.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Flush(FileSystemObject)"/><br/>
    /// <seealso cref="ReadBytes(FileSystemObject)"/><br/>
    /// <seealso cref="WriteBytes(FileSystemObject, byte[])"/><br/>
    /// <seealso cref="CloseFile(FileSystemObject)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <param name="size">
    /// size, in bytes, of buffer to allocate.
    /// </param>
    public static void SetBufferSize(FileSystemObject file, ulong size)
    {
        Assert(
            $"Setting the buffer size of {{0}}{file}{{1}} to {{0}}{size}{{1}}",
            !physfs.PHYSFS_setBuffer(file.Handle, size)
        );
    }

    /// <summary>
    /// Flush a buffered PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// For buffered files opened for writing, this will put the current contents
    /// of the buffer to disk and flag the buffer as empty if possible.<br/><br/>
    /// 
    /// For buffered files opened for reading or unbuffered files, this is a safe
    /// no-op, and will report success.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="SetBufferSize(FileSystemObject, ulong)"/><br/>
    /// <seealso cref="CloseFile(FileSystemObject)"/>
    /// </remarks>
    /// <param name="file">
    /// File from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    public static void Flush(FileSystemObject file)
    {
        Assert(
            $"Flushing {{0}}{file}{{1}}",
            !physfs.PHYSFS_flush(file.Handle)
        );
    }

    /// <summary>
    /// Add an archive or directory to the search path.
    /// </summary>
    /// <remarks>
    /// If this is a duplicate, the entry is not added again, even though the
    /// function succeeds. You may not add the same archive to two different
    /// mountpoints: duplicate checking is done against the archive and not the
    /// mountpoint.<br/><br/>
    ///
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
    ///
    /// The mountpoint does not need to exist prior to mounting, which is different
    /// than those familiar with the Unix concept of "mounting" may expect.
    /// As well, more than one archive can be mounted to the same mountpoint, or
    /// mountpoints and archive contents can overlap...the interpolation mechanism
    /// still functions as usual.<br/><br/>
    ///
    /// Specifying a symbolic link to an archive or directory is allowed here,
    /// regardless of the state of <seealso cref="AllowSymbolicLinks"/>. That function
    /// only deals with symlinks inside the mounted directory or archive.<br/><br/>
    /// 
    /// See also:<br/>
    /// <seealso cref="Unmount(string, bool)"/>
    /// <seealso cref="GetSearchPaths"/>
    /// <seealso cref="GetSearchPaths"/>
    /// <seealso cref="GetMountPoint(string, bool)"/>
    /// </remarks>
    /// <param name="directory">
    /// directory or archive to add to the path, in
    /// </param>
    /// <param name="isRelative">
    /// true if <paramref name="directory"/> is a relative path, false if full.
    /// </param>
    /// <param name="mountTo">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// String.Empty is equivalent to "/".
    /// </param>
    /// <param name="append">
    /// true to append to search path, false to prepend.
    /// </param>
    public static void Mount(
        string directory,
        bool isRelative = true,
        string mountTo = "/",
        bool append = true)
    {
        if (isRelative) directory = string.Join(DirSeparator, BasePath, directory);
        Assert(
            $"Adding {{0}}{directory}{{1}} to search paths to {{0}}{mountTo}{{1}}",
            !physfs.PHYSFS_mount(directory, mountTo, append)
        );
    }

    /// <summary>
    /// Add an archive, contained in <seealso cref="FileSystemObject"/>,
    /// to the search path.
    /// </summary>
    /// <remarks>
    /// This function operates just like <seealso cref="Mount(string, bool, string, bool)"/>,
    /// but takes a <seealso cref="FileSystemObject"/> instead of a pathname.<br/><br/>
    /// 
    /// This handle contains all the data of the
    /// archive, and is used instead of a real file in the physical filesystem.
    /// The <seealso cref="FileSystemObject"/> may be backed by a real
    /// file in the physical filesystem, but isn't necessarily.
    /// The most popular use for this is likely to mount
    /// archives stored inside other archives.<br/><br/>
    ///
    /// <paramref name="directory"/> must be a unique string to identify this archive.
    /// It is used to optimize archiver selection (if you name it XXXXX.zip, we might try
    /// the ZIP archiver first, for example, or directly choose an archiver that
    /// can only trust the data is valid by filename extension). It doesn't
    /// need to refer to a real file at all. If the filename extension isn't
    /// helpful, the system will try every archiver until one works or none
    /// of them do. This filename must be unique, as the system won't allow you
    /// to have two archives with the same name.<br/><br/>
    ///
    /// <paramref name="file"/> must remain until the archive is unmounted.
    /// When the archive is unmounted, the system will call
    /// <seealso cref="CloseFile(FileSystemObject)"/>.<br/><br/>
    /// 
    /// If you need this
    /// handle to survive, you will have to wrap this in a PHYSFS_Io and use
    /// PHYSFS_mountIo() instead.
    /// </remarks>
    /// <param name="file">
    /// The <seealso cref="FileSystemObject"/> containing archive data.
    /// </param>
    /// <param name="directory">
    /// Filename that can represent this stream.
    /// </param>
    /// <param name="isRelative">
    /// true if <paramref name="directory"/> is a relative path, false if full.
    /// </param>
    /// <param name="mountTo">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// String.Empty is equivalent to "/".
    /// </param>
    /// <param name="append">
    /// true to append to search path, false to prepend.
    /// </param>
    public static void Mount(
        FileSystemObject file,
        string directory,
        bool isRelative = true,
        string mountTo = "/",
        bool append = true)
    {
        if (isRelative) directory = string.Join(DirSeparator, BasePath, directory);
        Assert(
            $"Adding {{0}}{file.FullName}{{1}} to search paths to {{0}}{mountTo}{{1}}",
            !physfs.PHYSFS_mountHandle(file.Handle, directory, mountTo, append)
        );
    }

    /// <summary>
    /// Add an archive, contained in a memory buffer, to the search path.
    /// </summary>
    /// <param name="buffer">
    /// buffer of bytes to mount.
    /// </param>
    /// <param name="onUnmount">
    /// A callback that triggers upon unmount. Can be null.
    /// </param>
    /// <param name="directory">
    /// Filename that can represent this stream.
    /// </param>
    /// <param name="isRelative">
    /// true if <paramref name="directory"/> is a relative path, false if full.
    /// </param>
    /// <param name="mountTo">
    /// Location in the interpolated tree that this archive
    /// will be "mounted", in platform-independent notation.
    /// </param>
    /// <param name="append">
    /// true to append to search path, false to prepend.
    /// </param>
    public static void Mount(
        byte[] buffer,
        string directory,
        bool isRelative = true,
        string mountTo = "/",
        bool append = true,
        Action<IntPtr>? onUnmount = null)
    {
        GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr address = pinned.AddrOfPinnedObject();

        if (isRelative) directory = string.Join(DirSeparator, BasePath, directory);
        Assert(
            $"Adding {{0}}{buffer}{{1}} buffer of {{0}}{buffer.LongLength}{{1}} " +
            $"to search paths to {{0}}{mountTo}{{1}}",
            !physfs.PHYSFS_mountMemory(
                address,
                (ulong)buffer.LongLength,
                buf => onUnmount?.Invoke(buf),
                directory,
                mountTo,
                append
            )
        );

        pinned.Free();
    }

    /// <summary>
    /// Determine a mounted archive's mountpoint.
    /// </summary>
    /// <remarks>
    /// You give this function the name of an archive or dir you successfully
    /// added to the search path, and it reports the location in the interpolated
    /// tree where it is mounted. Files mounted with a "/" mountpoint or through
    /// <seealso cref="Mount(string, bool, string, bool)"/>
    /// will report "/". The return value is valid until the archive is
    /// removed from the search path.
    /// </remarks>
    /// <param name="directory">
    /// directory or archive previously added to the path, in
    /// platform-dependent notation. This must match the string
    /// used when adding, even if your string would also reference
    /// the same file with a different string of characters.
    /// </param>
    /// <param name="isRelative">
    /// true if <paramref name="directory"/> is a relative path, false if full.
    /// </param>
    /// <returns>
    /// string of mount point if added to path
    /// </returns>
    public static string GetMountPoint(string directory, bool isRelative = true)
    {
        if (isRelative) directory = string.Join(DirSeparator, BasePath, directory);
        string? mountPoint = physfs.PHYSFS_getMountPoint(directory);

        Assert(
            $"Getting the mount point of {{0}}{directory}{{1}}",
            mountPoint == null
        );

        return mountPoint!;
    }

    /// <summary>
    /// Get a file listing of a search path's directory,
    /// using an application-defined callback, with errors reported.
    /// </summary>
    /// <remarks>
    /// Items sent to the callback are not guaranteed to be in any order whatsoever.
    /// There is no sorting done at this level, and if you need that, you should
    /// probably use <seealso cref="EnumerateFiles(string)"/> instead, which guarantees
    /// alphabetical sorting. This form reports whatever is discovered in each
    /// archive before moving on to the next. Even within one archive, we can't
    /// guarantee what order it will discover data. <em>Any sorting you find in
    /// these callbacks is just pure luck. Do not rely on it.</em> As this walks
    /// the entire list of archives, you may receive duplicate filenames.<br/><br/>
    /// 
    /// This API and the callbacks themselves are capable of reporting errors.
    /// Prior to this API, callbacks had to accept every enumerated item, even if
    /// they were only looking for a specific thing and wanted to stop after that,
    /// or had a serious error and couldn't alert anyone. Furthermore, if
    /// PhysicsFS itself had a problem (disk error or whatnot), it couldn't report
    /// it to the calling app, it would just have to skip items or stop
    /// enumerating outright, and the caller wouldn't know it had lost some data
    /// along the way.<br/><br/>
    /// 
    /// Now the caller can be sure it got a complete data set, and its callback has
    /// control if it wants enumeration to stop early. See the documentation for
    /// <seealso cref="EnumerateCallback{T}"/> for details on how your callback should behave.
    /// </remarks>
    /// <typeparam name="T">
    /// Type of <paramref name="data"/> passed to the
    /// <paramref name="callback"/>.
    /// </typeparam>
    /// <param name="directory">
    /// Directory, in platform-independent notation, to enumerate.
    /// </param>
    /// <param name="callback">
    /// Callback function to notify about search path elements.
    /// </param>
    /// <param name="data">
    /// Application-defined data passed to callback.
    /// </param>
    public static void Enumerate<T>(string directory, EnumerateCallback<T> callback, ref T data)
    {
        unsafe PHYSFS_EnumerateCallbackResult Callback(IntPtr data, string origdir, string fname)
        {
            ref T str = ref Unsafe.AsRef<T>(data.ToPointer());
            EnumerateCallbackResult result = callback(ref str, origdir, fname);
            return (PHYSFS_EnumerateCallbackResult)result;
        }
        unsafe
        {
            void* targetRef = Unsafe.AsPointer(ref data);
            IntPtr handle = new IntPtr(targetRef);
            Assert(
                $"Enumerating files in {directory}",
                !physfs.PHYSFS_enumerate(directory, Callback, handle)
            );
        }
    }

    /// <summary>
    /// Remove a directory or archive from the search path.
    /// </summary>
    /// <remarks>
    /// This must be a (case-sensitive) match to a dir or archive already in the
    /// search path, specified in platform-dependent notation.<br/><br/>
    /// 
    /// This call will fail (and fail to remove from the path) if the element still
    /// has files open in it.
    /// </remarks>
    /// <param name="directory">
    /// Dir/archive to remove.
    /// </param>
    /// <param name="isRelative">
    /// true if <paramref name="directory"/> is a relative path, false if full.
    /// </param>
    public static void Unmount(string directory, bool isRelative = true)
    {
        if (isRelative) directory = string.Join(DirSeparator, BasePath, directory);
        Assert(
            $"Removing {{0}}{directory}{{1}} from search paths",
            !physfs.PHYSFS_unmount(directory)
        );
    }

    /// <summary>
    /// Get various information about a directory or a file.
    /// </summary>
    /// <remarks>
    /// Obtain various information about a file or directory from the meta data.<br/><br/>
    /// 
    /// This function will never follow symbolic links. If you haven't enabled
    /// symlinks by setting <seealso cref="AllowSymbolicLinks"/>, stat'ing a symlink will be
    /// treated like stat'ing a non-existant file. If symlinks are enabled,
    /// stat'ing a symlink will give you information on the link itself and not
    /// what it points to.
    /// </remarks>
    /// <param name="fileName">
    /// Filename to check, in platform-indepedent notation.
    /// </param>
    /// <returns>
    /// Structure filled with data about <paramref name="fileName"/>.
    /// </returns>
    public static FileStat GetStatsFor(string fileName)
    {
        Assert(
            $"Getting the file statistics of {{0}}{fileName}{{1}}",
            !physfs.PHYSFS_stat(fileName, out PHYSFS_Stat stat)
        );

        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
        return new FileStat
        {
            Length = stat.filesize,
            LastWriteTime = dateTime.AddSeconds(stat.modtime),
            CreationTime = dateTime.AddSeconds(stat.createtime),
            LastAccessTime = dateTime.AddSeconds(stat.accesstime),
            Type = (FileType)stat.filetype,
            IsReadOnly = stat.isreadonly == 1
        };
    }

    /// <summary>
    /// Read bytes from a PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.
    /// </remarks>
    /// <param name="file">
    /// Handle returned from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <returns>
    /// The bytes read from file. Throws a warning if the number of bytes read
    /// is less than file length; this does not
    /// signify an error, necessarily (a short read may mean EOF).
    /// </returns>
    public static byte[] ReadBytes(FileSystemObject file)
    {
        byte[] buffer = new byte[file.Stats.Length];
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            IntPtr address = handle.AddrOfPinnedObject();
            ulong len = (ulong)file.Stats.Length;

            long read = physfs.PHYSFS_readBytes(file.Handle, address, len);
            Assert(
                $"Reading {read} bytes from {file.FullName}",
                read == -1,
                read < file.Stats.Length
            );
        }
        finally
        {
            handle.Free();
        }

        return buffer;
    }

    /// <summary>
    /// Reads a PhysicsFS file to a stream.
    /// </summary>
    /// <remarks>
    /// The file must be opened for reading.
    /// </remarks>
    /// <param name="file">
    /// Handle returned from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <returns>
    /// An unmanaged stream. Throws a warning if the number of bytes read
    /// is less than file length; this does not
    /// signify an error, necessarily (a short read may mean EOF).
    /// </returns>
    public static UnmanagedMemoryStream ReadToStream(FileSystemObject file)
    {
        SafeHGlobalBuffer buffer = new SafeHGlobalBuffer(file.Stats.Length);
        long read = physfs.PHYSFS_readBytes(file.Handle, buffer.DangerousGetHandle(), file.Length);

        Assert(
            $"Reading {read} bytes from {file.FullName}",
            read == -1,
            read < file.Stats.Length
        );

        UnmanagedMemoryStream stream = new UnmanagedMemoryStream(buffer, 0, read, FileAccess.Read);
        return stream;
    }

    /// <summary>
    /// Write data to a PhysicsFS file.
    /// </summary>
    /// <remarks>
    /// Please note that you are limited to 63 bits (9223372036854775807 bytes),
    /// so we can return a negative value on error. If length is greater than
    /// 0x7FFFFFFFFFFFFFFF, this function will immediately fail. For systems
    /// without a 64-bit datatype, you are limited to 31 bits (0x7FFFFFFF, or
    /// 2147483647 bytes). We trust most things won't need to do multiple
    /// gigabytes of i/o in one call anyhow, but why limit things?
    /// </remarks>
    /// <param name="file">
    /// handle from <seealso cref="OpenFile(string, FileSystemObjectAccess)"/>
    /// </param>
    /// <param name="bytes">
    /// buffer of bytes to write to <paramref name="file"/>.
    /// </param>
    /// <returns>
    /// number of bytes written. This may be less than (len); in the case
    /// of an error, the system may try to write as many bytes as possible,
    /// so an incomplete write might occur.
    /// </returns>
    public static long WriteBytes(FileSystemObject file, byte[] bytes)
    {
        GCHandle pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        IntPtr address = pinned.AddrOfPinnedObject();

        long write = physfs.PHYSFS_writeBytes(
            file.Handle,
            address,
            (ulong)bytes.LongLength
        );

        Assert(
            $"Writing {bytes.LongLength} bytes to {file.FullName}",
            write == -1,
            write < bytes.LongLength
        );

        pinned.Free();
        return write;
    }

    /// <summary>
    /// Get the user-and-app-specific path where files can be written.
    /// </summary>
    /// <remarks>
    /// Helper function.<br/><br/>
    ///
    /// Get the "pref dir". This is meant to be where users can write personal
    /// files (preferences and save games, etc) that are specific to your
    /// application. This directory is unique per user, per application.<br/><br/>
    ///
    /// This function will decide the appropriate location in the native filesystem,
    /// create the directory if necessary, and return a string in
    /// platform-dependent notation, suitable for passing to
    /// <seealso cref="SetWriteDirectory(string)"/><br/><br/>
    ///
    /// On Windows, this might look like:<br/>
    /// "C:\\Users\\bob\\AppData\\Roaming\\My Company\\My Program Name"<br/><br/>
    ///
    /// On Linux, this might look like:<br/>
    /// "/home/bob/.local/share/My Program Name"<br/><br/>
    ///
    /// On Mac OS X, this might look like:<br/>
    /// "/Users/bob/Library/Application Support/My Program Name"<br/><br/>
    ///
    /// You should probably use the pref dir for your write dir, and also put it
    /// near the beginning of your search path. This finds the correct location
    /// for whatever platform, which not only changes between operating systems,
    /// but also versions of the same operating system.<br/><br/>
    ///
    /// You specify the name of your organization (if it's not a real organization,
    /// your name or an Internet domain you own might do) and the name of your
    /// application. These should be proper names.<br/><br/>
    ///
    /// Both the <paramref name="orgName"/> and <paramref name="appName"/>
    /// strings may become part of a directory name, so
    /// please follow these rules:<br/>
    /// - Try to use the same org string (including case-sensitivity) for
    ///   all your applications that use this function.<br/>
    /// - Always use a unique app string for each one, and make sure it never
    ///   changes for an app once you've decided on it.<br/>
    /// - Unicode characters are legal, as long as it's UTF-8 encoded, but...
    /// - ...only use letters, numbers, and spaces. Avoid punctuation like
    ///   "Game Name 2: Bad Guy's Revenge!" ... "Game Name 2" is sufficient.<br/><br/>
    ///
    /// The pointer returned by this function remains valid until you call this
    /// function again, or call <seealso cref="Deinit"/>. This is not necessarily a fast
    /// call, though, so you should call this once at startup and copy the string
    /// if you need it.<br/><br/>
    ///
    /// You should assume the path returned by this function is the only safe
    /// place to write files (and <seealso cref="BasePath"/>,
    /// while they might be writable, or even parents of the returned path, aren't
    /// where you should be writing things).<br/><br/>
    /// </remarks>
    /// <param name="orgName">
    /// The name of your organization.
    /// </param>
    /// <param name="appName">
    /// The name of your application.
    /// </param>
    /// <returns>
    /// string of user dir in platform-dependent notation
    /// </returns>
    public static string GetPrefDirectory(string orgName, string appName)
    {
        string pref = physfs.PHYSFS_getPrefDir(orgName, appName);
        Assert(
            $"Preferences directory for {{0}}[{orgName}]{{1}}," +
            $"{{0}}[{appName}]{{1}}: {{0}}{pref}{{1}}",
            pref == null
        );

        return pref!;
    }

    /// <summary>
    /// Make a subdirectory of an archive its root directory.
    /// </summary>
    /// <remarks>
    /// This lets you narrow down the accessible files in a specific archive.
    /// For example, if you have x.zip with a file in y/z.txt, mounted to /a,
    /// if you call <c>PhysFS.SetRootDirectory("x.zip", "/y")</c>,
    /// then the call <c>PhysFS.OpenFile("/a/z.txt", FileSystemObjectAccess.Read)</c>
    /// will succeed.<br/><br/>
    /// 
    /// You can change an archive's root at any time, altering the interpolated
    /// file tree (depending on where paths shift, a different archive may be
    /// providing various files). If you set the root to NULL or "/", the
    /// archive will be treated as if no special root was set (as if the archive
    /// was just mounted normally).<br/><br/>
    ///
    /// Changing the root only affects future operations on pathnames; a file
    /// that was opened from a path that changed due to a setRoot will not be
    /// affected.<br/><br/>
    ///
    /// Setting a new root is not limited to archives in the search path; you may
    /// set one on the write dir, too, which might be useful if you have files
    /// open for write and thus can't change the write dir at the moment.<br/><br/>
    ///
    /// It is not an error to set a subdirectory that does not exist to be the
    /// root of an archive; however, no files will be visible in this case. If
    /// the missing directories end up getting created (a mkdir to the physical
    /// filesystem, etc) then this will be reflected in the interpolated tree.
    /// </remarks>
    /// <param name="archive">
    /// dir/archive on which to change root.
    /// </param>
    /// <param name="subdirectory">
    /// new subdirectory to make the root of this archive.
    /// </param>
    public static void SetRootDirectory(string archive, string subdirectory = "/")
    {
        Assert(
            $"Setting {{0}}{subdirectory}{{1}} as {{0}}/{{1}}" +
            $"for {{0}}{{archive}}{{1}}",
            !physfs.PHYSFS_setRoot(archive, subdirectory)
        );
    }

    private static Version GetLinkedVersion()
    {
        physfs.PHYSFS_getLinkedVersion(out PHYSFS_Version version);
        return new Version(version.major, version.minor, version.patch);
    }

    private static void Assert(string message, bool failExpression, bool warnExpression = false)
    {
        (PHYSFS_ErrorCode, string) LogError()
        {
            PHYSFS_ErrorCode code = physfs.PHYSFS_getLastErrorCode();
            string err = physfs.PHYSFS_getErrorByCode(code);

        #if DEBUG
            (string tag, string color) = failExpression ?
                ("FAIL", FG_LRED) : ("WARN", FG_LYELLOW);

            string output = $"{{0}}{tag}{{1}} {message}: {{0}}{err}{{1}}";
            Log(color, output, REVERSE, NOREVERSE);
        #endif

            return (code, err);
        }

        if (failExpression)
        {
            (var code, var err) = LogError();
            ExceptionUtility.ThrowExceptionForPhysFSErr(code, err);
        }
        else if (warnExpression)
        {
            LogError();
        }
        else
        {
        #if DEBUG
            Log(FG_LGREEN, $"{{0}}SUCC{{1}} {message}", REVERSE, NOREVERSE);
        #endif
        }
    }

#if DEBUG
    // ANSI output.
    private static readonly string NL = Environment.NewLine;
    private static readonly string NORMAL = "\x1b[0m";
    private static readonly string BOLD = "\x1b[1m";
    private static readonly string UNDERLINE = "\x1b[4m";
    private static readonly string BLINK = "\x1b[5m";
    private static readonly string REVERSE = "\x1b[7m";
    private static readonly string NOBOLD = "\x1b[22m";
    private static readonly string NOUNDERLINE = "\x1b[24m";
    private static readonly string NOBLINK = "\x1b[25m";
    private static readonly string NOREVERSE = "\x1b[27m";
    private static readonly string FG_DBLACK = "\x1b[30m";
    private static readonly string FG_DRED = "\x1b[31m";
    private static readonly string FG_DGREEN = "\x1b[32m";
    private static readonly string FG_DYELLOW = "\x1b[33m";
    private static readonly string FG_DBLUE = "\x1b[34m";
    private static readonly string FG_DMAGENTA = "\x1b[35m";
    private static readonly string FG_DCYAN = "\x1b[36m";
    private static readonly string FG_DWHITE = "\x1b[37m";
    private static readonly string FG_DEFAULT = "\x1b[39m";
    private static readonly string FG_LGRAY = "\x1b[90m";
    private static readonly string FG_LRED = "\x1b[91m";
    private static readonly string FG_LGREEN = "\x1b[92m";
    private static readonly string FG_LYELLOW = "\x1b[93m";
    private static readonly string FG_LBLUE = "\x1b[94m";
    private static readonly string FG_LMAGENTA = "\x1b[95m";
    private static readonly string FG_LCYAN = "\x1b[96m";
    private static readonly string FG_LWHITE = "\x1b[97m";
    private static readonly string BG_DBLACK = "\x1b[40m";
    private static readonly string BG_DRED = "\x1b[41m";
    private static readonly string BG_DGREEN = "\x1b[42m";
    private static readonly string BG_DYELLOW = "\x1b[43m";
    private static readonly string BG_DBLUE = "\x1b[44m";
    private static readonly string BG_DMAGENTA = "\x1b[45m";
    private static readonly string BG_DCYAN = "\x1b[46m";
    private static readonly string BG_DWHITE = "\x1b[47m";
    private static readonly string BG_DEFAULT = "\x1b[49m";
    private static readonly string BG_LGRAY = "\x1b[100m";
    private static readonly string BG_LRED = "\x1b[101m";
    private static readonly string BG_LGREEN = "\x1b[102m";
    private static readonly string BG_LYELLOW = "\x1b[103m";
    private static readonly string BG_LBLUE = "\x1b[104m";
    private static readonly string BG_LMAGENTA = "\x1b[105m";
    private static readonly string BG_LCYAN = "\x1b[106m";
    private static readonly string BG_LWHITE = "\x1b[107m";

    /// <summary>
    /// Writes a log to the console and the log file.
    /// </summary>
    /// <param name="color">Main color used for the log message in the console</param>
    /// <param name="message">Message contents</param>
    /// <param name="args">Additional arguments used for formatting (only in console)</param>
    private static void Log(string color, string message, params object[] args)
    {
        string time = "[" + DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss::ffff") + "]";
        string log = $"{time} [PhysFS] {message}.";
        string formatted = string.Format(color + log + NORMAL, args);
        Console.WriteLine(Console.IsOutputRedirected ? log : formatted);

        string[] emptyArgs = new string[args.Length];
        for (byte i = 0; i < args.Length; i++)
        {
            emptyArgs[i] = string.Empty;
        }

        string fileContent = string.Format(log, emptyArgs);
        File.AppendAllText("log", fileContent + NL);
    }
#endif
}
