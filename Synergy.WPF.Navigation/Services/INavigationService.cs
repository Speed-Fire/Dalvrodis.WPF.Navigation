using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services
{
	/// <summary>
	/// Interface for providing navigation.
	/// </summary>
	public interface INavigationService
	{
		/// <summary>
		/// Current viewmodel.
		/// </summary>
		ViewModel? CurrentView { get; }

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
	}
}
