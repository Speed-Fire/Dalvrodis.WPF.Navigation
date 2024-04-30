using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Misc;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.Services.Dialog;
using Synergy.WPF.Navigation.ViewModels;
using System.Linq;

namespace Synergy.WPF.Navigation.Extensions
{
	public static class DIExtensions
	{
		/// <summary>
		/// Register navigation.
		/// <para>- Global navigation service.</para>
		/// <para>- Local scoped navigation service with key "Synergy.WPF.Navigation.Local".</para>
		/// <para>- Local transient navigation service with key "Synergy.WPF.Navigation.Local.Transient".</para>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterSynergyWPFNavigation(this IServiceCollection services)
		{
			services
				.RegisterNavigationServices()
				.RegisterMainVM();

			return services;
		}

		private static IServiceCollection RegisterNavigationServices(this IServiceCollection services)
		{
			services
				.AddKeyedSingleton<INavigationService, NavigationService>(NavConsts.SINGLETON_SERVICE,
				(provider, key) =>
				{
					return new NavigationService(type =>
					{
						var vm = (ViewModel?)provider.GetKeyedServices(type, key).FirstOrDefault();

						if (vm is null)
							vm = (ViewModel)provider.GetRequiredService(type);

						return vm;
					});
				})
				.AddKeyedScoped<INavigationService, NavigationService>(NavConsts.SCOPED_SERVICE,
				(provider, key) =>
				{
					return new NavigationService(type =>
					{
						var vm = (ViewModel?)provider.GetKeyedServices(type, key).FirstOrDefault();

						if (vm is null)
							vm = (ViewModel)provider.GetRequiredService(type);

						return vm;
					});
				})
				.AddKeyedTransient<INavigationService>(NavConsts.TRANSIENT_SERVICE,
				(provider, key) =>
				{
					return new NavigationService(type =>
					{
						var vm = (ViewModel?)provider.GetKeyedServices(type, key).FirstOrDefault();

						if (vm is null)
							vm = (ViewModel)provider.GetRequiredService(type);

						return vm;
					});
				});

			return services;
		}

		private static IServiceCollection RegisterMainVM(this IServiceCollection services)
		{
			services
				.AddKeyedScoped<UserControlFrame>(NavConsts.SCOPED_SERVICE,
				(provider, key) =>
				{
					var frameVm = provider.GetRequiredKeyedService<UserControlFrameVM>(key);

					return new(frameVm);
				})
				.AddKeyedScoped<UserControlFrameVM>(NavConsts.SCOPED_SERVICE,
				(provider, key) =>
				{
					var navService = provider.GetRequiredKeyedService<INavigationService>(key);

					return new(navService, provider);
				})
				.AddSingleton<UserControlFrame>()
				.AddSingleton<UserControlFrameVM>(
				(provider) =>
				{
					var navService = provider
					.GetRequiredKeyedService<INavigationService>(NavConsts.SINGLETON_SERVICE);

					return new(navService, provider);
				});

			services
				.AddScoped<DialogHostViewModel>()
				.AddScoped<GuidWrapper>()
				.AddTransient<IDialogService, DialogService>();

			return services;
		}
	}
}
