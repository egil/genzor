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
		/// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an implementation
		/// type specified in <typeparamref name="TImplementation"/> to the <see cref="GenzorHost"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddService<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			collection.AddSingleton<TService, TImplementation>();
			return this;
		}

		/// <summary>
		/// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an instance specified
		/// in <paramref name="implementationInstance"/> to the <see cref="GenzorHost"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <param name="implementationInstance">The instance of the service.</param>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddService<TService>(TService implementationInstance)
			where TService : class
		{
			collection.AddSingleton<TService>(implementationInstance);
			return this;
		}

		/// <summary>
		/// Adds a singleton service of the type specified in <typeparamref name="TService"/> with a factory specified
		/// in <paramref name="implementationFactory"/> to the specified <see cref="GenzorHost"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <param name="implementationFactory">The factory that creates the service.</param>
		/// <returns>The <see cref="GenzorHost"/> so that additional calls can be chained.</returns>
		public GenzorHost AddService<TService>(Func<IServiceProvider, TService> implementationFactory)
			where TService : class
		{
			collection.AddSingleton<TService>(implementationFactory);
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
		public Task InvokeGeneratorAsync<TComponent>(ParameterView? initialParameters) where TComponent : IComponent
			=> Renderer.InvokeGeneratorAsync<TComponent>(initialParameters);

		/// <summary>
		/// Invoke (render) the <typeparamref name="TComponent"/> generator and add
		/// any <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> components
		/// to the <see cref="IFileSystem"/> registered with the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="TComponent">The generator component to invoke.</typeparam>
		/// <param name="parametersBuilder">A optional parameter builder action.</param>
		/// <returns>A <see cref="Task"/> that completes when the generator finishes.</returns>
		public Task InvokeGeneratorAsync<TComponent>(Action<ParameterViewBuilder<TComponent>>? parametersBuilder = null)
			where TComponent : IComponent
		{
			var pvb = new ParameterViewBuilder<TComponent>();
			parametersBuilder?.Invoke(pvb);
			return Renderer.InvokeGeneratorAsync<TComponent>(pvb.Build());
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			renderer?.Dispose();
			serviceProvider?.Dispose();
		}
	}
}
