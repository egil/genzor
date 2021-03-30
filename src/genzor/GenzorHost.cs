using System;
using System.Threading.Tasks;
using Genzor.Components;
using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genzor
{
	/// <summary>
	/// Represents a host for a <see cref="GenzorRenderer"/> that
	/// handles setting up a default service provider.
	/// </summary>
	public sealed class GenzorHost : IDisposable
	{
		private readonly IServiceCollection collection;
		private ServiceProvider? serviceProvider;
		private GenzorRenderer? renderer;

		/// <summary>
		/// Gets the <see cref="GenzorRenderer"/>.
		/// </summary>
		public GenzorRenderer Renderer
		{
			get
			{
				if (renderer is null)
				{
					serviceProvider = collection.BuildServiceProvider();
					renderer = serviceProvider.GetRequiredService<GenzorRenderer>();
				}

				return renderer;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenzorHost"/> class.
		/// </summary>
		public GenzorHost()
		{
			collection = new ServiceCollection();
			collection.AddSingleton<GenzorRenderer>();
		}

		/// <summary>
		/// Adds an <see cref="IFileSystem"/> implementation to the <see cref="GenzorHost"/> for use when generating.
		/// </summary>
		/// <param name="instance">The file system instance to use.</param>
		/// <typeparam name="TImplementation">The type of <see cref="IFileSystem"/> to use.</typeparam>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddFileSystem<TImplementation>(TImplementation instance)
			where TImplementation : class, IFileSystem
		{
			collection.AddSingleton(instance);
			collection.AddSingleton<IFileSystem>(instance);
			return this;
		}

		/// <summary>
		/// Adds an <see cref="IFileSystem"/> implementation to the <see cref="GenzorHost"/> for use when generating.
		/// </summary>
		/// <typeparam name="TImplementation">The type of <see cref="IFileSystem"/> to use.</typeparam>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddFileSystem<TImplementation>()
			where TImplementation : class, IFileSystem
		{
			collection.AddSingleton<TImplementation>();
			collection.AddSingleton<IFileSystem>(s => s.GetRequiredService<TImplementation>());
			return this;
		}

		/// <summary>
		/// Adds logging services to the specified <see cref="IServiceCollection" /> in the <see cref="GenzorHost"/>.
		/// </summary>
		/// <param name="configure">The <see cref="Microsoft.Extensions.Logging.ILoggingBuilder"/> configuration delegate.</param>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddLogging(Action<ILoggingBuilder> configure)
		{
			collection.AddLogging(configure);
			return this;
		}

		/// <summary>
		/// Invoke (render) the <typeparamref name="TComponent"/> generator and add
		/// any <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> components
		/// to the <see cref="IFileSystem"/> registered with the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="TComponent">The generator component to invoke.</typeparam>
		/// <param name="initialParameters">Optional parameters to pass to the generator.</param>
		/// <returns>A <see cref="Task"/> that completes when the generator finishes.</returns>
		public Task InvokeGeneratorAsync<TComponent>(ParameterView? initialParameters = null) where TComponent : IComponent
			=> Renderer.InvokeGeneratorAsync<TComponent>(initialParameters);

		/// <inheritdoc/>
		public void Dispose()
		{
			renderer?.Dispose();
			serviceProvider?.Dispose();
		}
	}
}
