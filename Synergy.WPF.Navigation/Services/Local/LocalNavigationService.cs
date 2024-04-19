using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Linq;
using System.Reflection;

namespace Synergy.WPF.Navigation.Services.Local
{
	/// <summary>
	/// Implementation of local navigation service.
	/// </summary>
	public class LocalNavigationService : ObservableObject, ILocalNavigationService
	{
		private readonly Func<Type, ViewModel> _viewModelFactory;

		private ViewModel? _currentView;

		/// <summary>
		/// Current viewmodel.
		/// </summary>
		public ViewModel? CurrentView
		{
			get => _currentView;
			private set
			{
				SetProperty(ref _currentView, value);
			}
		}

		public LocalNavigationService(Func<Type, ViewModel> viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;
		}

		/// <summary>
		/// Navigates to viewmodel via type. Gets the viewmodel of specified type from DI.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		public void NavigateToDI<TViewModel>(bool suppressDisposing) where TViewModel : ViewModel
		{
			var vm = _viewModelFactory?.Invoke(typeof(TViewModel));

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = vm;
		}

		/// <summary>
		/// Navigates to viewmodel via type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		/// <param name="prms">Parameters for viewmodel constructor.</param>
		public void NavigateTo<TViewModel>(bool suppressDisposing, params object[] prms) where TViewModel : ViewModel
		{
			var constructor = GetSuitableConstructor<TViewModel>(prms);

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = (ViewModel)constructor.Invoke(prms);
		}

		/// <summary>
		/// Navigates to viewmodel via type.
		/// </summary>
		/// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		public void NavigateTo<TViewModel>(bool suppressDisposing) where TViewModel : ViewModel
		{
			var constructor = GetSuitableConstructor<TViewModel>(Array.Empty<object>());

			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = (ViewModel)constructor.Invoke(null);
		}

		/// <summary>
		/// Navigates to viewmodel via instance.
		/// </summary>
		/// <param name="viewModel">Instance of viewmodel.</param>
		/// <param name="suppressDisposing">Set true, if you don't want to dispose active viewmodel
		/// before setting new.</param>
		public void NavigateTo(ViewModel viewModel, bool suppressDisposing)
		{
			if (CurrentView != null && !suppressDisposing)
				CurrentView.Dispose();

			CurrentView = viewModel;
		}

		/// <summary>
		/// Gets suitable constructor for specified type by list of parameters.
		/// </summary>
		/// <typeparam name="T">Type, you want a constructor of.</typeparam>
		/// <param name="prms">Parameters for constructor.</param>
		/// <returns>Found suitable constructor.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		private static ConstructorInfo GetSuitableConstructor<T>(object[] prms)
		{
			var constructors = typeof(T).GetConstructors();

			if (constructors.Length == 0)
			{
				throw new InvalidOperationException("Class has no constructors!");
			}

			foreach (var constructor in constructors)
			{
				if (!constructor.IsPublic)
					continue;

				if (CompareConstructorParameters(constructor.GetParameters().Select(p => p.ParameterType).ToArray(),
					prms))
					return constructor;
			}

			throw new InvalidOperationException("There is no suitable constructor for specified parameters!");
		}

		/// <summary>
		/// Checks if given parameters can be assigned to constructor.
		/// </summary>
		/// <param name="prms1">Constructor accepted parameter types.</param>
		/// <param name="prms2">Given parameters.</param>
		/// <returns>True, if can be assigned, otherwise - false.</returns>
		private static bool CompareConstructorParameters(Type[] prms1, object[] prms2)
		{
			if (prms1.Length != prms2.Length)
				return false;

			for (int i = 0; i < prms1.Length; i++)
			{
				if (!prms1[i].IsAssignableFrom(prms2[i].GetType()))
					return false;
			}

			return true;
		}
	}
}
