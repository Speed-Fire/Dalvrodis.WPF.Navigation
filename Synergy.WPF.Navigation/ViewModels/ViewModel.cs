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

		/// <summary>
		/// Searches for specified resource in application. If resource is not found
		///  or specified type is wrong, exception will be thrown.
		/// </summary>
		/// <typeparam name="T">Resource type.</typeparam>
		/// <param name="key">Resource key.</param>
		/// <returns>Found resource.</returns>
		public static T GetAppResource<T>(object key)
		{
			return (T)Application.Current.FindResource(key);
		}

#nullable enable
		/// <summary>
		/// Searches for specified resource.
		/// </summary>
		/// <typeparam name="T">Resource type.</typeparam>
		/// <param name="key">Resource key.</param>
		/// <returns>Found resource or null, if not found or can't be casted to specified type.</returns>
		public static T? TryGetAppResource<T>(object key) where T : class
		{
			return Application.Current.TryFindResource(key) as T;
		}
	}
}
