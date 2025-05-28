# PhysFS.NET
### Lightweight, high-level API + Native method bindings for [physfs](https://github.com/icculus/physfs)
While I was looking for a C# wrapper for Icculus' physfs (a portable, flexible file i/o abstraction), I noticed my options were limited, as some of the bindings were outdated or lacked much needed convenience. While I was writing my own wrapper for personal use, it turned out a lot bigger than I had expected, and included several quality-of-life features, so I decided to publicly release it in case someone in the community needs as much as I did.

## Why use this API over others (perhaps even yours)
* Lightweight, while including all of the functionality a regular user might require;
* Includes original author's comments on the implementation details, as well as my own thoughts and notes here and there;
* Automatically processes errors, if any are encountered, and also raises exceptions with descriptive messages;
* Provides console and file logs during ``DEBUG`` compile mode.

## How do I use this?
Just clone the repository or put it as your repository's submodule and add it as your C# project's dependency.

⚠️Warning! Important!⚠️<br/>
Don't forget to reference ``physfs`` project (in relation to PhysFS.NET) with the following properties so the DLLs will be automatically copied to your application's output directory:
```xml
<ProjectReference Include="\physfs\build\physfs.vcxproj">
	<ReferenceOutputAssembly>false</ReferenceOutputAssembly>
	<OutputItemType>Content</OutputItemType>
	<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</ProjectReference>
```

## Simple Example
```cs
// First of all, initialize the library!
PhysFS.Init();

// This will set basic values, for example, the writing directory
// will be set to "AppData/Roaming/blazingzephyr/PhysFS.NET".
PhysFS.SetSaneConfig("blazingzephyr", "PhysFS.NET", ".zip", false, true);

// This will add the application's directory as an available search path.
PhysFS.Mount(PhysFS.BasePath, false);

// This is probably expected to be in the application's directory.
// Or could be in the writing dir.
FileSystemObject file = PhysFS.OpenFile("file.txt", FileSystemObjectAccess.Read);

// Equivalent to PhysFS.ReadBytes(file) (calls it internally).
// Use whichever one you prefer.
byte[] fileContents = file.ReadBytes();

// Do whatever you need to with the contents.
Console.WriteLine(fileContents.Length);

// Alternatively, you can use file.ReadToStream() or PhysFs.ReadToStream(file).
// UnmanagedMemoryStream stream = file.ReadToStream();
// *process the stream...*
// stream.Close();

// Equivalent to PhysFS.CloseFile(file).
file.Dispose();

// This will open a file for writing.
// If the file exists, it will be truncated (emptied).
// if the file doesn't exist, it will be created in the write dir.
file = PhysFS.OpenFile("new.txt", FileSystemObjectAccess.Truncate);

// PhysFS uses UTF8 encoding!
byte[] content = Encoding.UTF8.GetBytes("Sample text");

// This will write bytes to a file.
// You can also use this with FileSystemObjectAccess.Append
// to instead append contents to a file.
file.WriteBytes(content);

// Close the file.
file.Dispose();

// You can set it to something meaningless if you don't need it.
int data = 0;

// This will list every file found in "/".
PhysFS.Enumerate("/", EnumerateCallback, ref data);

// Our callback for enumerating files.
static EnumerateCallbackResult EnumerateCallback(ref int data,
string directory, string file)
{
    // But you can also use it for writing data from within.
    data += 1;

    // You can do whatever you want with the provided data.
    // Open files, for example.
    Console.WriteLine($"{directory}{file}");

    if (data == 10)
    {
        // This will signal that we should stop on the 10th file found.
        return EnumerateCallbackResult.Stop;
    }

    // Signals that we should continue looking for files.
    return EnumerateCallbackResult.Continue;
}

// Don't forget to deinitialize PhysFS when you're done!
PhysFS.Deinit();
```
This program will generate the following output:
```
[01.05.2025 11:37:56::8540] [PhysFS] SUCC Initializing PhysFS.
[01.05.2025 11:37:56::8689] [PhysFS] SUCC Setting default config for blazingzephyr, PhysFS.NET with CD-ROMS support set to False, default archive extension set to .zip and prepend archives set to True.
[01.05.2025 11:37:56::8725] [PhysFS] SUCC Adding D:\WTRedux\WTRedux\bin\x64\Debug\net8.0\ to search paths to /.
[01.05.2025 11:37:56::8778] [PhysFS] SUCC Opening file.txt for Read.
[01.05.2025 11:37:56::8804] [PhysFS] SUCC Getting the file statistics of file.txt.
[01.05.2025 11:37:56::8835] [PhysFS] SUCC Reading 4178 bytes from file.txt.
4178
[01.05.2025 11:37:56::8869] [PhysFS] SUCC Closing file.txt.
[01.05.2025 11:37:56::8878] [PhysFS] SUCC Opening new.txt for Truncate.
[01.05.2025 11:37:56::8881] [PhysFS] SUCC Getting the file statistics of new.txt.
[01.05.2025 11:37:56::8889] [PhysFS] SUCC Writing 11 bytes to new.txt.
[01.05.2025 11:37:56::8918] [PhysFS] SUCC Closing new.txt.
/new.txt
/newWrite.png
/1.jpg
/1.zip
/983445408413.mp4
/FAudio.dll
/file.txt
/FNA.dll
/FNA.dll.config
/FNA.pdb
[01.05.2025 11:37:56::9000] [PhysFS] SUCC Enumerating files in /.
[01.05.2025 11:37:56::9007] [PhysFS] SUCC Deinitializing PhysFS.
```

## When to use low-level bindings.
``Icculus.PhysFS.NET.PhysFS`` class includes all of the essential functionality of physfs, so diving deeper into the internals is not required from the user at all. However, if you need to call the native methods directly or if you want to write your own wrapper, you can use the fucntionality provided by the ``Internals`` namespace.

## What's left to cover?
Outdated and deprecated functionality prior to PhysFS 2.1 was deliberately left uncovered. However, there is still some functionality left untouched due to uncertainties or time constraints. Most of it is either way too in-depth and would be problematic to recreate in C# (such as custom allocators or archivers) or simply not needed (reading and writing single bytes of data), or C# provides the similar functionality already (such as encoding conversion and other string operations). You can find out more about what could be added later in the [issues](https://github.com/blazingzephyr/PhysFS.NET/issues) tab.

## How can I contribute?
If you encountered an issue while using the bindings, open an issue, or open a pull request if you want to merge some new functionality you added. You can also DM me on Discord (username: blazingzephyr). I would gladly discuss your contributions to the project!

![line](https://capsule-render.vercel.app/api?type=rect&color=gradient&height=2)

[![Avatar](https://images.weserv.nl/?url=https://avatars.githubusercontent.com/u/119159668?v=4&h=96&w=96&fit=cover&mask=circle&maxage=7d)](https://github.com/blazingzephyr)<br>
Provided by [@blazingzephyr](https://github.com/blazingzephyr)<br>
Last updated: Jan 06, 2025.
[![GitHub](https://images.weserv.nl/?url=https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png&h=15&w=15&fit=none&mask=circle&maxage=7d)](https://github.com/blazingzephyr/PhysFS.NET/commits/main)
