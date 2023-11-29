
using Adhika_2._0.Models;
using Mopups.Pages;
using Mopups.Services;

namespace Adhika;

public partial class ExplorePop : PopupPage
{
    StoryData classStories;
    StoryData storyUnlock;
    public ExplorePop(StoryData stories , StoryData storiestounlock)
    {
		InitializeComponent();
        classStories = stories;
        storyUnlock = storiestounlock;
	}
    private async  void readnow_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(classStories.StoryReadingUrl));
    }
    private async void Quiz_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Assesment(true, classStories.QuizData, storyUnlock));
    }

    private async void Lumi_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(classStories.StoryVideoUrl));
    }
}