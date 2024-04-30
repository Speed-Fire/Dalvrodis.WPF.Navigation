using Synergy.WPF.Navigation.ViewModels;

namespace Synergy.WPF.Navigation.Services.Dialog
{
	/// <summary>
	/// Dialog close callback.
	/// </summary>
	/// <param name="returnValue">Dialog return value.</param>
	public delegate void DialogCallback<T>(DReturnValue<T> returnValue);

	/// <summary>
	/// Dialog close callback.
	/// </summary>
	/// <param name="dialogResult">Dialog result.</param>
	public delegate void DialogCallback(bool? dialogResult);

	/// <summary>
	/// Viewmodel configuring context.
	/// </summary>
	/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
	/// <param name="viewModel">Viewmodel to configure.</param>
	public delegate void ConfigureContext<TViewModel>(TViewModel viewModel)
		where TViewModel : ViewModel;

	/// <summary>
	/// Record of dialog result and its return value.
	/// </summary>
	/// <typeparam name="T">Type of return value.</typeparam>
	/// <param name="Result">Dialog result.</param>
	/// <param name="ReturnValue">Dialog return value.</param>
	public record DReturnValue<T>(bool? Result, T? ReturnValue);

	/// <summary>
	/// Interface for providing dialogs to user.
	/// </summary>
	public interface IDialogService
	{
		/// <summary>
		/// Shows a dialog with no result return to point of call.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="callback">Dialog close callback.</param>
		/// <param name="configure">Viewmodel configuration.</param>
		/// /// <param name="adjustScale">Adjust view scale to current monitor scale.</param>
		void Show<TViewModel>(
			DialogCallback? callback = null,
			ConfigureContext<TViewModel>? configure = null,
			bool adjustScale = true)
			where TViewModel : ViewModel;

		/// <summary>
		/// Shows a dialog.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <typeparam name="TReturn">Type of dialog return value.</typeparam>
		/// <param name="callback">Dialog close callback.</param>
		/// <param name="configure">Viewmodel configuration.</param>
		/// <param name="adjustScale">Adjust view scale to current monitor scale.</param>
		/// <returns>Dialog result and return value.</returns>
		DReturnValue<TReturn> Show<TViewModel, TReturn>(
			DialogCallback<TReturn>? callback = null,
			ConfigureContext<TViewModel>? configure = null,
			bool adjustScale = true)
			where TViewModel : ViewModel;
	}
}
