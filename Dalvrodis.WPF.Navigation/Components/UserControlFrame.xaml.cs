using Dalvrodis.WPF.Navigation.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dalvrodis.WPF.Navigation.Components
{
	/// <summary>
	/// Логика взаимодействия для UserControlFrame.xaml
	/// </summary>
	public partial class UserControlFrame : UserControl
	{
		public UserControlFrame(UserControlFrameVM vm)
		{
			InitializeComponent();
			
			DataContext = vm;
		}
	}
}
