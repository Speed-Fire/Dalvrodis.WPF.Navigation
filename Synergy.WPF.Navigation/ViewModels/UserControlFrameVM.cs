using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.Extensions;
using Synergy.WPF.Navigation.Services;
using System;
using System.Windows.Controls;

namespace Synergy.WPF.Navigation.ViewModels
{
	public partial class UserControlFrameVM : ViewModel
	{
		private readonly INavigationService _navigation;
		private readonly IServiceProvider _serviceProvider;

		[ObservableProperty]
		private UserControl? _currentView;

		public UserControlFrameVM(INavigationService navigation,
			IServiceProvider serviceProvider)
		{
			_navigation = navigation;

			_navigation.PropertyChanged += Navigation_PropertyChanged;
			_serviceProvider = serviceProvider;
		}

		private void Navigation_PropertyChanged(object? sender,
			System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(INavigationService.CurrentViewModel))
			{
				var vm = _navigation.CurrentViewModel;

				if (CurrentView is not null)
				{
					CurrentView.DataContext = null;
					CurrentView = null;
				}

				if (vm is not null)
				{
					var vmType = vm.GetType();

					if (!vmType.IsDerivedFromGenericType(typeof(ViewModel<>)) ||
						vmType.BaseType is null)
						return;

					var viewType = vmType.BaseType.GetGenericArguments()[0];

					if ((CurrentView = UpdateCurrentView(viewType)) is not null)
					{
						CurrentView.DataContext = vm;
					}
				}
			}
		}

		private UserControl? UpdateCurrentView(Type type)
		{
			// try get service from DI container.
			UserControl? control = (UserControl?)_serviceProvider.GetService(type);

			// if DI container doesn't have it, then try to create object by reflection.
			if (control is null)
			{
				var constructor = type.GetConstructor(Type.EmptyTypes);

				// return, if there's no parameterless constructor.
				if (constructor is null)
					return CurrentView;

				// create an instance of user control.
				control = (UserControl)constructor.Invoke(null);
			}

			// Update CurrentView
			return control;
		}

		public override void Dispose()
		{
			_navigation.PropertyChanged -= Navigation_PropertyChanged;
		}
	}
}
