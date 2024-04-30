using Synergy.WPF.Navigation.Messages;
using Synergy.WPF.Navigation.Misc;
using System.Windows.Controls;

namespace Synergy.WPF.Navigation.ViewModels
{
	public class DialogViewModel<TView> : ViewModel<TView> where TView : UserControl
	{
		private readonly GuidWrapper _guid;

		public DialogViewModel(GuidWrapper guid)
		{
			_guid = guid;
		}

		protected void FinishDialog(bool? dialogResult, object? returnValue)
		{
			Messenger.Send(new DialogResultMessage((dialogResult, returnValue)), _guid.Guid);
		}
	}
}
