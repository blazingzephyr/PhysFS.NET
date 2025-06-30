using System;
using System.Collections.Generic;
using System.Text;

namespace Icculus.PhysFS.NET;

public struct PHYSFS_Version
{
    public byte major; /**< major revision */
    public byte minor; /**< minor revision */
    public byte patch; /**< patchlevel */
}