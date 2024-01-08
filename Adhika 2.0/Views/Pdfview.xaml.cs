
using Mopups.Services;
using System;
using System.IO;
using System.Reflection;
using static System.Net.WebRequestMethods;


namespace Adhika;

public partial class Pdfview 
{
    string urls;
    string downloadlink;
    public Pdfview(string url)
	{
		InitializeComponent();
        downloadlink = url.Replace("https://drive.google.com/file/d/", "https://drive.google.com/uc?export=download&id=");
        if (url.Contains("lumi"))
        {
            webView1.IsVisible = false;
            webView.Source = url;
        }
        else
        {
            webView.IsVisible = false;
            webView1.Source = url;
        }
        urls = url;
    }
    
    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        
    }
  
    private async    void Button_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Launcher.OpenAsync(urls);
    }
}