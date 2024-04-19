using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.ViewModels;
using System;

namespace Synergy.WPF.Navigation.Services.Global
{
	/// <summary>
	/// Implementation of navigation service. Uses DI to retrieve viewmodels.
	/// </summary>
	public class NavigationService : ObservableObject, INavigationService
	{
		private readonly Func<Type, ViewModel> _viewModelFactory;

		private ViewModel? _currentView;

		/// <summary>
		/// Current viewmodel.
		/// </summary>
		public ViewModel? CurrentView
		{
			get => _currentView;
			private set => SetProperty(ref _currentView, value);
		}

		public NavigationService(Func<Type, ViewModel> viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;
		}

		/// <summary>
		/// Navigates to viewmodel via type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		void INavigationService.NavigateTo<TViewModel>(bool suppressDisposing)
		{
			var vm = _viewModelFactory?.Invoke(typeof(TViewModel));

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = vm;
		}

		/// <summary>
		/// Navigates to viewmodel via instance.
		/// </summary>
		/// <param name="viewModel">Instance of viewmodel.</param>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		void INavigationService.NavigateTo(ViewModel viewModel, bool suppressDisposing)
		{
			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = viewModel;
		}
	}
}
