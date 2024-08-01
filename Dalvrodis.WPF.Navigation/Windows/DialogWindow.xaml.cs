using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Synergy.WPF.Navigation.Windows
{
	/// <summary>
	/// Логика взаимодействия для DialogWindow.xaml
	/// </summary>
	public partial class DialogWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private object? _dialogContent;
		public object? DialogContent
		{
			get => _dialogContent;
			set
			{
				if (_dialogContent == value)
					return;

				_dialogContent = value;
				OnPropertyChanged();
			}
		}

		private readonly bool _scaleAdjustment;

		public DialogWindow(bool scaleAdjustment)
		{
			InitializeComponent();

			_scaleAdjustment = scaleAdjustment;

			Loaded += DialogWindow_Loaded;
		}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (!_scaleAdjustment)
				return;

			Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
			ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11 * 1.25, 1 / m.M22 * 1.25);
			if (dpiTransform.CanFreeze)
				dpiTransform.Freeze();
			this.LayoutTransform = dpiTransform;
		}

		private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
