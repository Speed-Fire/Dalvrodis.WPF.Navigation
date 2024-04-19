using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.Services.Global;
using Synergy.WPF.Navigation.Services.Local;
using Synergy.WPF.Navigation.ViewModels;
using System;

namespace Synergy.WPF.Navigation.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterSynergyWPFNavigation(this IServiceCollection services)
		{
			services
				.AddSingleton<INavigationService, NavigationService>()
				.AddSingleton<Func<Type, ViewModel>>(provider => vmType => (ViewModel)provider.GetRequiredService(vmType))
				.AddTransient<ILocalNavigationService, LocalNavigationService>();

			return services;
		}
	}
}
