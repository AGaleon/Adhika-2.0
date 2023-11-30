using Adhika.Models;
using Adhika_2._0.Views;
using Mopups.Services;

namespace Adhika;

public partial class Admintools 
{
    List<ClassTopics> topics;

    public Admintools()
	{
		InitializeComponent();
        
	}

    private void PopupPage_Disappearing(object sender, EventArgs e)
    {
       MessagingCenter.Send<object>(this, "logoreset");
    }
    
    private async void AddtopicClicked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new AddTopic());
    }
    
    private async void RegisterClicked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new RegisterUser());
    }
    private async void AddStoryClicked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new AddStory(null));
    }
}