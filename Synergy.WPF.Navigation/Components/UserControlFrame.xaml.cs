using Synergy.WPF.Navigation.ViewModels;
using System.Windows.Controls;

namespace Synergy.WPF.Navigation.Components
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
