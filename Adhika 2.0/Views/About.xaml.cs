using Mopups.Services;

namespace Adhika_2._0.Views;

public partial class About 
{
	public About()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await MopupService.Instance.PopAsync();
    }
}