
using System.Runtime.InteropServices;
using System.Text;

namespace Icculus.PhysFS.NET;

public static class PhysFsTest
{
    delegate string? CommandDelegate(string[] args);
    struct Command
    {
        public string[] Args;
        public CommandDelegate Action;
    }

    static readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>
    {
        { "quit", new Command { Args = [], Action = Cmd_quit } },
        { "q", new Command { Args = [], Action = Cmd_quit } },
        { "help", new Command { Args = [], Action = Cmd_help } },
        { "init", new Command { Args = ["argv0"], Action = Cmd_init } },
        { "deinit", new Command { Args = [], Action = Cmd_deinit } },
        { "addarchive", new Command { Args = ["archiveLocation", "append"], Action = Cmd_addarchive } },
        { "mount", new Command { Args = ["archiveLocation", "mntpoint", "append"], Action = Cmd_mount } },
        { "mountmem", new Command { Args = ["archiveLocation", "mntpoint", "append"], Action = Cmd_mountmem } },
        { "mounthandle", new Command { Args = ["archiveLocation", "mntpoint", "append"], Action = Cmd_mounthandle } },
        { "removearchive", new Command { Args = ["archiveLocation"], Action = Cmd_unmount } },
        { "unmount", new Command { Args = ["archiveLocation"], Action = Cmd_unmount } },
        { "enumerate", new Command { Args = ["dirToEnumerate"], Action = Cmd_enumerate } },
        { "ls", new Command { Args = ["dirToEnumerate"], Action = Cmd_enumerate } },
        { "tree", new Command { Args = ["dirToEnumerate"], Action = Cmd_tree } },
        { "getlasterror", new Command { Args = [], Action = Cmd_getlasterror } },
        { "getdirsep", new Command { Args = [], Action = Cmd_getdirsep } },
        { "getcdromdirs", new Command { Args = [], Action = Cmd_getcdromdirs } },
        { "getsearchpath", new Command { Args = [], Action = Cmd_getsearchpath } },
        { "getbasedir", new Command { Args = [], Action = Cmd_getbasedir } },
        { "getuserdir", new Command { Args = [], Action = Cmd_getuserdir } },
        { "getprefdir", new Command { Args = ["org", "app"], Action = Cmd_getprefdir } },
        { "getwritedir", new Command { Args = [], Action = Cmd_getwritedir } },
        { "setwritedir", new Command { Args = ["newWriteDir"], Action = Cmd_setwritedir } },
        { "permitsymlinks", new Command { Args = ["1or0"], Action = Cmd_permitsyms } },
        { "setsaneconfig", new Command { Args = ["org", "appName", "arcExt", "includeCdRoms", "archivesFirst"], Action = Cmd_setsaneconfig } },
        { "mkdir", new Command { Args = ["dirToMk"], Action = Cmd_mkdir } },
        { "delete", new Command { Args = ["dirToDelete"], Action = Cmd_delete } },
        { "getrealdir", new Command { Args = ["fileToFind"], Action = Cmd_getrealdir } },
        { "exists", new Command { Args = ["fileToCheck"], Action = Cmd_exists } },
        { "isdir", new Command { Args = ["fileToCheck"], Action = Cmd_isdir } },
        { "issymlink", new Command { Args = ["fileToCheck"], Action = Cmd_issymlink } },
        { "cat", new Command { Args = ["fileToCat"], Action = Cmd_cat } },
        { "cat2", new Command { Args = ["fileToCat1", "fileToCat2"], Action = Cmd_cat2 } },
        { "filelength", new Command { Args = ["fileToCheck"], Action = Cmd_filelength } },
        { "stat", new Command { Args = ["stat"], Action = Cmd_stat } },
        { "append", new Command { Args = ["fileToAppend"], Action = Cmd_append } },
        { "write", new Command { Args = ["fileToCreateOrTrash"], Action = Cmd_write } },
        { "getlastmodtime", new Command { Args = ["fileToExamine"], Action = Cmd_getlastmodtime } },
        { "setbuffer", new Command { Args = ["buffersize"], Action = Cmd_setbuffer } },
        { "stressbuffer", new Command { Args = ["bufferSize"], Action = Cmd_stressbuffer } },
        { "crc32", new Command { Args = ["fileToHash" ], Action = Cmd_crc32 } },
        { "getmountpoint", new Command { Args = ["dir"], Action = Cmd_getmountpoint } },
        { "setroot", new Command { Args = ["archiveLocation", "root"], Action = Cmd_setroot } }
    };

    static ulong _bufferSize = 0;

    static string? Cmd_quit(string[] args)
    {
        return null;
    }

    static string Cmd_help(string[] args)
    {
        return "Commands:\n" + string.Join(
            '\n',
            Array.ConvertAll(
                _commands.ToArray(),
                i =>
                {
                    if (i.Value.Args.Length == 0)
                    {
                        return $" - \"{i.Key}\" (no arguments)";
                    }
                    else
                    {
                        string[] args = Array.ConvertAll(i.Value.Args, j => $"<{j}>");
                        string argsStr = string.Join(' ', args);
                        return $" - \"{i.Key} {argsStr}\"";
                    }
                }
            )
        );
    }

    static string Cmd_init(string[] args)
    {
        PhysicsFS.Init(args[0]);
        return "Successful.";
    }

    static string Cmd_deinit(string[] args)
    {
        PhysicsFS.Deinit();
        return "Successful.";
    }

    static string Cmd_addarchive(string[] args)
    {
        PhysicsFS.Mount(args[0], null, args[1] == "1");
        return "Successful.";
    }

    static string Cmd_mount(string[] args)
    {
        PhysicsFS.Mount(args[0], args[1], args[2] == "1");
        return "Successful.";
    }

    static string Cmd_mountmem(string[] args)
    {
        PhysFsFileHandle fhandle = PhysicsFS.OpenRead(args[0]);
        long len = PhysicsFS.FileLength(fhandle);

        if (len < 0)
        {
            return "File size could not be determined.";
        }

        IntPtr handle = Marshal.AllocHGlobal((int)len);
        PhysicsFS.ReadBytes(fhandle, handle, (ulong)len);
        PhysicsFS.MountMemory(handle, (ulong)len, Marshal.FreeHGlobal,
            args[0], args[1], args[2] == "1");

        return "Successful.";
    }

    static string Cmd_mounthandle(string[] args)
    {
        PhysFsFileHandle fhandle = PhysicsFS.OpenRead(args[0]);
        PhysicsFS.MountHandle(fhandle, args[0], args[1], args[2] == "1");
        return "Successful.";
    }

    static string Cmd_unmount(string[] args)
    {
        PhysicsFS.Unmount(args[0]);
        return "Successful.";
    }

    static string Cmd_enumerate(string[] args)
    {
        return Cmd_enumerate_inner(args[0], false,
            out int dirs, out int files, out int others) +
                "\n" + $" total ({dirs}) directories," +
                $" ({files}) files, ({others}) others.";
    }

    static string Cmd_tree(string[] args)
    {
        return Cmd_enumerate_inner(args[0], true,
            out int dirs, out int files, out int others) +
                "\n" + $" total ({dirs}) directories," +
                $" ({files}) files, ({others}) others.";
    }

    static string Cmd_enumerate_inner(string dir, bool recursive,
        out int dirCount, out int fileCount, out int others)
    {
        dirCount = 0;
        fileCount = 0;
        others = 0;

        StringBuilder output = new StringBuilder();
        var files = PhysicsFS.EnumerateFiles(dir);

        foreach (string file in files)
        {
            var curFile = (dir + "/" + file).Replace("//", "/");
            output.AppendLine(curFile);

            var stat = PhysicsFS.Stat(curFile);
            switch (stat.FileType)
            {
                case PhysFsFileType.Directory:
                    if (recursive)
                    {
                        output.Append(
                            Cmd_enumerate_inner(curFile, true,
                                out int dirCountInner, out int fileCountInner, out int othersInner));

                        dirCount += dirCountInner;
                        fileCount += fileCountInner;
                        others += othersInner;
                    }

                    dirCount += 1;
                    break;

                case PhysFsFileType.Regular:
                    fileCount += 1;
                    break;

                case PhysFsFileType.Unknown:
                    others += 1;
                    break;
            }
        }

        return output.ToString();
    }

    static string Cmd_getlasterror(string[] args)
    {
#pragma warning disable CS0618
        string err = PhysicsFS.GetLastError() ?? "(null)";
#pragma warning restore CS0618
        return $"last error is [{err}].";
    }

    static string Cmd_getdirsep(string[] args)
    {
        return $"Directory separator is [{PhysicsFS.DirSeparator}].";
    }

    static string Cmd_getcdromdirs(string[] args)
    {
        var dirs = PhysicsFS.GetCdRomDirs();
        string output = "";
        int count = 0;

        foreach (string dir in dirs)
        {
            output += dir + '\n';
            count++;
        }

        return output + $"\n total ({count}) drives.";
    }

    static string Cmd_getsearchpath(string[] args)
    {
        var dirs = PhysicsFS.GetSearchPath();
        string output = "";
        int count = 0;

        foreach (string dir in dirs)
        {
            output += dir + '\n';
            count++;
        }

        return output + $"\n total ({count}) directories.";
    }

    static string Cmd_getbasedir(string[] args)
    {
        return $"Base dir is [{PhysicsFS.BaseDir}].";
    }

    static string Cmd_getuserdir(string[] args)
    {
#pragma warning disable CS0618
        return $"User dir is [{PhysicsFS.GetUserDir()}].";
#pragma warning restore CS0618
    }

    static string Cmd_getprefdir(string[] args)
    {
        return $"Pref dir is [{PhysicsFS.GetPrefDir(args[0], args[1])}].";
    }

    static string Cmd_getwritedir(string[] args)
    {
        string writeDir = PhysicsFS.GetWriteDir() ?? "(null)";
        return $"Write dir is [{writeDir}].";
    }

    static string Cmd_setwritedir(string[] args)
    {
        PhysicsFS.SetWriteDir(args[0]);
        return "Successful.";
    }

    static string Cmd_permitsyms(string[] args)
    {
        bool num = args[0] == "1";
        PhysicsFS.SymbolicLinksPermitted = num;
        return string.Format("Symlinks are now {0}", num ? "permitted" : "forbidden");
    }

    static string Cmd_setsaneconfig(string[] args)
    {
        PhysicsFS.SetSaneConfig(args[0], args[1], args[2], args[3] == "1", args[4] == "1");
        return "Successful.";
    }

    static string Cmd_mkdir(string[] args)
    {
        PhysicsFS.Mkdir(args[0]);
        return "Successful.";
    }

    static string Cmd_delete(string[] args)
    {
        PhysicsFS.Delete(args[0]);
        return "Successful.";
    }

    static string Cmd_getrealdir(string[] args)
    {
        string? realDir = PhysicsFS.GetRealDir(args[0]);
        return realDir is not null ? $"Found at [{realDir}]." : "Not found.";
    }

    static string Cmd_exists(string[] args)
    {
        return PhysicsFS.Exists(args[0]) ? "File exists." : "File does not exist.";
    }

    static string Cmd_isdir(string[] args)
    {
#pragma warning disable CS0618
        string isDir = PhysicsFS.IsDirectory(args[0]) ? "is" : "is NOT";
#pragma warning restore CS0618
        return $"File {isDir} a directory.";
    }

    static string Cmd_issymlink(string[] args)
    {
#pragma warning disable CS0618
        string isDir = PhysicsFS.IsSymbolicLink(args[0]) ? "is" : "is NOT";
#pragma warning restore CS0618
        return $"File {isDir} a symlink.";
    }

    static string Cmd_cat(string[] args) => Cmd_cat_internal(args[0]);

    static string Cmd_cat2(string[] args)
    {
        string file1 = Cmd_cat_internal(args[0]);
        string file2 = Cmd_cat_internal(args[1]);
        return $"file {args[0]}...\n" + file1 + $"\n\nfile {args[1]}...\n" + file2;
    }
    
    static string Cmd_cat_internal(string fpath)
    {
        using PhysFsFileHandle handle = PhysicsFS.OpenRead(fpath);
        if (_bufferSize > 0)
        {
            PhysicsFS.SetBuffer(handle, _bufferSize);
        }

        long len = PhysicsFS.FileLength(handle);
        byte[] bytes = new byte[len];
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        PhysicsFS.ReadBytes(handle, gcHandle.AddrOfPinnedObject(), (ulong)bytes.LongLength);

        gcHandle.Free();
        return Encoding.UTF8.GetString(bytes);
    }

    static string Cmd_filelength(string[] args)
    {
        PhysFsFileHandle handle = PhysicsFS.OpenRead(args[0]);
        long length = PhysicsFS.FileLength(handle);
        return $" {length} bytes.";
    }

    static string Cmd_stat(string[] args)
    {
        PhysFsStat stat = PhysicsFS.Stat(args[0]);
        return $"Filename: {args[0]}\n" +
            $"Size: {stat.FileSize}\n" +
            $"Type: {stat.FileType}\n" +
            $"Created at: {stat.CreatedAt}\n" +
            $"Last modified at: {stat.LastModifiedAt}\n" +
            $"Last accessed at: {stat.LastAccessedAt}\n" +
            $"Readonly: {stat.IsReadOnly}";
    }

    static string Cmd_append(string[] args)
    {
        using PhysFsFileHandle handle = PhysicsFS.OpenAppend(args[0]);
        if (_bufferSize > 0)
        {
            PhysicsFS.SetBuffer(handle, _bufferSize);
        }

        byte[] bytes = Encoding.UTF8.GetBytes(string.Join(" ", args[1..]));
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        PhysicsFS.WriteBytes(handle, gcHandle.AddrOfPinnedObject(), (ulong)bytes.LongLength);

        gcHandle.Free();
        return "Successful.";
    }

    static string Cmd_write(string[] args)
    {
        using PhysFsFileHandle handle = PhysicsFS.OpenWrite(args[0]);
        if (_bufferSize > 0)
        {
            PhysicsFS.SetBuffer(handle, _bufferSize);
        }

        byte[] bytes = Encoding.UTF8.GetBytes(string.Join(" ", args[1..]));
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        PhysicsFS.WriteBytes(handle, gcHandle.AddrOfPinnedObject(), (ulong)bytes.LongLength);

        gcHandle.Free();
        return "Successful.";
    }

    static string Cmd_getlastmodtime(string[] args)
    {
        PhysFsStat stat = PhysicsFS.Stat(args[0]);
        return $"Last modified: {stat.LastModifiedAt}.";
    }

    static string Cmd_setbuffer(string[] args)
    {
        _bufferSize = Convert.ToUInt64(args[0]);
        return _bufferSize > 0 ? $"Further tests will set a {_bufferSize} size buffer." :
            "Further tests will NOT use a buffer.";
    }

    const uint RAND_MAX = 0x7fff;

    static string Cmd_stressbuffer(string[] args)
    {
        ulong num = Convert.ToUInt64(args[0]);
        string output = $"Stress testing with {num} byte buffer...\n";

        byte[] buf = Array.ConvertAll("abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray(), i => (byte)i);
        using (PhysFsFileHandle f = PhysicsFS.OpenWrite("test.txt"))
        {
            GCHandle gcHandle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr bufAddr = gcHandle.AddrOfPinnedObject();

            PhysicsFS.SetBuffer(f, num);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    uint right = 1 + (uint)(35.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    uint left = 36 - right;
                    PhysicsFS.WriteBytes(f, bufAddr, left);

                    int rndnum = 1 + (int)(1000.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    if (rndnum == 42) PhysicsFS.Flush(f);

                    PhysicsFS.WriteBytes(f, bufAddr + (nint)left, right);

                    rndnum = 1 + (int)(1000.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    if (rndnum == 42) PhysicsFS.Flush(f);
                }

                PhysicsFS.Flush(f);
            }

            output += " ... test file written ...\n";
        }

        byte[] buf2 = new byte[36];
        using (PhysFsFileHandle f = PhysicsFS.OpenRead("test.txt"))
        {
            GCHandle gcHandle = GCHandle.Alloc(buf2, GCHandleType.Pinned);
            IntPtr bufAddr = gcHandle.AddrOfPinnedObject();

            PhysicsFS.SetBuffer(f, num);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    uint right = 1 + (uint)(35.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    uint left = 36 - right;
                    PhysicsFS.ReadBytes(f, bufAddr, left);

                    int rndnum = 1 + (int)(1000.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    if (rndnum == 42) PhysicsFS.Flush(f);

                    PhysicsFS.ReadBytes(f, bufAddr + (nint)left, right);

                    rndnum = 1 + (int)(1000.0 * new Random().NextDouble() / (RAND_MAX + 1.0));
                    if (rndnum == 42) PhysicsFS.Flush(f);

                    if (!buf2.SequenceEqual(buf))
                    {
                        output += $"readback is mismatched on iterations ({i}, {j})\n";
                        output += $"wanted: [{string.Join(",", buf)}].\n";
                        output += $"got: [{string.Join(",", buf2)}].\n";
                        return output;
                    }
                }

                PhysicsFS.Flush(f);
            }

            if (!PhysicsFS.EOF(f))
            {
                output += "PhysicsFS.EOF() returned false! That's wrong.\n";
            }

            output += " ... test file read ...\n";
        }

        PhysicsFS.Delete("test.txt");
        output += "stress test completed successfully.";
        return output;
    }

    const ushort CRC32_BUFFERSIZE = 512;
    static string Cmd_crc32(string[] args)
    {
        using PhysFsFileHandle handle = PhysicsFS.OpenRead(args[0]);
        byte[] buffer = new byte[CRC32_BUFFERSIZE];
        uint crc = uint.MaxValue;
        long bytesread = 0;

        GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr gcaddr = gch.AddrOfPinnedObject();

        while ((bytesread = PhysicsFS.ReadBytes(handle, gcaddr, CRC32_BUFFERSIZE)) > 0)
        {
            for (uint i = 0; i < bytesread; i++)
            {
                for (uint bit = 0; bit < 8; bit++, buffer[i] >>= 1)
                {
                    crc = (crc >> 1) ^ ((((crc ^ buffer[i]) & 1) > 0) ? 0xEDB88320 : 0);
                }
            }
        }

        crc ^= uint.MaxValue;
        return $"CRC32 for {args[0]}: 0x{crc:X}";
    }

    static string Cmd_getmountpoint(string[] args)
    {
        string mountPoint = PhysicsFS.GetMountPoint(args[0]);
        return $"Dir [{args[0]}] is mounted at [{mountPoint}].";
    }

    static string Cmd_setroot(string[] args)
    {
        PhysicsFS.SetRoot(args[0], args[1]);
        return "Successful.";
    }

    public static string OutputArchivers()
    {
        StringBuilder output = new StringBuilder();
        output.AppendLine();
        output.Append("TestPhysFs version ");
        output.Append(PhysicsFS.LinkedVersion);
        output.AppendLine();
        output.AppendLine();
        output.AppendLine("Supported archive types:");

        PhysicsFS.Init();

        foreach (var archiveType in PhysicsFS.SupportedArchiveTypes())
        {
            output.Append(" * ");
            output.Append(archiveType.Extension);
            output.Append(":\t");
            output.AppendLine(archiveType.Description);
            output.Append("\tWritten by ");
            output.AppendLine(archiveType.Author);
            output.Append("\t");
            output.AppendLine(archiveType.Url);
            output.AppendLine(archiveType.SupportsSymlinks ?
                "\tSupports symbolic links." : "\tDoes not support symbolic links.");
        }

        output.AppendLine();
        return output.ToString();
    }

    public static string? ProcessCommand(string input)
    {
        string[] args = input.Split();
        string key = args[0];

        if (!_commands.TryGetValue(key, out Command command))
        {
            return "No such command. Type 'help' for additional info.";
        }

        string[] commandArgs = args[1..];
        if (commandArgs.Length < command.Args.Length)
        {
            string[] a = Array.ConvertAll(command.Args, j => $"<{j}>");
            string argsStr = string.Join(' ', a);
            return $"usage: \"{key} {argsStr}\"";
        }

        return command.Action(commandArgs);
    }
}