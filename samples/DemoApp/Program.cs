using System;

namespace GenzorSourceGeneratorsDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
			HelloWorldGenerated.HelloWorldRazor.SayHello();
		}
    }
}
