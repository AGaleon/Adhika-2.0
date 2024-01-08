using Adhika;
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;


namespace Adhika_2._0;

public partial class Home
{
    public Pagemodel ViewModel { get; }
    bool isTopicLoaded = false;
    public static string _lrn;
    ObservableCollection<Topic> preloadedTopic = new ObservableCollection<Topic>();
    public static StudentInfo _studentInfo = new StudentInfo();
    string connectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
    StoryData Selectedstory_ = new StoryData();
    private bool isLoadingImages = false;
    private bool isAdmin;
    ObservableCollection<StoryData> storyDatas { get; set; } = new ObservableCollection<StoryData>();
    public Home(ObservableCollection<StoryData> storyDatas_ , StudentInfo studentInfo)
	{
		InitializeComponent();
        _studentInfo = studentInfo;
        ViewModel = new Pagemodel();
        BindingContext = ViewModel;
        storyDatas = storyDatas_;
        Profile.newpic += UpdatePic;
        TopicSelection.DelItem += DeleteTopic;
        GradeLvlSelection.SelectedG += selectedchange;
        TopicSelection.Data += changedTopicstory;
        AddTopic.TopicGrade += updatedgrade;
        AddStory.newvalue += updatess;
        header_.Text = storyDatas_[0].TopicTitle;
        isAdmin = storyDatas_[0].isAdminmode;
        ViewModel.storydataItemsSource = storyDatas;
        Gradesel.IsVisible = isAdmin;
    }

    private void DeleteTopic(object sender, Topic e)
    {
        preloadedTopic.Remove(e);
    }

    private void UpdatePic(object sender, byte[] e)
    {
        _studentInfo.StudentImageData = e;
    }

    private async void selectedchange(object sender, string e)
    {
        isTopicLoaded = false;
        _ = MopupService.Instance.PushAsync( new LoadingAnim());
        var change = await GetStoriesForStudentAsync(_studentInfo.Lrn, int.Parse(e), _studentInfo.IsAdmin);
        if (change.Count != 0)
        {
            for (int i = 0; i < change.Count; i++)
            {
                change[i].ImageStory = await GetImageForStoryAsync(change[i].StoryID);
            }
            ViewModel.storydataItemsSource = change;
            header_.Text = ViewModel.storydataItemsSource[0].TopicTitle;
        }
        else
        {
            header_.Text = "No Available Topic Yet";
            ViewModel.storydataItemsSource = new ObservableCollection<StoryData>();
        }
        getPreloadedTopics(e, _studentInfo.Lrn);
        await  MopupService.Instance.PopAsync();
    }
    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync(string lrn, int grade, bool isAdmin)
    {
        var stories = new ObservableCollection<StoryData>();

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query
                command.CommandText = @"
WITH RankedStories AS (
  SELECT
    S.StoryID,
    T.TopicTitle,
    S.Descriptions,
    S.QuizData,
    S.StoryReadingUrl,
    S.StoryTitle,
    S.StoryTopic,
    S.StoryVideoUrl,
    COALESCE(SU.Points, 0) AS Points,
    CASE WHEN SU.Stories IS NOT NULL THEN true ELSE false END AS Unlocked,
    ROW_NUMBER() OVER (PARTITION BY S.StoryID ORDER BY COALESCE(SU.Points, 0) DESC) AS RowNum
  FROM
    Topic T
    JOIN Story S ON T.TopicTitle = S.StoryTopic
    LEFT JOIN StudentUserdata SU ON S.StoryTitle = SU.Stories AND SU.Lrn = @Lrn
  WHERE
    T.TopicTitle = (SELECT TopicTitle FROM Topic WHERE Grade = @Grade LIMIT 1)
    AND T.Grade = @Grade
)
SELECT
  StoryID,
  TopicTitle,
  Descriptions,
  QuizData,
  StoryReadingUrl,
  StoryTitle,
  StoryTopic,
  StoryVideoUrl,
  Points,
  Unlocked
FROM
  RankedStories
WHERE
  RowNum = 1
";

                // Add parameters
                command.Parameters.AddWithValue("@LRN", lrn);
                //command.Parameters.AddWithValue("@Topic", topic);
                command.Parameters.AddWithValue("@Grade", grade);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    int count = 0;
                    while (await reader.ReadAsync())
                    {
                        var story = new StoryData
                        {
                            TopicTitle = reader["TopicTitle"].ToString(),
                            StoryID = Convert.ToInt32(reader["StoryID"]),
                            StoryTitle = reader["StoryTitle"].ToString(),
                            Descriptions = reader["Descriptions"].ToString(),
                            StoryTopic = reader["StoryTopic"].ToString(),
                            StoryReadingUrl = reader["StoryReadingUrl"].ToString(),
                            StoryVideoUrl = reader["StoryVideoUrl"].ToString(),
                            QuizData = reader["QuizData"].ToString(),
                            Points = Convert.ToInt32(reader["Points"]),
                            IsLocked = !Convert.ToBoolean(reader["Unlocked"]),
                            isAdminmode = isAdmin
                        };
                        if (count == 0)
                        {
                            story.IsLocked = false;
                        }
                        count++;
                        stories.Add(story);

                    }
                }
            }
        }

        return stories;
    }
    public async Task<ImageSource> GetImageForStoryAsync(int storyId)
    {
        using (var connection = new MySqlConnection("Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=AdhikaStoryAssests;Uid=admin;Password=lZknW95N;SslMode=None;"))
        {
            await connection.OpenAsync();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query
                command.CommandText = "SELECT ImageData FROM StoryAssets WHERE StoryId = @StoryId";
                command.Parameters.AddWithValue("@StoryId", storyId);

                // Execute the query
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
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
    


    private async void logout_Tapped(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Logout Confirmation", "Are you sure you want to log out?", "Yes", "No");

        if (result)
        {
            // User clicked "Yes", perform logout
            // You can add your logout logic here, such as clearing authentication tokens, navigating to login page, etc.
            // For demonstration purposes, I'm using the App.Current.MainPage.Navigation.PopToRootAsync() method
            App.Updatestatus(_studentInfo.Lrn, false);
            await MopupService.Instance.PopAsync();
        }
      
    }

    // Event handler for the grade level grid tap event
    private async void changeGrade(object sender, EventArgs e)
    {
      await  MopupService.Instance.PushAsync(new GradeLvlSelection());
    }

    // Event handler for the topics grid tap event
    private async void changeTopic(object sender, EventArgs e)
    {
        if (isTopicLoaded)
        {
           await MopupService.Instance.PushAsync(new TopicSelection(preloadedTopic, _studentInfo, header_.Text));
        }
        
    }

    [Obsolete]
    protected override bool OnBackButtonPressed()
    {
        // Display exit confirmation when the back button is pressed
        Device.BeginInvokeOnMainThread(async () =>
        {
            bool result = await DisplayAlert("Exit Confirmation", "Are you sure you want to exit the app?", "Yes", "No");

            if (result)
            {
                // User clicked "Yes", exit the app
                // Note: Exiting the app might not be allowed on all platforms
                // On some platforms, you might navigate to the main page or minimize the app instead
                // For demonstration purposes, I'm using the App.Current.MainPage.Navigation.PopToRootAsync() method
                await App.Current.MainPage.Navigation.PopToRootAsync();
            }
            // If user clicked "No", do nothing
        });

        // Return true to indicate that the back button press has been handled
        return true;
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
        if (!currentitem.IsLocked)
        {
            StoryData tounlocked = new StoryData();
            StoryData Result = new StoryData();

            for (int i = 0; i < ViewModel.storydataItemsSource.Count; i++)
            {
                if (ViewModel.storydataItemsSource[i].StoryTitle == currentitem.StoryTitle)
                {
                    try
                    {
                        Result = ViewModel.storydataItemsSource[i];
                        tounlocked = ViewModel.storydataItemsSource[i + 1];
                        tounlocked.StudentLRN = _studentInfo.Lrn;
                        Result.StudentLRN = _studentInfo.Lrn;
                    }
                    catch (Exception)
                    {

                    }
                    break;
                }
            }
            await MopupService.Instance.PushAsync(new ExplorePop(currentitem, tounlocked,Result  ,isAdmin));
        }
    }
    
    // Event handler for the carousel view current item changed event
   
    string storyConnectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
    private string storyAssetsConnectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=AdhikaStoryAssests;Uid=admin;Password=lZknW95N;SslMode=None;";

    public bool DeleteStoryWithAssets(int storyId)
    {
        using (MySqlConnection storyConnection = new MySqlConnection(storyConnectionString))
        using (MySqlConnection storyAssetsConnection = new MySqlConnection(storyAssetsConnectionString))
        {
            storyConnection.Open();
            storyAssetsConnection.Open();
            // Delete from StoryAssets table
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = storyAssetsConnection;
                command.CommandText = "DELETE FROM StoryAssets WHERE StoryId = @StoryId;";
                command.Parameters.AddWithValue("@StoryId", storyId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Delete from Story table
                    using (MySqlCommand storyCommand = new MySqlCommand())
                    {
                        storyCommand.Connection = storyConnection;
                        storyCommand.CommandText = "DELETE FROM Story WHERE StoryId = @StoryId;";
                        storyCommand.Parameters.AddWithValue("@StoryId", storyId);

                        int storyRowsAffected = storyCommand.ExecuteNonQuery();

                        // Return true if at least one row was affected
                        return storyRowsAffected > 0;
                    }
                }

                // Return false if no rows were affected in StoryAssets
                return false;
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
    private void SwipeItemView_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItemView swipeItem)
        {
            if (swipeItem.BindingContext is StoryData item)
            {

                if (true)
                {
                    DeleteStoryWithAssets(item.StoryID);
                    ViewModel.DeleteItem(item);
                }

            }
        }
    }
    StoryData currentitem = null;
    private void carouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {

        if (_home.IsVisible && storyDatas != null)
        {
            StoryData stories_ = (StoryData)carouselView.CurrentItem;
            currentitem = stories_;
            try
            {
                bg_item.Source = stories_.ImageStory;
            }
            catch (Exception)
            {
                bg_item.Source = "";

            }
        }
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DeleteStoryWithAssets(currentitem.StoryID);
        ViewModel.DeleteItem(currentitem);
    }

    private async void about_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new About());
    }
    private async void Settings_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Profile(_studentInfo.Id.ToString(),_studentInfo.StudentImageData, _studentInfo.FName+" "+_studentInfo.LName, _studentInfo.Grade.ToString()));
    }

}