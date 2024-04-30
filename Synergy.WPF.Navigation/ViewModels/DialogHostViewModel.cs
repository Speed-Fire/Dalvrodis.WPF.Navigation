using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Messages;
using Synergy.WPF.Navigation.Misc;
using System;

namespace Synergy.WPF.Navigation.ViewModels
{
	/// <summary>
	/// Base dialog viewmodel class.
	/// </summary>
	public partial class DialogHostViewModel : ViewModel, IRecipient<DialogResultMessage>
	{
		// Scope identifier.
		private readonly GuidWrapper _guid;

		private readonly UserControlFrameVM _ucFrameVM;
		public UserControlFrameVM UcFrameVM => _ucFrameVM;

		[ObservableProperty]
		private bool? _dialogResult;

		public object? ReturnValue { get; private set; }

		public DialogHostViewModel(
			[FromKeyedServices(NavConsts.SCOPED_SERVICE)] UserControlFrameVM ucFrameVM,
			GuidWrapper guid)
		{
			_ucFrameVM = ucFrameVM;

			_guid = guid;

			IsActive = true;
		}

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

		void IRecipient<DialogResultMessage>.Receive(DialogResultMessage message)
		{
			ReturnValue = message.Value.ReturnValue;
			DialogResult = message.Value.Result;
		}

		protected override void OnActivated()
		{
			this.Messenger.Register(this, _guid.Guid);
		}
	}
}
