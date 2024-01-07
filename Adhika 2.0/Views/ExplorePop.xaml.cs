
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Mopups.Pages;
using Mopups.Services;

namespace Adhika;

public partial class ExplorePop : PopupPage
{
    bool isAdmin = false;
    StoryData classStories;
    StoryData storyUnlock;
    StoryData result;
    public ExplorePop(StoryData stories , StoryData storiestounlock, StoryData _result, bool isadmin )
    {
		InitializeComponent();
        isAdmin = isadmin;
        classStories = stories;
        result = _result;
        storyUnlock = storiestounlock;
	}
    private async  void readnow_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(classStories.StoryReadingUrl));
    }
    private async void Quiz_Clicked(object sender, EventArgs e)
    {
        if (isAdmin)
        {
            await MopupService.Instance.PushAsync(new QuizResponseview(classStories.StoryTitle, classStories.TopicTitle));
        }
        else
        {
            await MopupService.Instance.PushAsync(new Assesment(true, classStories.QuizData, storyUnlock,result));
        }
    }

    private async void Lumi_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(classStories.StoryVideoUrl));
    }
}