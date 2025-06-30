using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Icculus.PhysFS.NET.SourceGeneration;

namespace Icculus.PhysFS.NET;

// [PhysFsCallConvAttribute([typeof(CallConvStdcall)])]
public static partial class PhysFS
{
    //[PhysFsFunctionDefinition("PHYSFS_getLinkedVersion", [typeof(PHYSFS_Version*)], typeof(void))]
    //public static partial Version GetLinkedVersion();

    //[PhysFsFunctionDefinition("PHYSFS_init", [typeof(byte*)], typeof(int))]
    //[ThrowsPhysFsException]
    //public static partial void Init([PhysFsMarshalUsing(typeof(Utf8StringMarshaller))] string? argv0);

    //[PhysFsFunctionDefinition("PHYSFS_deinit", [], typeof(int))]
    //[ThrowsPhysFsException]
    //public static partial void Deinit();

    [LibraryImport("physfs.dll", EntryPoint = "PHYSFS_init")]
    [return: MarshalUsing(typeof(IntegerAsVoidMarshaller))]
    public static partial void Init();
}
