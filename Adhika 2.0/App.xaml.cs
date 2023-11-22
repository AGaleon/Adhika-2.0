using Adhika_Final_Build.Views;

namespace Adhika_2._0;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new LoginPage();
	}
}
