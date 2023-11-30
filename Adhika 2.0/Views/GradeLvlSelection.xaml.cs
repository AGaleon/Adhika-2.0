using Mopups.Pages;
using Mopups.Services;

namespace Adhika;

public partial class GradeLvlSelection 
{
    string gradedselected;
    public static event EventHandler<string> SelectedG;
    public GradeLvlSelection()
	{
		InitializeComponent();
	}
    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        gradedselected = "7";
        g7s.IsVisible = true;
        g8s.IsVisible = false;
        g9s.IsVisible = false;
        g10s.IsVisible = false; await MopupService.Instance.PopAsync();
        SelectedG?.Invoke(this, gradedselected);

    }

    private async void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        gradedselected = "8";
        g7s.IsVisible = false;
        g8s.IsVisible = true;
        g9s.IsVisible = false;
        g10s.IsVisible = false;
        await MopupService.Instance.PopAsync();
        SelectedG?.Invoke(this, gradedselected);
        
    }

    private async void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
    {
        gradedselected = "9";
        g7s.IsVisible = false;
        g8s.IsVisible = false;
        g9s.IsVisible = true;
        g10s.IsVisible = false;
        
        await MopupService.Instance.PopAsync();
        SelectedG?.Invoke(this, gradedselected);
    }

    private async void TapGestureRecognizer_Tapped_4(object sender, TappedEventArgs e)
    {
        gradedselected = "10";
        g7s.IsVisible = false;
        g8s.IsVisible = false;
        g9s.IsVisible = false;
        g10s.IsVisible = true;
       
        await MopupService.Instance.PopAsync();
        SelectedG?.Invoke(this, gradedselected);
    }

    
    
}