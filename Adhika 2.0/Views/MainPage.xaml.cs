using Adhika;
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Adhika_Final_Build.Models;

using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;


namespace Adhika_2._0;

public partial class MainPage
{
    public Pagemodel ViewModel { get; }
    bool isTopicLoaded = false;

    ObservableCollection<Topic> preloadedTopic = new ObservableCollection<Topic>();
    StudentInfo _studentInfo = new StudentInfo();
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    StoryData Selectedstory_ = new StoryData();
    private bool isLoadingImages = false;
    private bool isAdmin;

    ObservableCollection<StoryData> storyDatas { get; set; } = new ObservableCollection<StoryData>();
    public MainPage(ObservableCollection<StoryData> storyDatas_ , StudentInfo studentInfo)
	{
		InitializeComponent();
        _studentInfo = studentInfo;
        ViewModel = new Pagemodel();
        BindingContext = ViewModel;
        storyDatas = storyDatas_;
        TopicSelection.Data += changedTopicstory;
        AddTopic.TopicGrade += updatedgrade;
        AddStory.newvalue += updatess;
        header_.Text = storyDatas_[0].TopicTitle;
        isAdmin = storyDatas_[0].isAdminmode;
        ViewModel.storydataItemsSource = storyDatas;
    }

    private void updatess(object sender, StoryData e)
    {
        var a = e;
        a.isAdminmode = isAdmin;
        ViewModel.AddNewItem(a);
    }

    private void updatedgrade(object sender, string e)
    {
        getPreloadedTopics(_studentInfo.Grade.ToString(), _studentInfo.Lrn);
    }

    private void changedTopicstory(object sender, ObservableCollection<StoryData> e)
    {
        header_.Text = e[0].TopicTitle;
        ViewModel.storydataItemsSource = e;
    }

    public async Task<ObservableCollection<Topic>> GetTopicsWithClearedStatusAsync(string grade, string lrn)
    {
        ObservableCollection<Topic> topics = new ObservableCollection<Topic>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
                SELECT
                    Topic.*,
                    TopicAssets.ImageData AS TopicImageData,
                    CASE
                        WHEN (
                            SELECT COUNT(*)
                            FROM StudentUserdata
                            WHERE StudentUserdata.Lrn = @LRN
                                AND StudentUserdata.Topic = Topic.TopicTitle
                                AND StudentUserdata.Passed = 1
                        ) THEN 1
                        ELSE 0
                    END AS Cleared
                FROM
                    Topic
                LEFT JOIN TopicAssets ON Topic.TopicId = TopicAssets.TopicId
                WHERE
                    Topic.Grade = @Grade";

                command.Parameters.AddWithValue("@Grade", grade);
                command.Parameters.AddWithValue("@LRN", lrn);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        byte[] imageData = (byte[])reader["TopicImageData"];
                        Topic topic = new Topic
                        {
                            TopicTitle = reader["TopicTitle"].ToString(),
                            TopicDescription = reader["TopicDescription"].ToString(),
                            Grade = (int)reader["Grade"],
                            Cleared = Convert.ToBoolean(reader["Cleared"]),
                            TopicId = (int)reader["TopicId"],
                            TopicImage = ImageSource.FromStream(() => new MemoryStream(imageData))
                        };

                        topics.Add(topic);
                    }
                }
            }
        }
        isTopicLoaded = true;
        return topics;
    }
    


    private void logout_Tapped(object sender, EventArgs e)
    {
        // Handle the logout event
    }

    // Event handler for the grade level grid tap event
    private void changeGrade(object sender, EventArgs e)
    {
        // Handle the grade level change event
    }

    // Event handler for the topics grid tap event
    private async void changeTopic(object sender, EventArgs e)
    {
        if (isTopicLoaded)
        {
           await MopupService.Instance.PushAsync(new TopicSelection(preloadedTopic, _studentInfo, header_.Text));
        }
        
    }

    // Event handler for the logo button click event
    private async void MainmenuLogo_Clicked(object sender, EventArgs e)
    {
        if (isAdmin)
        {
            _ = MainmenuLogo.ScaleTo(1.3, 500);
            await MopupService.Instance.PushAsync(new Admintools(),true);
        }
    }
    private async void ReadNow_tapped(object sender, EventArgs e)
    {
        if (!Selectedstory_.IsLocked)
        {
            StoryData tounlocked = new StoryData();

            for (int i = 0; i < 10; i++)
            {
                if (ViewModel.storydataItemsSource[i] == Selectedstory_)
                {
                    tounlocked = ViewModel.storydataItemsSource[i + 1];
                    break;
                }
            }
            await MopupService.Instance.PushAsync(new ExplorePop(Selectedstory_, tounlocked));
        }
    }
    
    private void SwipeItemView_InvokedDeleteSory(object sender, EventArgs e)
    {
        // Handle the logo button click event
    }
    // Event handler for the carousel view current item changed event
    private async void carouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
       
        if (storyDatas != null)
        {
            StoryData stories_ = (StoryData)carouselView.CurrentItem;
            try
            {
                Selectedstory_ = stories_;
                bg_item.Source = stories_.ImageStory;
              
            }
            catch (System.Exception)
            {
                bg_item.Source = "";

            }
        }
    }

    [Obsolete]
    private async void PopupPage_Appearing(object sender, EventArgs e)
    {
        getPreloadedTopics(_studentInfo.Grade.ToString(), _studentInfo.Lrn);
    }
    public async void getPreloadedTopics(string grade, string lrn)
    {
        preloadedTopic = await GetTopicsWithClearedStatusAsync(grade, lrn);

    }
 
}