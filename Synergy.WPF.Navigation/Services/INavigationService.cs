using Synergy.WPF.Navigation.Services.Dialog;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Synergy.WPF.Navigation.Services
{
	internal enum NavigationAction
	{
		CreateNew,
		PushToStack,
		ReleaseFromStack
	}

	internal record NavigationEventArgs(ViewModel? NewViewModel, NavigationAction NavigationAction);

	/// <summary>
	/// Interface for providing navigation.
	/// </summary>
	public interface INavigationService
	{
		internal event Action<NavigationEventArgs> Navigated;

		/// <summary>
		/// Current viewmodel.
		/// </summary>
		ViewModel? CurrentViewModel { get; }

		/// <summary>
		/// Navigates to viewmodel via type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		void NavigateTo<TViewModel>(bool suppressDisposing = false) where TViewModel : ViewModel;

		/// <summary>
		/// Navigates to viewmodel via instance.
		/// </summary>
		/// <param name="viewModel">Instance of viewmodel.</param>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		void NavigateTo(ViewModel viewModel, bool suppressDisposing = false);

		void PushDialog<TViewModel>(DialogCallback? callback = null)
			where TViewModel : ViewModel;
		void PushDialog<TViewModel, TReturnValue>(DialogCallback<TReturnValue> callback)
			where TViewModel : ViewModel;

		void PushDialog(ViewModel dialog, DialogCallback? callback = null);
		void PushDialog<TReturnValue>(ViewModel dialog, DialogCallback<TReturnValue> callback);

		void ReleaseDialog(bool? result = null);
		void ReleaseDialog<TReturnValue>(bool? result, TReturnValue returnValue);
	}
}
