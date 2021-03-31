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
                    .SetMinimumLevel(LogLevel.Debug))
                .AddFileSystem(fileSystem);

            await host.InvokeGeneratorAsync<HelloWorldGenerator>();
        }
    }
}