using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.WPF.Navigation.Services.Global
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly Func<Type, ViewModel> _viewModelFactory;

        private ViewModel? _currentView;
        public ViewModel? CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        void INavigationService.NavigateTo<TViewModel>()
        {
            var vm = _viewModelFactory?.Invoke(typeof(TViewModel));
            CurrentView = vm;
        }

        void INavigationService.NavigateTo(ViewModel viewModel)
        {
            CurrentView = viewModel;
        }
    }
}
