
using Mopups.Services;
using System;
using System.IO;
using System.Reflection;
using static System.Net.WebRequestMethods;


namespace Adhika;

public partial class Pdfview 
{

    public Pdfview(string url)
	{
		InitializeComponent();

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
    }
    
    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        
    }
  
    private async    void Button_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();

    }

   
}