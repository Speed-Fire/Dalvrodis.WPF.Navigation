using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.WPF.Navigation.Services.Local
{
    public class LocalNavigationService : ObservableObject, ILocalNavigationService
    {
		private readonly Func<Type, ViewModel> _viewModelFactory;

		private ViewModel? _currentView;
        public ViewModel? CurrentView
        {
            get => _currentView;
            private set
            {
                SetProperty(ref _currentView, value);
			}
        }

        public LocalNavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
		}

		public void NavigateToDI<TViewModel>(bool suppressDisposing) where TViewModel : ViewModel
		{
			var vm = _viewModelFactory?.Invoke(typeof(TViewModel));

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = vm;
		}

		public void NavigateTo<TViewModel>(bool suppressDisposing, params object[] prms) where TViewModel : ViewModel
        {
            var constructor = GetSuitableConstructor<TViewModel>(prms);

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = (ViewModel)constructor.Invoke(prms);
        }

        public void NavigateTo<TViewModel>(bool suppressDisposing) where TViewModel : ViewModel
        {
            var constructor = GetSuitableConstructor<TViewModel>(Array.Empty<object>());

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = (ViewModel)constructor.Invoke(null);
        }

        public void NavigateTo(ViewModel viewModel, bool suppressDisposing)
        {
			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = viewModel;
        }

        private ConstructorInfo GetSuitableConstructor<T>(object[] prms)
        {
            var constructors = typeof(T).GetConstructors();

            if(constructors.Length == 0)
            {
                throw new InvalidOperationException("Class has no constructors!");
            }

            foreach(var constructor in constructors)
            {
                if (!constructor.IsPublic)
                    continue;

                if (CompareConstructorParameters(constructor.GetParameters().Select(p => p.ParameterType).ToArray(),
                    prms))
                    return constructor;
            }

            throw new InvalidOperationException("There is no suitable constructor for specified parameters!");
        }

        private bool CompareConstructorParameters(Type[] prms1, object[] prms2)
        {
            if(prms1.Length != prms2.Length)
                return false;

            for(int i = 0; i < prms1.Length; i++)
            {
                if (!prms1[i].IsAssignableFrom(prms2[i].GetType()))
                    return false;
            }

            return true;
        }
	}
}
