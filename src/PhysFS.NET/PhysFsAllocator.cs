namespace Icculus.PhysFS.NET;

/// <summary>
/// Initializes the current PhysicsFS allocator.
/// </summary>
/// <returns>non-zero on success, zero on failure</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int PhysFsAllocatorInit();

/// <summary>
/// Deinitializes the current PhysicsFS allocator.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PhysFsAllocatorDeinit();

/// <summary>
/// Allocates memory using the specified number of bytes and returns the pointer.
/// </summary>
/// <param name="size">The required number of bytes in memory.</param>
/// <returns>A pointer to the newly allocated memory.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate IntPtr PhysFsAllocatorMalloc(ulong size);

/// <summary>
/// Resizes a block of memory previously allocated by <see cref="PhysFsAllocator.Malloc"/>.
/// </summary>
/// <param name="handle">the handle returned by <see cref="PhysFsAllocator.Malloc"/>.</param>
/// <param name="size">The required number of bytes in memory.</param>
/// <returns>A pointer to the reallocated memory.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate IntPtr PhysFsAllocatorRealloc(IntPtr handle, ulong size);

/// <summary>
/// Frees memory previously allocated by PhysicsFS.
/// </summary>
/// <param name="handle">the handle returned by <see cref="PhysFsAllocator.Malloc"/>.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PhysFsAllocatorFree(IntPtr handle);

/// <summary>
/// PhysicsFS allocation function pointers.
/// </summary>
/// <remarks>
/// (This is for limited, hardcore use. If you don't immediately see a need
/// for it, you can probably ignore this forever.)<br/><br/>
/// You create one of these structures for use with 
/// <see cref="PhysicsFS.SetAllocator"/>.
/// Allocators are assumed to be reentrant by the caller; please mutex
/// accordingly.<br/><br/>
/// Allocations are always discussed in 64-bits, for future expansion...we're
/// on the cusp of a 64-bit transition, and we'll probably be allocating 6
/// gigabytes like it's nothing sooner or later, and I don't want to change
/// this again at that point. If you're on a 32-bit platform and have to
/// downcast, it's okay to return NULL if the allocation is greater than
/// 4 gigabytes, since you'd have to do so anyhow.<br/><br/>
/// See also:<br/>
/// <seealso cref="PhysicsFS.SetAllocator"/>
/// </remarks>
public readonly ref struct PhysFsAllocator
{
    /// <summary>
    /// A PhysicsFS allocator handle.
    /// Incapsulates the function pointers, which are then passed to PhysicsFS.
    /// </summary>
    /// <remarks>
    /// As you can see from the lack of meaningful fields, you should treat this
    /// as opaque data. Don't try to manipulate the allocator handle, just pass the
    /// pointer you got, unmolested, to various PhysicsFS APIs.
    /// </remarks>
    public struct AllocatorHandle
    {
        internal IntPtr Init;
        internal IntPtr Deinit;
        internal IntPtr Malloc;
        internal IntPtr Realloc;
        internal IntPtr Free;
    }

    /// <summary>
    /// Native handle this allocator points to.
    /// </summary>
    public readonly ref AllocatorHandle Handle => ref _handle;

    private readonly ref AllocatorHandle _handle;

    /// <summary>
    /// Initialize. Can be Can be <see langword="null"/>. Zero on failure.
    /// </summary>
    public readonly PhysFsAllocatorInit? Init
    {
        get
        {
            if (_handle.Init == IntPtr.Zero) return null;
            return Marshal.GetDelegateForFunctionPointer<PhysFsAllocatorInit>(_handle.Init);
        }
        set
        {
            nint ptr = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            _handle.Init = ptr;
        }
    }

    /// <summary>
    /// Deinitialize your allocator. Can be <see langword="null"/>.
    /// </summary>
    public readonly PhysFsAllocatorDeinit? Deinit
    {
        get
        {
            if (_handle.Init == IntPtr.Zero) return null;
            return Marshal.GetDelegateForFunctionPointer<PhysFsAllocatorDeinit>(_handle.Init);
        }
        set
        {
            nint ptr = value is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            _handle.Init = ptr;
        }
    }

    /// <summary>
    /// Allocate like malloc().
    /// </summary>
    public readonly PhysFsAllocatorMalloc Malloc
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsAllocatorMalloc>(_handle.Malloc);
        set => _handle.Malloc = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Reallocate like realloc().
    /// </summary>
    public readonly PhysFsAllocatorRealloc Realloc
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsAllocatorRealloc>(_handle.Realloc);
        set => _handle.Realloc = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Free memory from <see cref="Malloc"/> or <see cref="Realloc"/>.
    /// </summary>
    public readonly PhysFsAllocatorFree Free
    {
        get => Marshal.GetDelegateForFunctionPointer<PhysFsAllocatorFree>(_handle.Free);
        set => _handle.Free = Marshal.GetFunctionPointerForDelegate(value);
    }

    /// <summary>
    /// Creates a friendly wrapper for a PhysicsFS allocator.
    /// </summary>
    /// <param name="allocator">
    /// Holds native function pointers. Treat this as opaque data.
    /// </param>
    public PhysFsAllocator(ref AllocatorHandle allocator)
    {
        _handle = ref allocator;
    }
}
