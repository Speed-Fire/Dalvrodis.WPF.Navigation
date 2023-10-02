using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.Services.Global;
using Synergy.WPF.Navigation.Services.Local;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.WPF.Navigation.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection RegisterNavigation(this IServiceCollection services)
        {
            services
                .AddSingleton<INavigationService, NavigationService>()
                .AddSingleton<Func<Type, ViewModel>>(provider => vmType => (ViewModel)provider.GetRequiredService(vmType))
                .AddTransient<ILocalNavigationService, LocalNavigationService>();

            return services;
        }
    }
}
