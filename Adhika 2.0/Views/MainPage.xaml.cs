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
    ObservableCollection<StoryData> StoryDatas = new ObservableCollection<StoryData>();
    ObservableCollection<Topic> preloadedTopic = new ObservableCollection<Topic>();
    StudentInfo _studentInfo = new StudentInfo();
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    StoryData Selectedstory_ = new StoryData();
    private bool isLoadingImages = false;
    ObservableCollection<StoryData> storyDatas { get; set; } = new ObservableCollection<StoryData>();
    public MainPage(ObservableCollection<StoryData> storyDatas_ , StudentInfo studentInfo)
	{
		InitializeComponent();
        _studentInfo = studentInfo;
        ViewModel = new Pagemodel();
        BindingContext = ViewModel;
        StoryDatas = storyDatas_;
        TopicSelection.Data += changedTopicstory;
        header_.Text = storyDatas_[0].TopicTitle;
        foreach (var item in storyDatas_)
        {
            item.ImageStory = GetImageForStory(item.StoryID);
            ViewModel.AddNewItem(item);
        }
    }

    private void changedTopicstory(object sender, ObservableCollection<StoryData> e)
    {
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
    public ImageSource GetImageForStory(int storyId)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query
                command.CommandText = "SELECT ImageData FROM StoryAssets WHERE StoryId = @StoryId";
                command.Parameters.AddWithValue("@StoryId", storyId);

                // Execute the query
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Check for null value
                        if (reader["ImageData"] != DBNull.Value)
                        {
                            // Retrieve the image data from the database
                            var imageData = (byte[])reader["ImageData"];

                            // Create an ImageSource from the stream
                            return ImageSource.FromStream(() => new MemoryStream(imageData));
                        }
                    }
                }
            }
        }

        // If no image is found, return null or some default image
        return null;
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
    private void changeTopic(object sender, EventArgs e)
    {
        while (!isTopicLoaded)
        {

        }
        MopupService.Instance.PushAsync(new TopicSelection(preloadedTopic,_studentInfo, header_.Text));
    }

    // Event handler for the logo button click event
    private void MainmenuLogo_Clicked(object sender, EventArgs e)
    {
        // Handle the logo button click event
    }
    private async void ReadNow_tapped(object sender, EventArgs e)
    {
        StoryData tounlocked = new StoryData();

       for (int i = 0; i < 10; i++)
        {
            if (ViewModel.storydataItemsSource[i] == Selectedstory_)
            {
              tounlocked =   ViewModel.storydataItemsSource[i+1];
                break;
            }
        }
        await MopupService.Instance.PushAsync(new ExplorePop(Selectedstory_,tounlocked));
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
    async void getPreloadedTopics(string grade, string lrn)
    {
       
        preloadedTopic = await GetTopicsWithClearedStatusAsync(grade, lrn);
       
    }
 
}