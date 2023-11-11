using CommunityToolkit.Mvvm.ComponentModel;

namespace Synergy.WPF.Navigation.ViewModels
{
	public partial class DialogViewModel : ViewModel
	{
		[ObservableProperty]
		private bool? _dialogResult;

		public virtual void ConfigureWindow(System.Windows.Window window) { }
	}
}
