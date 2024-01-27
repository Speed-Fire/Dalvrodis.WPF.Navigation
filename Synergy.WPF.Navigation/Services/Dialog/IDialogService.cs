using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services.Dialog
{
	public delegate void DialogCallback(bool? result, object? returnValue = null);
	public delegate void ConfigureContext<TViewModel>(TViewModel viewModel);

	public record DReturnValue<T>(bool? Result, T? ReturnValue);

	public interface IDialogService
	{
		void Show<TViewModel>(DialogCallback? callback = null, ConfigureContext<TViewModel>? configure = null)
			where TViewModel : DialogViewModel;

		DReturnValue<TReturn> Show<TViewModel, TReturn>(DialogCallback? callback = null, ConfigureContext<TViewModel>? configure = null)
			where TViewModel : DialogViewModel<TReturn>;
	}
}
