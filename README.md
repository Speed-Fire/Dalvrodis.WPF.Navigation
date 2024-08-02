# Dalvsidor.WPF.Navigation
____

This library is intended to simplify navigation in MVVM WPF applications. It's no need to use Frames with its journals or specify DataTemplates in each view, which hosts another views, anymore. The library offers you __ViewModel__ classes and __INavigationService__ interface with its base implementation.

Navigation service is provided in 2 variants:
1. Global navigation as Singleton;
2. Local navigation as Transient.

The library depends on __CommunityToolkit.MVVM__ package.

___

## Getting started

### Setup

1. Install __Dalvsidor.WPF.Navigation__ package;
2. Create static class __Program__ and add __main__ function:
3. Create Dependency Injection container in __Program__:
4. Use ```AddNavigation``` method on DI container.
 
    ```c#
	public static class Program
	{
    	public static void Main()		
		{
            IServiceCollection services = ...; // get it somehow.
        	services.AddNavigation();
		}
	}
	```

### Views and ViewModels

Base ```ViewModel``` class is derived from ```ObservableValidator``` and ```IDisposable```. NavigationService uses view models with specified generic parameter _TView_, which defines view to show, when this view model is active.
```c#
public abstract class ViewModel : 
	ObservableValidator, 
	iDisposable
{
	// some code...

	public virtual void Dispose() {}
}

public abstract class ViewModel<TView> : ViewModel
	where TView : UserControl
{}
```

> NOTE: all view models must be added to DI container.

__UserControls__ are using as views.

Let's create some for example:

Views: _MainView_, _HomeView_, _SettingsView_.

View models:

```c#
public partial class MainVM : ViewModel<MainView>
{
	[RelayCommand]
	private void GoToHome()
	{
		this.Navigation.NavigateTo<HomeVM>();
	}

	[RelayCommand]
	private void GoToSettings()
	{
		this.Navigation.NavigateTo<SettingsVM>();
	}
}

public partial class HomeVM : ViewModel<HomeView> 
{
	[RelayCommand]
	private void GoToMain()
	{
		this.Navigation.NavigateTo<MainVM>();
	}
}

public class SettingsVM : ViewModel<SettingsView> {} 
```

> NOTE: view model contains reference to __Navigation Service__ it was navigated from. Then you can navigate to next view model right from the current view model, and next one will get the same __Navigation Service__ as well.

> NOTE: when you navigate __Navigation Service__ to view model, previous view model, if it is not null, will be disposed.

Add all views and view models to DI in our __Program__ class:
```c#
services
	.AddTransient<MainVM>()
	.AddTransient<MainView>()
	.AddTransient<HomeVM>()
	.AddTransient<HomeView>()
	.AddTransient<SettingsVM>()
	.AddTransient<SettingsView>();
```

> NOTE: it is unnecessary to add views to DI, until they depends on some services from DI. But they must have public parameterless constructors then.

### Global navigation

__Navigation Service__ works in pair with __UserControlFrame__. This frame shows view of current view model. __UserControlFrame__ is __UserControl__, so it can be put in any WPF window or control. But it can't be used directly in XAML, instead it comes from DI:
```c#
public class MainWindow : Window
{
	public MainWindow(
		[FromKeyedService(Misc.NavConsts.SINGLETON_SERVICE)]
		UserControlFrame frame)
	{
		this.Content = frame;
	}
}
```

Next rewrite your ```App``` class like this:
```c#
public class App : Application
{
	private readonly IServiceProvider _provider;

	public App(IServiceProvider provider)
	{
		InitializeComponent();

		_provider = provider;
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		var window = _provider.GetRequiredService<MainWindow>();
		MainWindow = window;

		INavigationService globalNavigation = provider
			.GetRequiredKeyedService<INavigationService>(
				Misc.NavConsts.SINGLETON_SERVICE);

		globalNavigation.NavigateTo<MainVM>();

		MainWindow.Show();
	}
}
```

And remove ```StartupUri``` in your _App.xaml_ file:
```xml
<Application x:Class="Test.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:Test"
             StartupUri="MainWindow.xaml"> -- remove this parameter
			 ...
</Application>
```

Then in _App.xaml_ properties set _Action on compilation_ to _Page_.

Next add __App__ and __MainWindow__ to DI in __Program__ class:
```c#
services
	.AddTransient<App>()
	.AddTransient<MainWindow>();
```

Now everything is prepared, so build service provider, get and start the app:
```c#
var provider = services.BuildServiceProvider();

var app = provider.GetRequiredService<App>();
app.Run();
```

The whole __Program__ class:
```c#
public static class Program
{
    public static void Main()		
	{
        IServiceCollection services = ...; // get it somehow.
        services.AddNavigation();

		services
			.AddTransient<MainVM>()
			.AddTransient<MainView>()
			.AddTransient<HomeVM>()
			.AddTransient<HomeView>()
			.AddTransient<SettingsVM>()
			.AddTransient<SettingsView>();

		services
			.AddTransient<App>()
			.AddTransient<MainWindow>();

		var provider = services.BuildServiceProvider();

		var app = provider.GetRequiredService<App>();
		app.Run();
	}
}
```

### Local navigation

If you need to create nested element with its own navigation, then you need to use __Navigation Service Factory Function__. Provide it to your host view model and create some key to define navigation channel:
```c#
public static class NavigationChannels
{
	public const string SETTINGS_CHANNEL = "SETTINGS_CHANNEL";
}
```
> NOTE: channel key can be of any type.

And use it in your host view model:
```c#
public class SettingsVM : ViewModel<SettingsView>
{
	private readonly INavigationService _localNavigation;

	public SettingsVM(
		Func<object, INavigationService> navigationServiceFactory)
	{
		_localNavigation = navigationServiceFactory
			.Invoke(NavigationChannels.SETTINGS_CHANNEL);
	}

	public override void Dispose()
	{
		_localNavigation.Dispose();
	}
}
```
> NOTE: don't forget to dispose your local navigation service!

Now you need to provide NavigationManager to your view in order to setup local __UserControlFrame__:
```c#
public partial class SettingsView : UserControl
{
	public SettingsView(NavigationManager navigationManager)
	{
		InitializeComponent();

		// the same key as for NavigationService factory
		navigationManager
			.Attach(
				NavigationChannels.SETTINGS_CHANNEL, 
				AttachFrame, 
				DetachFrame);
	}

	private void AttachFrame(UserControlFrame frame)
	{
		frame.SetValue(Grid.ColumnProperty, 2);

		InnerGrid.Children.Add(frame);
	}

	private void DetachFrame(UserControlFrame frame)
	{
		InnerGrid.Children.Remove(frame);
	}
}
```
> NOTE: InnerGrid is defined in __XAML__. You can put __UserControlFrame__ to any supporting children control.

Now you can use local __Navigation Service__ the same way you use global.

### Dialogs

Sometimes you need to start a dialog and do some actions depending on dialog result, when dialog finishes. And my library supports this behavior. Instead of navigating __NavigationService__, you just need to push dialog view model into it:
```c#
public partial class HomeVM : ViewModel<HomeView> 
{
	// previous code...

	[RelayCommand]
	private void StartDialog()
	{
		this.Navigation
			.PushDialog<DialogVM>(DialogFinishCallback);
	}

	private void DialogFinishCallback(DReturnValue value)
	{
		bool? result = value.Result;

		// ... desired behavior
	}
}
```
_DialogFinishCallback_ will be called when __DialogVM__ releases dialog:

```c#
public partial class DialogVM : ViewModel<DialogView>
{
	[RelayCommand]
	private void FinishDialog()
	{
		// the dialog will be finished with false result
		this.Navigation.ReleaseDialog(false);
	}
}
```

> NOTE: when you starts dialog, previous view model will not be disposed and will be saved. When dialog finishes, that view model will be popped instead of creating new one.

> NOTE: you can have as much nested dialogs as you want.

If only bool dialog result is not enough for you, then you can specify what you want to get from dialog:
```c#
public partial class HomeVM : ViewModel<HomeView> 
{
	// previous code...

	[RelayCommand]
	private void StartDialogWithAdditionalReturnValue()
	{
		this.Navigation
			.PushDialog<IntDialogVM, int>(
				IntDialogFinishCallback);
	}

	private void IntDialogFinishCallback(
		DReturnValue<int> value)
	{
		bool? result = value.Result;

		if(result == true)
		{
			int number = value.ReturnValue;

			// some code...
		}
	}
}
```

And dialog view model:
```c#
public partial class DialogVM : ViewModel<DialogView>
{
	[RelayCommand]
	private void FinishDialog()
	{
		// the dialog will be finished with true result
		this.Navigation.ReleaseDialog<int>(true, 45);
	}
}
```

___

## Addition

All navigation and dialog methods of __Navigation Service__ supports navigation to already created view models.
```c#
public partial class HomeVM : ViewModel<HomeView>
{
	[RelayCommand]
	private void GoToMain()
	{
		var mainVm = new MainVM();

		this.Navigation.NavigateTo(mainVm);
	}
}
```