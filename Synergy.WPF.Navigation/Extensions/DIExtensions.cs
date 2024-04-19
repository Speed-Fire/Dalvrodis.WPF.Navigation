using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.Services.Global;
using Synergy.WPF.Navigation.ViewModels;
using System;

namespace Synergy.WPF.Navigation.Extensions
{
	public static class DIExtensions
	{
		public const string NAV_SERVICE_LOCAL_SCOPED = "Synergy.WPF.Navigation.Local";
		public const string NAV_SERVICE_LOCAL_TRANSIENT = "Synergy.WPF.Navigation.Local.Transient";

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
				.AddSingleton<INavigationService, NavigationService>(provider =>
				{
					return new NavigationService(type => (ViewModel)provider.GetRequiredService(type));
				})
				.AddKeyedScoped<INavigationService>(NAV_SERVICE_LOCAL_SCOPED, (provider, key) =>
				{
					return new NavigationService(type => (ViewModel)provider.GetRequiredKeyedService(type, key));
				})
				.AddKeyedTransient<INavigationService>(NAV_SERVICE_LOCAL_TRANSIENT, (provider, key) =>
				{
					return new NavigationService(type => (ViewModel)provider.GetRequiredKeyedService(type, key));
				});

			return services;
		}
	}
}
