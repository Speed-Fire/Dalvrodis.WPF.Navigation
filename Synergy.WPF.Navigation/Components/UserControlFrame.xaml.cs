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
			if (Parent is not FrameworkElement element)
				return;
			
			var removeLogicalChild = typeof(FrameworkElement)
				.GetMethod(
					"RemoveLogicalChild",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
					[typeof(object)]
				);

			if (removeLogicalChild is null)
				return;

			Dispatcher?.Invoke(() => removeLogicalChild?.Invoke(element, [this]),
				System.Windows.Threading.DispatcherPriority.ApplicationIdle);
		}
	}
}
