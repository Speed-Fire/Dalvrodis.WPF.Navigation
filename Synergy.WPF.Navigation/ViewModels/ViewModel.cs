using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

#nullable disable

namespace Synergy.WPF.Navigation.ViewModels
{
	public abstract class ViewModel<TView> : ViewModel
		where TView : UserControl
	{

	}

	/// <summary>
	/// Base viewmodel class.
	/// </summary>
	[ObservableRecipient]
	public abstract partial class ViewModel : ObservableValidator, IDisposable
	{
		/// <summary>
		/// Application dispatcher.
		/// </summary>
		public static Dispatcher Dispatcher => Application.Current.Dispatcher;

		private INavigationService _navigation;
		protected INavigationService Navigation => _navigation;

		public virtual void Validate()
		{
			ValidateAllProperties();
		}

		internal void SetNavigation(INavigationService navigation)
		{
			_navigation = navigation;
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public virtual void Dispose() { }
	}
}
