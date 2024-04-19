using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services.Local
{
	/// <summary>
	/// Interface for providing local navigation.
	/// </summary>
	public interface ILocalNavigationService : INavigationService
	{
		/// <summary>
		/// Navigates to viewmodel via type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		/// <param name="prms">Parameters for viewmodel constructor.</param>
		void NavigateTo<TViewModel>(bool suppressDisposing = false, params object[] prms) where TViewModel : ViewModel;

		/// <summary>
		/// Navigates to viewmodel via type. Gets the viewmodel of specified type from DI.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		void NavigateToDI<TViewModel>(bool suppressDisposing = false) where TViewModel : ViewModel;
	}
}
