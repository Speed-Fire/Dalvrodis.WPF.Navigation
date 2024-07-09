using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Synergy.WPF.Navigation.Components
{
	/// <summary>
	/// Логика взаимодействия для UserControlFrame.xaml
	/// </summary>
	public partial class UserControlFrame : UserControl, IDisposable
	{
		public UserControlFrame(UserControlFrameVM vm)
		{
			InitializeComponent();
			
			DataContext = vm;
		}

		public void Dispose()
		{
			if (Parent is not Visual visual)
				return;
			
			var removeVisualChild = typeof(Visual)
				.GetMethod(
					"RemoveVisualChild",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
					[typeof(Visual)]
				);

			if (removeVisualChild is null)
				return;

			Dispatcher?.Invoke(() => removeVisualChild?.Invoke(visual, [this]));
		}
	}
}
