using Adhika;
using Adhika_2._0.Views;
using Adhika_Final_Build.Views;

namespace Adhika_2._0;

public partial class App : Application
{
	public App()
	{
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx0QXxbf1xzZFZMZV1bRndPMyBoS35RdURiW3tedHVSRWNYUUV+");
        InitializeComponent();
		MainPage = new Splash();
	}
}
