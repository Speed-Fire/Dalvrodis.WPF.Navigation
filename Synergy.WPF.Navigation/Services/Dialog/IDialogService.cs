using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Synergy.WPF.Navigation.Services.Dialog
{
	public delegate void DialogCallback(bool? result);
	public delegate void ConfigureContext<TViewModel>(TViewModel viewModel);

	public interface IDialogService
	{
		void ShowDialog<TViewModel>(DialogCallback? callback = null, ConfigureContext<TViewModel>? configure = null)
			where TViewModel : DialogViewModel;
	}
}
