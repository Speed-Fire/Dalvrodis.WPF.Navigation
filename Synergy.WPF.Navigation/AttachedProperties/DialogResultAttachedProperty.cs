using System;
using System.Windows;

namespace Synergy.WPF.Navigation.AttachedProperties
{
	/// <summary>
	/// Dialog Result attached property.
	/// </summary>
	public class DialogResultAttachedProperty
	{
		#region Attached Property Definitions

		/// <summary>
		/// The attached property for this class
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
			"Value",
			typeof(bool?),
			typeof(DialogResultAttachedProperty),
			new UIPropertyMetadata(
				default(bool?),
				new PropertyChangedCallback(OnValueChanged)));

		/// <summary>
		/// Gets the attached property
		/// </summary>
		/// <param name="d">The element to get the property from</param>
		/// <returns></returns>
		public static bool? GetValue(DependencyObject d) => (bool?)d.GetValue(ValueProperty);

		/// <summary>
		/// Sets the attached property
		/// </summary>
		/// <param name="d">The element to get the property from</param>
		/// <param name="value">The value to set the property to</param>
		public static void SetValue(DependencyObject d, bool? value) => d.SetValue(ValueProperty, value);

		#endregion

		#region Value changed

		/// <summary>
		/// The method that is called when value is changed.
		/// </summary>
		/// <param name="sender">The UI element that this property was changed for</param>
		/// <param name="e">The arguments for this event</param>
		public static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not Window window)
				throw new InvalidOperationException("Can't set dialog result to not Window object.");

			if (window != null)
				window.DialogResult = e.NewValue as bool?;
		}

		#endregion
	}
}
