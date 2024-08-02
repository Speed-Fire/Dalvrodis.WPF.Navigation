using Dalvrodis.WPF.Navigation.Misc;
using Dalvrodis.WPF.Navigation.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Dalvrodis.WPF.Navigation.Services
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
	public interface INavigationService : IDisposable
	{
		internal event Action<NavigationEventArgs> Navigated;

		/// <summary>
		/// Current ViewModel.
		/// </summary>
		ViewModel? CurrentViewModel { get; }

		/// <summary>
		/// Navigates to ViewModel by its type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active ViewModel
		/// before setting new.</param>
		void NavigateTo<TViewModel>(bool suppressDisposing = false) where TViewModel : ViewModel;

		/// <summary>
		/// Navigates to ViewModel by instance.
		/// </summary>
		/// <param name="viewModel">Instance of ViewModel.</param>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active ViewModel
		/// before setting new.</param>
		void NavigateTo(ViewModel? viewModel, bool suppressDisposing = false);

		/// <summary>
		/// Pushes dialog to stack. Dialog must return only bool value.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel.</typeparam>
		/// <param name="callback">Callback to call when opened dialog finished.</param>
		void PushDialog<TViewModel>(DialogCallback? callback = null)
			where TViewModel : ViewModel;

		/// <summary>
		/// Pushes dialog to stack.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel.</typeparam>
		/// <typeparam name="TReturnValue">Type of value, which will be returned from the dialog.</typeparam>
		/// <param name="callback">Callback to call when opened dialog finished.</param>
		void PushDialog<TViewModel, TReturnValue>(DialogCallback<TReturnValue> callback)
			where TViewModel : ViewModel;

		/// <summary>
		/// Pushes dialog to stack. Dialog must return only bool value.
		/// </summary>
		/// <param name="dialog">Instance of dialog ViewModel.</param>
		/// <param name="callback">Callback to call when opened dialog finished.</param>
		void PushDialog(ViewModel dialog, DialogCallback? callback = null);

		/// <summary>
		/// Pushes dialog to stack.
		/// </summary>
		/// <typeparam name="TReturnValue">Type of value, which will be returned from the dialog.</typeparam>
		/// <param name="dialog">Instance of dialog ViewModel.</param>
		/// <param name="callback">Callback to call when opened dialog finished.</param>
		void PushDialog<TReturnValue>(ViewModel dialog, DialogCallback<TReturnValue> callback);

		/// <summary>
		/// Finishes the dialog and returns back to previous ViewModel.
		/// </summary>
		/// <param name="result">Dialog result.</param>
		void ReleaseDialog(bool? result = null);

		/// <summary>
		/// Finishes the dialog and returns back to previous ViewModel.
		/// </summary>
		/// <typeparam name="TReturnValue">Type of value, which will be returned from the dialog.</typeparam>
		/// <param name="result">Dialog result.</param>
		/// <param name="returnValue">Returning value.</param>
		void ReleaseDialog<TReturnValue>(bool? result, TReturnValue returnValue);
	}
}
