using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Synergy.WPF.Navigation.ViewModels
{
	public abstract class ViewModel : ObservableRecipient, IDisposable
	{
		public static Dispatcher Dispatcher => Application.Current.Dispatcher;

		public virtual void Dispose() { }
	}
}
