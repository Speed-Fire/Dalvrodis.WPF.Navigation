using CommunityToolkit.Mvvm.ComponentModel;
using Dalvrodis.WPF.Navigation.Extensions;
using Dalvrodis.WPF.Navigation.Services;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Dalvrodis.WPF.Navigation.ViewModels
{
	public sealed partial class UserControlFrameVM : ViewModel
	{
		private readonly INavigationService _navigation;
		private readonly IServiceProvider _serviceProvider;

		private readonly Stack<UserControl> _userControlStack = [];

		[ObservableProperty]
		private UserControl? _currentView;

		public UserControlFrameVM(INavigationService navigation,
			IServiceProvider serviceProvider)
		{
			_navigation = navigation;

			_navigation.Navigated += Navigation_Navigated;
			_serviceProvider = serviceProvider;
		}

		private void Navigation_Navigated(NavigationEventArgs e)
		{
			Dispatcher?.Invoke(() => NavigateView(e));
		}

		private void NavigateView(NavigationEventArgs e)
		{
			var vm = e.NewViewModel;

			if (CurrentView is not null && e.NavigationAction != NavigationAction.PushToStack)
			{
				CurrentView.DataContext = null;
				CurrentView = null;
			}

			if (e.NavigationAction == NavigationAction.PushToStack)
			{
				if (CurrentView is null)
					throw new InvalidOperationException("There is no CurrentView to push to stack!");

				_userControlStack.Push(CurrentView);
			}

			if (e.NavigationAction == NavigationAction.ReleaseFromStack)
			{
				if (_userControlStack.Count == 0)
					throw new InvalidOperationException("UserControl stack is empty!");

				CurrentView = _userControlStack.Pop();
			}
			else if (vm is not null)
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
			_navigation.Navigated -= Navigation_Navigated;
		}
	}
}
