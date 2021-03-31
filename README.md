# Genzor - generating files with Blazor components
Genzor is an experimental library ideally suited for generating files spanning multiple folders, using Blazor component model to generating the output.

That means all the Blazor goodness such as cascading values, service injection, etc., should work, making it easy to split up complex (code) generation into different Blazor components that each produce a small bit of the total generated output.

## How it works
The basic version of Genzor allows you to create components that produce files and directories into a file system abstraction that you provide to Genzor.

The files and directories are represented by two sets of types in Genzor, one in your generator component's render tree and one your file system:

|          Type | Generator component's<br>render tree |    File system    |
|--------------:|:------------------------------------:|:-----------------:|
| **Directory** |         `IDirectoryComponent`        |    `IDirectory`   |
|      **File** |           `IFileComponent`           | `IFile<TContent>` |
|               |                                      |                   |

**NOTE:** Genzor comes with an implemetation of the `IDirectoryComponent` and `IFileComponent` types to make it easier to get started. These are simply named `Directory` and `TextFile`. But you are free to create your own as needed.

Genzor will add files and directories to the file system abstraction you provide like this:

1. Invoke (render) the generator component, pass in any regular `[Parameter]` parameters to it or `[Inject]` services into it.
2. Walk through the entire render tree, and create `IDirectory` and `IFile<string>` whenever a `IDirectoryComponent` or `IFileComponent` is encountered.  
   - Top level `IDirectoryComponent` or `IFileComponent` components, i.e. ones not nested inside a parent `IDirectoryComponent` component, are added directly to the file system through its `AddItem()` method.
   - `IDirectoryComponent` or `IFileComponent` components nested inside a parent `IDirectoryComponent` component is instead added to the related `IDirectory` through it's `Add()` method.
3. A `IDirectoryComponent` component can contain other content and components, but only `IDirectoryComponent` or `IFileComponent` child components will be added passed to the `IDirectory` it maps to.
4. A `IFileComponent` component can contain other content and components, whose rendered output will be added to as the content of the `IFile<string>` that it maps to. The only exception is that a `IFileComponent` component cannot contain a `IDirectoryComponent` component inside it.

For example, consider the simple `HelloWorldGenerator.razor` generator component:

```razor
<Directory Name="HelloWorld">
    <TextFile Name="hello-text.txt">HELLO TEXT</TextFile>
    <Directory Name="NestedHello">
        <TextFile Name="nested-hello-text.txt">NESTED HELLO TEXT</TextFile>
    </Directory>
</Directory>
```

That will generate the following (assuming standard file system behaviour):

```text
> ls -r .\HelloWorld\
    Directory: \GenzorDemo\HelloWorld

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          30-03-2021    23:36                NestedHello
-a---          30-03-2021    23:36             10 hello-text.txt

    Directory: \GenzorDemo\HelloWorld\NestedHello

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---          30-03-2021    23:36             17 nested-hello-text.txt
```

## Example

The following creates a console application that uses Genzor to run the `HelloWorldGenerator.razor` generator component.

**NOTE:** You can also download the sample from https://github.com/egil/genzor/tree/main/samples/GenzorDemo if you prefer.

**The steps are as follows:**

1. Create new console app, e.g. using `dotnet new console -o GenzorDemo`.
2. Change the project SDK type to `Microsoft.NET.Sdk.Razor`.
3. Add the [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) package to the project, e.g. using `dotnet add package Microsoft.Extensions.DependencyInjection`.
4. Optionally, to see log output from Genzor, add the [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) package to the project as well, e.g. using `dotnet add package Microsoft.Extensions.Logging.Console`. 
5. Add the [Genzor](https://github.com/egil/genzor/packages/700971) package to the project. It is hosted currently here on GitHubs Package Repository, but will show up on NuGet.org if this turns out to be useful to folks. See the guide below for how to connect to it. 
6. Update the Program.cs file to look as follows:  
   
    ```c#
    using System.IO;
    using System.Threading.Tasks;
    using Genzor;
    using Microsoft.Extensions.Logging;
    using GenzorDemo.Generators;

    namespace GenzorDemo
    {
        class Program
        {
            static async Task Main(string[] args)
            {
                var fileSystem = new FileSystem(new DirectoryInfo(Directory.GetCurrentDirectory()));

                using var host = new GenzorHost()
                    .AddLogging(configure => configure
						      .AddConsole()
								.SetMinimumLevel(LogLevel.Debug)) // if the optional logging package has beed added
                    .AddFileSystem(fileSystem);

                await host.InvokeGeneratorAsync<HelloWorldGenerator>();
            }
        }
    }
    ```
8. Create a new directory in the project named `Generators` (not strictly a requirement from Genzor, but it groups the generators together here).
9. Add the following `HelloWorldGenerator.razor` file inside a `Generators` folder:   
       
    ```razor
    @using Genzor.Components

    <Directory Name="HelloWorld">
        <TextFile Name="hello-text.txt">HELLO TEXT</TextFile>
        <Directory Name="NestedHello">
            <TextFile Name="nested-hello-text.txt">NESTED HELLO TEXT</TextFile>
        </Directory>
    </Directory>
    ```
10. Add the following `FileSystem.cs` file to the project (this is our basic implementation of Genzor's `IFileSystem`):   
    
    ```c#
    using System;
    using System.IO;
    using Genzor.FileSystem;

    namespace GenzorDemo
    {
        class FileSystem : IFileSystem
        {
            private readonly DirectoryInfo rootDirectory;

            public FileSystem(DirectoryInfo rootDirectory) 
                => this.rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));

            public void AddItem(IFileSystemItem item)
                => AddItem(rootDirectory, item);

            private void AddItem(DirectoryInfo parent, IFileSystemItem item)
            {
                switch (item)
                {
                    case IDirectory directory:
                        AddDirectory(parent, directory);
                        break;
                    case IFile<string> textFile:
                        AddTextFile(parent, textFile);
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported file system item {item.GetType().FullName}");
                }
            }

            private void AddDirectory(DirectoryInfo parent, IDirectory directory)
            {
                var createdDirectory = parent.CreateSubdirectory(directory.Name);

                foreach (var item in directory)
                    AddItem(createdDirectory, item);
            }

            private void AddTextFile(DirectoryInfo parent, IFile<string> file)
            {
                var fullPath = Path.Combine(parent.FullName, file.Name);
                File.WriteAllText(fullPath, file.Content);
            }
        }
    }
    ```
11. Run the application, e.g. by typing `dotnet run` in a terminal. After the app runs, you should see some files created in whatever is your "current directory" when running the app.

## Getting the Genzor package from GitHub Package Repository

To be able to download packages from GitHub Package Repository, do the following:

1. Go into your GitHub settings under [security tokens](https://github.com/settings/tokens) and generate a new access token. The token should only have `read:packages` rights.
2. Create a **`nuget.config`** file and place it in your project folder.
3. Add the following content to the `nuget.config`, replacing USERNAME with your GitHub username and TOKEN with the generated token from step 1:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <!--To inherit the global NuGet package sources remove the <clear/> line below -->
        <clear />		
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="github" value="https://nuget.pkg.github.com/egil/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <github>
            <add key="Username" value="USERNAME" />
            <add key="ClearTextPassword" value="TOKEN" />
        </github>
    </packageSourceCredentials>
</configuration>
```

Then you should be able to do a `dotnet add package` or `dotnet restore` and pull packages from my GPR feed.
