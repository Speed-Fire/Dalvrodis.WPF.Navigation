using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Synergy.WPF.Navigation.ViewModels
{
#nullable disable
	/// <summary>
	/// Dialog viewmodel class with supporting of return value.
	/// </summary>
	/// <typeparam name="TReturn"></typeparam>
	public partial class DialogViewModel<TReturn> : DialogViewModel
	{
		public TReturn ReturnValue { get; protected set; }
	}
#nullable enable

	/// <summary>
	/// Base dialog viewmodel class.
	/// </summary>
	public partial class DialogViewModel : ViewModel
	{
		public string DialogNumber { get; set; } = "";

		[ObservableProperty]
		private bool? _dialogResult;

		/// <summary>
		/// Configures window to dispose this viewmodel on closed.
		/// </summary>
		/// <param name="window">Window to configure.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void ConfigureWindow(System.Windows.Window window)
		{
			if (window == null)
				throw new ArgumentNullException(nameof(window));

			EventHandler? closedHandler = null;
			closedHandler = (sender, e) =>
			{
				this.Dispose();

				window.Closed -= closedHandler;
			};
			window.Closed += closedHandler;

			ConfigureWindowInternal(window);
		}

		/// <summary>
		/// Additional configuration for window.
		/// </summary>
		/// <param name="window">Window to configure.</param>
		protected virtual void ConfigureWindowInternal(System.Windows.Window window) { }
	}
}
