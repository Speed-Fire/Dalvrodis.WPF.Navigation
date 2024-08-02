using Microsoft.Extensions.DependencyInjection;
using Dalvrodis.WPF.Navigation.Components;
using Dalvrodis.WPF.Navigation.Managers;
using Dalvrodis.WPF.Navigation.Misc;
using Dalvrodis.WPF.Navigation.Services;
using Dalvrodis.WPF.Navigation.ViewModels;
using System;
using System.Linq;

namespace Dalvrodis.WPF.Navigation.Extensions
{
	public static class DIExtensions
	{
		/// <summary>
		/// Adds navigation service to DI.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddNavigation(this IServiceCollection services)
		{
			services
				.AddNavigationServices()
				.AddAdditions();

			return services;
		}

		private static IServiceCollection AddNavigationServices(this IServiceCollection services)
		{
			services
				.AddKeyedSingleton<INavigationService, NavigationService>(
					NavConsts.SINGLETON_SERVICE,
					(provider, key) => CreateNavigationService(provider, key, NavConsts.MAIN_NAVIGATION_CHANNEL))
				.AddTransient<Func<object, INavigationService>>(provider => channel =>
				{
					if (channel is string str && str == NavConsts.MAIN_NAVIGATION_CHANNEL)
						throw new InvalidOperationException($"Channel \"{NavConsts.MAIN_NAVIGATION_CHANNEL}\" is preserved for singleton navigation service!");

					return CreateNavigationService(provider, null, channel);
				});

			services
				.AddSingleton<UserControlFrame>(provider =>
				{
					var manager = provider.GetRequiredService<NavigationManager>();

					return manager.CreateMainNavigationFrame();
				});

			return services;
		}

		private static IServiceCollection AddAdditions(this IServiceCollection services)
		{
			services
				.AddSingleton<NavigationManager>();

			return services;
		}

		private static NavigationService CreateNavigationService(
			IServiceProvider provider,
			object? key,
			object channel)
		{
			Func<Type, ViewModel> factory = type =>
			{
				var vm = (ViewModel?)provider.GetKeyedServices(type, key).FirstOrDefault();

				vm ??= (ViewModel)provider.GetRequiredService(type);

				return vm;
			};

			return (NavigationService)ActivatorUtilities.CreateInstance(provider,
				typeof(NavigationService), factory, channel);
		}
	}
}
