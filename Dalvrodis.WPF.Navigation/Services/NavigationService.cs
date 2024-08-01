using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Synergy.WPF.Navigation.Managers;
using Synergy.WPF.Navigation.Messages;
using Synergy.WPF.Navigation.Misc;
using Synergy.WPF.Navigation.Services.Dialog;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Synergy.WPF.Navigation.Services
{
	/// <summary>
	/// Implementation of navigation service. Uses DI to retrieve viewmodels.
	/// </summary>
	public sealed class NavigationService : INavigationService
	{
		#region Embedded types

		private class PreviousVMInfo
		{
			public required ViewModel ViewModel { get; set; }
			public DialogCallback? Callback { get; set; }
		}

		private class PreviousVMInfo<TReturnType> : PreviousVMInfo
		{
			public required DialogCallback<TReturnType> ParametrizedCallback { get; set; }
		}

		#endregion

		private readonly NavigationManager _navigationManager;
		private readonly object _key;
		private readonly Func<Type, ViewModel> _viewModelFactory;
		private readonly Stack<PreviousVMInfo> _dialogStack = [];

		private ViewModel? _currentViewModel;

		/// <inheritdoc/>
		public ViewModel? CurrentViewModel
		{
			get => _currentViewModel;
			private set => _currentViewModel = value;
		}

#nullable disable

		private Action<NavigationEventArgs> _navigated;
		event Action<NavigationEventArgs> INavigationService.Navigated
		{
			add => _navigated += value;
			remove => _navigated -= value;
		}

#nullable enable

        public NavigationService(
			NavigationManager manager,
			Func<Type, ViewModel> viewModelFactory,
			object key)
        {
            _viewModelFactory = viewModelFactory;
			_navigationManager = manager;
			_key = key;

			_navigationManager.Attach(key, this);
        }

        #region NavigateTo

		/// <inheritdoc/>
        public void NavigateTo<TViewModel>(bool suppressDisposing = false)
			where TViewModel : ViewModel
		{
			NavigateTo<TViewModel>(NavigationAction.CreateNew, suppressDisposing);
		}

		/// <inheritdoc/>
		public void NavigateTo(ViewModel? viewModel, bool suppressDisposing = false)
		{
			NavigateTo(NavigationAction.CreateNew, viewModel, suppressDisposing);
		}

		private void NavigateTo<TViewModel>(NavigationAction action, bool suppressDisposing = false)
			where TViewModel : ViewModel
		{
			var vm = _viewModelFactory?.Invoke(typeof(TViewModel));

			if (CurrentViewModel != null && !suppressDisposing)
				CurrentViewModel.Dispose();

			vm?.SetNavigation(this);

			CurrentViewModel = vm;

			InvokeNavigated(CurrentViewModel, action);
		}

		private void NavigateTo(NavigationAction action, ViewModel? viewModel,
			bool suppressDisposing = false)
		{
			if (CurrentViewModel != null && !suppressDisposing)
				CurrentViewModel.Dispose();

			viewModel?.SetNavigation(this);

			CurrentViewModel = viewModel;

			InvokeNavigated(CurrentViewModel, action);
		}

		#endregion

		#region PushDialog

		/// <inheritdoc/>
		public void PushDialog<TViewModel>(DialogCallback? callback = null) where TViewModel : ViewModel
		{
			if (CurrentViewModel is null)
				throw new InvalidOperationException("Can't open embedded dialog while general View Model is not set!");

			var info = new PreviousVMInfo()
			{
				ViewModel = CurrentViewModel,
				Callback = callback
			};

			PushDialog<TViewModel>(info);
		}

		/// <inheritdoc/>
		public void PushDialog<TViewModel, TReturnValue>(DialogCallback<TReturnValue> callback)
			where TViewModel : ViewModel
		{
			if (CurrentViewModel is null)
				throw new InvalidOperationException("Can't open embedded dialog while general View Model is not set!");

			var info = new PreviousVMInfo<TReturnValue>()
			{
				ViewModel = CurrentViewModel,
				ParametrizedCallback = callback
			};

			PushDialog<TViewModel>(info);
		}

		/// <inheritdoc/>
		public void PushDialog(ViewModel dialog, DialogCallback? callback = null)
		{
			if (CurrentViewModel is null)
				throw new InvalidOperationException("Can't open embedded dialog while general View Model is not set!");

			var info = new PreviousVMInfo()
			{
				ViewModel = CurrentViewModel,
				Callback = callback
			};

			PushDialog(dialog, info);
		}

		/// <inheritdoc/>
		public void PushDialog<TReturnValue>(ViewModel dialog, DialogCallback<TReturnValue> callback)
		{
			if (CurrentViewModel is null)
				throw new InvalidOperationException("Can't open embedded dialog while general View Model is not set!");

			var info = new PreviousVMInfo<TReturnValue>()
			{
				ViewModel = CurrentViewModel,
				ParametrizedCallback = callback
			};

			PushDialog(dialog, info);
		}

		private void PushDialog<TViewModel>(PreviousVMInfo prevInfo)
			where TViewModel : ViewModel
		{
			_dialogStack.Push(prevInfo);
			this.NavigateTo<TViewModel>(NavigationAction.PushToStack, true);
		}

		private void PushDialog(ViewModel vm, PreviousVMInfo prevInfo)
		{
			_dialogStack.Push(prevInfo);
			this.NavigateTo(NavigationAction.PushToStack, vm, true);
		}

		#endregion

		#region ReleaseDialog

		/// <inheritdoc/>
		public void ReleaseDialog(bool? result = null)
		{
			if (_dialogStack.Count == 0)
				throw new InvalidOperationException("Dialog stack is empty!");

			var info = _dialogStack.Pop();

			this.NavigateTo(NavigationAction.ReleaseFromStack, info.ViewModel);

			if (info.Callback is null)
				return;

			info.Callback.Invoke(result);
		}

		/// <inheritdoc/>
		public void ReleaseDialog<TReturnValue>(bool? result, TReturnValue returnValue)
		{
			if (_dialogStack.Count == 0)
				throw new InvalidOperationException("Dialog stack is empty!");

			var data = _dialogStack.Pop();
			if (data is not PreviousVMInfo<TReturnValue> info)
				throw new InvalidOperationException("Callback for such type of return value is not registered!");

			this.NavigateTo(NavigationAction.ReleaseFromStack, info.ViewModel);
			info.ParametrizedCallback.Invoke(new DReturnValue<TReturnValue>(result, returnValue));
		}

		#endregion

		private void InvokeNavigated(ViewModel? vm, NavigationAction action)
		{
			_navigated?.Invoke(new(vm, action));
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			_navigationManager.Detach(_key);
		}
	}
}
