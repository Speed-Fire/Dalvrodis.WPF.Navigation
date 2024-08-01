using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Synergy.WPF.Navigation.Messages
{
	internal class DialogResultMessage : ValueChangedMessage<(bool? Result, object? ReturnValue)>
	{
		public DialogResultMessage((bool? Result, object? ReturnValue) value) : base(value)
		{
		}
	}
}
