using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Synergy.WPF.Navigation.ViewModels
{
	public partial class DialogViewModel : ViewModel
	{
		[ObservableProperty]
		private bool? _dialogResult;

		public object? ReturnValue { get; protected set; }

		public void ConfigureWindow(System.Windows.Window window)
		{
			if(window == null)
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
