using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Misc;
using Synergy.WPF.Navigation.ViewModels;
using Synergy.WPF.Navigation.Windows;
using System;

#nullable disable

namespace Synergy.WPF.Navigation.Services.Dialog
{
	[Obsolete]
	internal class DialogService : IDialogService
	{
		private readonly IServiceProvider _service;

		public DialogService(IServiceProvider service)
		{
			_service = service;
		}

		public DReturnValue<TReturn> Show<TViewModel, TReturn>(
			DialogCallback<TReturn> callback = null,
			ConfigureContext<TViewModel> configure = null,
			bool adjustScale = true)
			where TViewModel : ViewModel
		{
			using var scope = InitDialog(configure, out DialogHostViewModel dialogHost);

			var result = ShowDialogInternal(dialogHost, callback, adjustScale);

			// dispose scope.
			scope.Dispose();

			return result;
		}

		/// <summary>
		/// Shows dialog with specified context. Before using this method make sure, that you registered
		/// a type of view for the type of viewmodel.
		/// </summary>
		/// <typeparam name="TViewModel">Context.</typeparam>
		/// <param name="callback">Callback will be executed, when dialog is closed.</param>
		/// <param name="configure">Configuration method for your context.</param>
		/// <param name="adjustScale">Adjust view scale to current monitor scale.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void Show<TViewModel>(
			DialogCallback callback = null,
			ConfigureContext<TViewModel> configure = null,
			bool adjustScale = true)
			where TViewModel : ViewModel
		{
			using var scope = InitDialog(configure, out DialogHostViewModel dialogHost);

			ShowDialogInternal(dialogHost, callback, adjustScale);
		}

		private IServiceScope InitDialog<TViewModel>(
			ConfigureContext<TViewModel> configure,
			out DialogHostViewModel dialogHost)
			where TViewModel : ViewModel
		{
			// create scope.
			var scopeFactory = _service.GetRequiredService<IServiceScopeFactory>(); ;
			var scope = scopeFactory.CreateScope();

			// create dialog host.
			dialogHost = scope.ServiceProvider.GetRequiredService<DialogHostViewModel>();
			var innerNavService = scope.ServiceProvider
				.GetRequiredKeyedService<INavigationService>(NavConsts.SCOPED_SERVICE);

			// navigate dialog host to required vm.
			innerNavService.NavigateTo<TViewModel>();

			if (dialogHost.DialogResult != null)
				dialogHost.DialogResult = null;

			// configure required vm.
			configure?.Invoke((TViewModel)innerNavService.CurrentViewModel);

			return scope;
		}

		private static void ShowDialogInternal(
			DialogHostViewModel dialogHost,
			DialogCallback callback,
			bool adjustScale)
		{
			var dialog = new DialogWindow(adjustScale);
			ConfigureDialog(dialogHost, dialog);

			dialog.ShowDialog();

			// probably will be disposed within the scope.
			//dialogHost.Dispose();

			// Don't use window's DialogResult!!!
			callback?.Invoke(dialogHost.DialogResult);
		}


		private static DReturnValue<TReturn> ShowDialogInternal<TReturn>(
			DialogHostViewModel dialogHost,
			DialogCallback<TReturn> callback,
			bool adjustScale)
		{
			var dialog = new DialogWindow(adjustScale);
			ConfigureDialog(dialogHost, dialog);

			dialog.ShowDialog();

			// probably will be disposed within the scope.
			//dialogHost.Dispose();

			// Don't use window's DialogResult!!!
			var retVal = new DReturnValue<TReturn>(
				dialogHost.DialogResult,
				(TReturn)dialogHost.ReturnValue);

			callback?.Invoke(retVal);

			return retVal;
		}


		private static void ConfigureDialog(DialogHostViewModel dialogHost, DialogWindow dialog)
		{
			dialogHost.ConfigureWindow(dialog);

			var content = new UserControlFrame(dialogHost.UcFrameVM);
			dialog.Content = content;
			dialog.DataContext = dialogHost;

			EventHandler closeEventHandler = null;
			System.ComponentModel.CancelEventHandler closingEventHandler = null;
			closeEventHandler = (sender, e) =>
			{
				//callback?.Invoke(dialog.DialogResult);

				dialog.Closed -= closeEventHandler;
			};
			closingEventHandler = (sender, e) =>
			{
				dialog.DialogContent = null;
				dialog.Closing -= closingEventHandler;
			};

			dialog.Closing += closingEventHandler;
			dialog.Closed += closeEventHandler;

			//dialog.DialogContent = content;
			//(content as FrameworkElement).DataContext = vm;
		}
	}
}
