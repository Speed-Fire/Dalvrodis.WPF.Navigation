using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Synergy.WPF.Navigation.ViewModels
{
	/// <summary>
	/// Base viewmodel class.
	/// </summary>
	public abstract class ViewModel : ObservableRecipient, IDisposable
	{
		/// <summary>
		/// Application dispatcher.
		/// </summary>
		public static Dispatcher Dispatcher => Application.Current.Dispatcher;

		/// <summary>
		/// Dispose.
		/// </summary>
		public virtual void Dispose() { }
	}
}
