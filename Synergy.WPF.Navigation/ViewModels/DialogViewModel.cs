using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Synergy.WPF.Navigation.ViewModels
{
#nullable disable
	public partial class DialogViewModel<TReturn> : DialogViewModel
	{
		public TReturn ReturnValue { get; protected set; }
	}
#nullable enable


	public partial class DialogViewModel : ViewModel
	{
		public string DialogNumber { get; set; } = "";

		[ObservableProperty]
		private bool? _dialogResult;

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

		protected virtual void ConfigureWindowInternal(System.Windows.Window window) { }
	}
}
