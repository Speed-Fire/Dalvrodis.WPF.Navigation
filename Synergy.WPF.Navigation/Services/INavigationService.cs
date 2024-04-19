using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services
{
	public interface INavigationService
	{
		ViewModel? CurrentView { get; }
		void NavigateTo<TViewModel>(bool suppressDisposing = false) where TViewModel : ViewModel;
		void NavigateTo(ViewModel viewModel, bool suppressDisposing = false);
	}
}
