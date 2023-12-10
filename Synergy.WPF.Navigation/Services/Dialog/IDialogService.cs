using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services.Dialog
{
	public delegate void DialogCallback(bool? result, object? returnValue = null);
	public delegate void ConfigureContext<TViewModel>(TViewModel viewModel);

	public interface IDialogService
	{
		void ShowDialog<TViewModel>(DialogCallback? callback = null, ConfigureContext<TViewModel>? configure = null)
			where TViewModel : DialogViewModel;
	}
}
