// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text;

namespace System.Runtime.InteropServices.Marshalling;

/// <summary>
/// Marshaller for UTF-8 strings.
/// </summary>
[CustomMarshaller(typeof(bool), MarshalMode.Default, typeof(VoidMarshaller))]
// [CustomMarshaller(typeof(void), MarshalMode.UnmanagedToManagedIn, typeof(UnmanagedToManagedIn))]
public static unsafe class VoidMarshaller
{
    public static nint ConvertToUnmanaged(bool managed)
    {
        throw new NotImplementedException();
    }

    public static bool ConvertToManaged(nint unmanaged)
    {
        throw new NotImplementedException();
    }
}
