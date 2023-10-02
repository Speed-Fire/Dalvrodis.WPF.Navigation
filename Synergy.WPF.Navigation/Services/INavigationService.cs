using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.WPF.Navigation.Services
{
    public interface INavigationService
    {
        ViewModel? CurrentView { get; }
        void NavigateTo<TViewModel>() where TViewModel : ViewModel;
        void NavigateTo(ViewModel viewModel);
    }
}
