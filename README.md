# PhysFS.NET
### Extensive, high-level API wrapper for [physfs](https://github.com/icculus/physfs).
A simple-to-use C# wrapper for Icculus' physfs (a portable, flexible file i/o abstraction).
Currently supports builds for Windows and Android. Adding support for other platforms is considered in the future.

## Advantages of using this wrapper over your own
* Lightweight, you do not need to keep an extensive system of dependencies or rely on other packages for this to work;
* Extensive, covers the entire API (even the hardcore stuff you most likely won't need ever);
* Includes the entirety of the author's comments with additions and fixes where needed;
* Automatically handles any underlying errors and raises exceptions;
* Android support included;
* Tested with NativeAOT for Windows builds.

## How do I use this?
Just clone the repository or put it as your repository's submodule and add the Windows/Android project as a dependency.

⚠️Warning! Important!⚠️<br/>
When publishing with NativeAOT, you will have to copy the compiled native library yourself, otherwise the application crashes!

## Simple Example
```cs
// First of all, initialize the library!
PhysicsFS.Init();

// This will set basic values, for example, the writing directory
// will be set to "AppData/Roaming/blazingzephyr/PhysFS.NET".
PhysicsFS.SetSaneConfig("blazingzephyr", "PhysFS.NET", "ZIP", false, true);

// This will add the application's directory as an available search path.
PhysicsFS.Mount(PhysicsFS.BaseDir!, "/", true);

// This is probably expected to be in the application's directory.
// Or could be in the writing dir.
// PhysFsFileHandle implements disposable pattern, so you don't need to call Close.
using (PhysFsFileHandle file = PhysicsFS.OpenRead("file.txt"))
{
    // Wraps over PhysicsFS.ReadBytes for convenience.
    byte[] buffer = PhysFsUtil.ReadAllBytes(file);

    // PhysFS uses UTF8 encoding!
    string fileContents = Encoding.UTF8.GetString(buffer);

    // Do whatever you need to with the contents.
    Console.WriteLine(fileContents);

    // Alternatively, you can use PhysFsUtil.ReadToBuffer.
    // using (SafeHGlobalBuffer buffer = PhysFsUtil.ReadToBuffer(file))
    // using (var stream = new UnmanagedMemoryStream(buffer, 0, (long)buffer.ByteLength))
    // using (StreamReader reader = new StreamReader(stream, leaveOpen: false))
    // {
    //  string fileContents = reader.ReadToEnd();
    //  Console.WriteLine(fileContents);
    //  testdeps.Dispose();
    // }
}

// This will open a file for writing.
// If the file exists, it will be truncated (emptied).
// if the file doesn't exist, it will be created in the write dir.
using (PhysFsFileHandle file = PhysicsFS.OpenWrite("new.txt"))
{
    // PhysFS uses UTF8 encoding!
    byte[] content = Encoding.UTF8.GetBytes("Sample text");

    // This will write bytes to a file.
    // You can also use this with FileSystemObjectAccess.Append
    // to instead append contents to a file.
    // 
    // Wraps over PhysicsFS.WriteBytes for convenience.
    PhysFsUtil.WriteAllBytes(file, content);
}

// You can set it to something meaningless if you don't need it.
int data = 0;

// This will list every file found in "/".
//
// There is an overload which does not take any callback data in case you don't need it!
PhysicsFS.Enumerate("/", EnumerateCallback, ref data);

// Our callback for enumerating files.
static PhysFsEnumerateCallbackResult EnumerateCallback(ref int data,
string directory, string file)
{
    // But you can also use it for writing data from within.
    data += 1;

    // You can do whatever you want with the provided data.
    // Open files, for example.
    Console.WriteLine($"{directory}{file}");

    if (data == 6)
    {
        // This will signal that we should stop on the 10th file found.
        return PhysFsEnumerateCallbackResult.Stop;
    }

    // Signals that we should continue looking for files.
    return PhysFsEnumerateCallbackResult.OK;
}

// Don't forget to deinitialize PhysFS when you're done!
PhysicsFS.Deinit();
```
This program generates the following output:
```
"Content inside file.txt"
/new.txt
/file.txt
/physfs.dll
/PhysFS.NET.dll
/PhysFS.NET.pdb
/TestPhysFsWin.deps.json
```

## Detailed Example
The bindings also include a test application in the [/test](https://github.com/blazingzephyr/PhysFS.NET/tree/main/test) subfolder for both Windows and Android.
Build it if you want more insights on how to use the library.
There is also [an unfinished custom Archiver example](https://github.com/blazingzephyr/PhysFS.NET/tree/main/test/src/ArchiverExample.cs) if you want to mess with it.

## ``old`` branch
There is [an old version of the wrapper](https://github.com/blazingzephyr/PhysFS.NET/tree/old) available for those who need it.
It includes an entirely different API and does not contain some functionality you might need. Outdated and deprecated functionality prior to PhysFS 2.1 was deliberately left uncovered there. Some functionality was left untouched due to uncertainties or time constraints.
Most of it was either way too in-depth and was problematic to recreate in C# (such as custom allocators or archivers) or simply not needed (reading and writing single bytes of data), or C# provided similar functionality already (such as encoding conversion and string comparison).
However, all of the functionality previously not included is available now and I even added most of the utilities from the old API in the current version.

### When to use low-level bindings
The API provides any functionality you might need, but if you need to call the native methods directly or if you want to write your own wrapper, you can include the [``old`` branch](https://github.com/blazingzephyr/PhysFS.NET/tree/old) and use the functionality provided by the ``Internals`` namespace. That branch will not be updated.

## How can I contribute?
If you encountered a problem while using the bindings, open an issue, or open a pull request if you want to merge some new functionality. You can also DM me on Discord (username: blazingzephyr). I would gladly discuss your contributions to the project!

![line](https://capsule-render.vercel.app/api?type=rect&color=gradient&height=2)

[![Avatar](https://images.weserv.nl/?url=https://avatars.githubusercontent.com/u/119159668?v=4&h=96&w=96&fit=cover&mask=circle&maxage=7d)](https://github.com/blazingzephyr)<br>
Provided by [@blazingzephyr](https://github.com/blazingzephyr)<br>
Last updated: Jun 30, 2025.
[![GitHub](https://images.weserv.nl/?url=https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png&h=15&w=15&fit=none&mask=circle&maxage=7d)](https://github.com/blazingzephyr/PhysFS.NET/commits/main)
