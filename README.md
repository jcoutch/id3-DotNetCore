# ID3.NET for .NET Core [![Build status](https://ci.appveyor.com/api/projects/status/4h6xnd2cvjpf93oe?svg=true)](https://ci.appveyor.com/project/jcoutch/id3-dotnetcore)


ID3-DotNetCore is an effort to port the [ID3.NET library](https://id3.codeplex.com/) to .NET Core.

ID3.NET is a set of libraries for reading, modifying and writing ID3 and Lyrics3 tags in MP3 audio files.

This version of the core library currently supports .NET Core 1.1 and 2.0, as well as .NET Standard 1.3 and 2.0.

**NOTE** - The code in this repo has not been thoroughly tested.  I essentially took the original source code, created new project files, fixed some dependencies, and packaged it up.  I did port the unit tests, but none of the test MP3 files were in the original developer's repo...so I can't run them at the moment.

## Extension Support

The original version of ID3.NET supported multiple extensions, but as of right now, only the ID3.File extension has been ported.

Since the original library was written back in 2002, I assumed the metadata library was heavily out of date, and would need some serious refactoring.

Also, .NET Core's binary serialization support is significantly different from the full framework, so I didn't port the Serialization extension.  When/If I have the time, I'll look at porting the code to use [MessagePack](https://github.com/neuecc/MessagePack-CSharp), but if anyone feels ambitious, I'll accept a pull request.  :-)

## Getting Started

Install the Nuget package via Package Management Console:
```powershell
Install-Package ID3.Net-DotNetCore
```

If you want to use the ID3.Files package:
```powershell
Install-Package ID3.Files-DotNetCore
```

## Example
```csharp
var musicFiles = Directory.GetFiles(@"C:\Music", "*.mp3");
foreach (string musicFile in musicFiles)
{
    using (var mp3 = new Mp3File(musicFile))
    {
        Id3Tag tag = mp3.GetTag(Id3TagFamily.FileStartTag);
        Console.WriteLine("Title: {0}", tag.Title.Value);
        Console.WriteLine("Artist: {0}", tag.Artists.Value);
        Console.WriteLine("Album: {0}", tag.Album.Value);
    }
}
```
