using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Genzor.Assertions
{
	public static class LoggingBuilderExtensions
	{
		[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Serilog should dispose of its logger itself")]
		public static ILoggingBuilder AddXunitLogger(this ILoggingBuilder loggingBuilder, ITestOutputHelper outputHelper)
		{
			var serilogLogger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(outputHelper, LogEventLevel.Verbose)
				.CreateLogger();
			loggingBuilder.Services.AddSingleton<ILoggerFactory>(new LoggerFactory().AddSerilog(serilogLogger, dispose: true));
			loggingBuilder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
			return loggingBuilder;
		}
	}
}
