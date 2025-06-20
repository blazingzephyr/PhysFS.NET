
/*
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Icculus.PhysFS.NET.PhysFsIo;

namespace Icculus.PhysFS.NET;

/// <summary>
/// An example of a custom <see cref="PhysFsArchiver"/>.
/// This was never finished, but you're free to try and write your own archiver.
/// This basically just rewrites the original QPAK archiver.
/// </summary>
internal static class ArchiverExample
{
    const int QPACK_SIG = 0x4B434150;

    public struct __PHYSFS_DirTreeEntry
    {
        /// <summary>
        /// Full path in archive
        /// </summary>
        public string name;

        /// <summary>
        /// next item in hash bucket.
        /// __PHYSFS_DirTreeEntry*
        /// </summary>
        public IntPtr hashnext;

        /// <summary>
        /// linked list of kids, if dir.
        /// __PHYSFS_DirTreeEntry*
        /// </summary>
        public IntPtr children;

        /// <summary>
        /// next item in same dir.
        /// __PHYSFS_DirTreeEntry*
        /// </summary>
        public IntPtr sibling;

        public bool isdir;
    }

    public struct __PHYSFS_DirTree
    {
        /// <summary>
        /// root of directory tree.
        /// </summary>
        public IntPtr root;

        /// <summary>
        /// all entries hashed for fast lookup.
        /// __PHYSFS_DirTreeEntry**.
        /// </summary>
        public IntPtr hash;

        /// <summary>
        /// number of buckets in hash.
        /// </summary>
        public nint hashBuckets;

        /// <summary>
        /// size in bytes of entries (including subclass).
        /// </summary>
        public nint entrylen;

        /// <summary>
        /// non-zero to treat entries as case-sensitive in DirTreeFind
        /// </summary>
        public bool case_sensitive;

        /// <summary>
        /// non-zero to treat paths as US ASCII only (one byte per char, only 'A' through 'Z' are considered for case folding).
        /// </summary>
        public bool only_usascii;
    }

    public struct UNPKinfo
    {
        public __PHYSFS_DirTree tree;
        public IntPtr io;
    }

    public struct UNPKentry
    {
        public __PHYSFS_DirTreeEntry tree;
        public ulong startPos;
        public ulong size;
        public long ctime;
        public long mtime;
    }

    public struct UNPKfileinfo
    {
        public IoHandle io;
        public IntPtr entry; //UNPKentry*
        public uint curPos;
    }

    public static IntPtr OpenArchive(ref IoHandle native_io, string name, bool forWriting, out bool claimed)
    {
        uint val = 0;
        uint pos = 0;
        uint count = 0;
        IntPtr unpkarc = IntPtr.Zero;

        if (Unsafe.IsNullRef(ref native_io))
        {
            claimed = false;
            return IntPtr.Zero;
        }

        if (forWriting)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_READ_ONLY);
            claimed = false;
            return IntPtr.Zero;
        }

        PhysFsIo io = new PhysFsIo(ref native_io);
        if (io.Read(ref native_io, RefToIntPtr(ref val), 4) != 4)
        {
            claimed = false;
            return IntPtr.Zero;
        }

        if (PhysicsFS.SwapULE32(val) != QPACK_SIG)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_UNSUPPORTED);
            claimed = false;
            return IntPtr.Zero;
        }

        claimed = true;

        if (io.Read(ref native_io, RefToIntPtr(ref val), 4) != 4) return IntPtr.Zero;
        pos = PhysicsFS.SwapULE32(val);

        if (io.Read(ref native_io, RefToIntPtr(ref val), 4) != 4) return IntPtr.Zero;
        count = PhysicsFS.SwapULE32(val);

        if ((count % 64) != 0)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_CORRUPT);
            return IntPtr.Zero;
        }
        count /= 64;

        if (!io.Seek(ref native_io, pos)) return IntPtr.Zero;

        // !!! FIXME: check case_sensitive and only_usascii params for this archive.
        unpkarc = UNPK_openArchive(ref native_io, true, false);
        if (unpkarc == IntPtr.Zero) return IntPtr.Zero;

        if (!qpakLoadEntries(io, count, unpkarc))
        {
            UNPK_abandonArchive(unpkarc);
            return IntPtr.Zero;
        }

        return IntPtr.Zero;
    }

    public static bool qpakLoadEntries(PhysFsIo io, uint count, IntPtr arc)
    {
        for (uint i = 0; i < count; i++)
        {
            uint size = 0, pos = 0;
            byte[] name = new byte[56];

            unsafe
            {
                fixed (byte* nameN = name)
                {
                    if (io.Read(ref io.Handle, new IntPtr(nameN), sizeof(byte) * 56) != sizeof(byte) * 56) return false;
                }

                if (io.Read(ref io.Handle, new nint(&pos), sizeof(uint)) != sizeof(uint)) return false;
                if (io.Read(ref io.Handle, new nint(&size), sizeof(uint)) != sizeof(uint)) return false;
            }

            size = PhysicsFS.SwapULE32(size);
            pos = PhysicsFS.SwapULE32(pos);
            if (UNPK_addEntry(arc, name, false, -1, -1, pos, size) == IntPtr.Zero) return false;
        }

        return true;
    }

    public static IntPtr UNPK_addEntry(IntPtr opaque, byte[] name, bool isDir, long ctime,
        long mtime, ulong pos, ulong len)
    {
        char[] c = Array.ConvertAll(name, i => (char)i);
        string s = new string(c).TrimEnd('\0');

        throw new NotImplementedException(
            $"This is supposed to serve as an example.\n" +
            $"opaque: {opaque}\n" +
            $"name: {new string(s)}\n" +
            $"isDir: {isDir}\n" +
            $"ctime: {ctime}\n" +
            $"mtime: {mtime}\n" +
            $"pos: {pos}\n" +
            $"len: {len}\n");
    }

    public static IntPtr UNPK_openArchive(ref IoHandle io, bool case_sensitive, bool only_useascii)
    {
        var allocator = new PhysFsAllocator(ref PhysicsFS.GetAllocator().Value);
        IntPtr infoHandle = allocator.Malloc((ulong)Marshal.SizeOf<UNPKinfo>());

        if (infoHandle == IntPtr.Zero)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_OUT_OF_MEMORY);
            return IntPtr.Zero;
        }

        ref UNPKinfo info = ref IntPtrToRef<UNPKinfo>(infoHandle);

        if (!__PHYSFS_DirTreeInit(ref info.tree, Marshal.SizeOf<UNPKentry>(), case_sensitive, only_useascii))
        {
            allocator.Free(infoHandle);
            return IntPtr.Zero;
        }

        info.io = RefToIntPtr(ref io);
        return infoHandle;
    }

    public static bool __PHYSFS_DirTreeInit(ref __PHYSFS_DirTree dt, nint entrylen, bool case_sensitive, bool only_usascii)
    {
        nint alloclen;

        if (entrylen < Marshal.SizeOf<__PHYSFS_DirTreeEntry>())
        {
            throw new Exception();
        }

        static ref byte GetStart<T>(ref T t)
        {
            unsafe
            {
                void* b = Unsafe.AsPointer(ref t);
                return ref Unsafe.AsRef<byte>(b);
            }
        }

        Unsafe.InitBlock(ref GetStart(ref dt), (byte)'\0', (uint)Marshal.SizeOf<IntPtr>());
        dt.case_sensitive = case_sensitive;
        dt.only_usascii = only_usascii;

        PhysFsAllocator allocator = new PhysFsAllocator(ref PhysicsFS.GetAllocator().Value);
        dt.root = allocator.Malloc((ulong)entrylen);

        if (dt.root == IntPtr.Zero)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_OUT_OF_MEMORY);
            return false;
        }

        Unsafe.InitBlock(ref IntPtrToRef<byte>(dt.root), (byte)'\0', (uint)entrylen);
        ref var entry = ref IntPtrToRef<__PHYSFS_DirTreeEntry>(dt.root);
        entry.name = "/\0";
        entry.isdir = true;
        dt.hashBuckets = 64;
        if (dt.hashBuckets == 0) dt.hashBuckets = 1;
        dt.entrylen = entrylen;

        alloclen = dt.hashBuckets * Marshal.SizeOf<IntPtr>();
        dt.hash = allocator.Malloc((ulong)alloclen);

        if (dt.hash == IntPtr.Zero)
        {
            PhysicsFS.SetErrorCode(PhysFsErrorCode.PHYSFS_ERR_OUT_OF_MEMORY);
            return false;
        }

        Unsafe.InitBlock(ref IntPtrToRef<byte>(dt.hash), (byte)'\0', (uint)alloclen);

        return true;
    }

    public static void UNPK_abandonArchive(IntPtr opaque)
    {
        ref UNPKinfo info = ref IntPtrToRef<UNPKinfo>(opaque);
        if (!Unsafe.IsNullRef(ref info))
        {
            info.io = default;
            UNPK_closeArchive(opaque);
        }
    }

    public static void UNPK_closeArchive(IntPtr opaque)
    {
        ref UNPKinfo info = ref IntPtrToRef<UNPKinfo>(opaque);
        if (!Unsafe.IsNullRef(ref info))
        {
            // TBA

            if (!Unsafe.IsNullRef(ref info.io))
            {
                ref IoHandle handle = ref IntPtrToRef<IoHandle>(info.io);
                new PhysFsIo(ref handle).Destroy(ref handle);
            }

            new PhysFsAllocator(ref PhysicsFS.GetAllocator().Value).Free(opaque);
        }
    }

    public static IntPtr RefToIntPtr<T>(ref T value)
    {
        unsafe
        {
            void* ptr = Unsafe.AsPointer(ref value);
            return new IntPtr(ptr);
        }
    }

    public static ref T IntPtrToRef<T>(IntPtr intPtr)
    {
        unsafe
        {
            void* ptr = intPtr.ToPointer();
            return ref Unsafe.AsRef<T>(ptr);
        }
    }
}
*/