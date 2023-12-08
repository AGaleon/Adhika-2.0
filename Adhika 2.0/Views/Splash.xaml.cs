using Adhika_Final_Build.Views;
using Mopups.Services;

namespace Adhika_2._0.Views;

public partial class Splash 
{
	public Splash()
	{
		InitializeComponent();
	}

    private async void PopupPage_Appearing(object sender, EventArgs e)
    {
        int i = 0;
        while (i!=3)
        {

            await Task.Delay(100);
            _ = TWO.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = THREE.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = FOUR.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = FIVE.ScaleTo(1.3, 300);
            await Task.Delay(100);

            await Task.Delay(100);
            _ = TWO.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = THREE.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = FOUR.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = FIVE.ScaleTo(1, 300);
            i++;
        }
        MopupService.Instance.PushAsync(new LoginPage());
    }
}