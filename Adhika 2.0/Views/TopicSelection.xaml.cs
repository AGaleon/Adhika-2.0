using Adhika_2._0.Models;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Adhika_2._0.Views;

public partial class TopicSelection
{
    bool isLoaded = false;
    string selected;
    StudentInfo _studentInfo = new  StudentInfo();
    ObservableCollection<Topic> _topics = new ObservableCollection<Topic>();
    string connectionString = "Server=mysql-156307-0.cloudclusters.net;Port=19890;Database=Adhika;Uid=admin;Password=NymIxFjs;SslMode=None;";
    public TopicSelection(ObservableCollection<Topic> topics , StudentInfo studentInfo ,string Seltopic)
	{
		InitializeComponent();
       _studentInfo = studentInfo;
      
        TopicsList.ItemsSource = topics;
    }
    public static event EventHandler<ObservableCollection<StoryData>> Data;
    private async void PopupPage_Appearing(object sender, EventArgs e)
    {
      
        
    }

    private void TopicsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var sel = (Topic)e.SelectedItem;

        selected = sel.TopicTitle;
        _studentInfo.Grade = sel.Grade;
           
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _ = MopupService.Instance.PushAsync(new LoadingAnim());
        ObservableCollection<StoryData> getstories = await GetStoriesForStudentAsync(selected,  _studentInfo.Lrn, _studentInfo.Grade,_studentInfo.IsAdmin);
        
        if (getstories.Count != 0)
        {
            for (int i = 0; i < getstories.Count; i++)
            {
                getstories[i].ImageStory = await GetImageForStoryAsync(getstories[i].StoryID);
            }
            Data?.Invoke(this, getstories);
        }
        else
        {
            // Display alert when no topics are available
           await DisplayAlert("No Topics", "No topics are available yet.", "OK");
        }
        _ = MopupService.Instance.PopAsync();
      
    }
    public async Task<ImageSource> GetImageForStoryAsync(int storyId)
    {
        using (var connection = new MySqlConnection("Server=mysql-156307-0.cloudclusters.net;Port=19890;Database=AdhikaStoryAssests;Uid=admin;Password=NymIxFjs;SslMode=None;"))
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

    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync( string topic ,string lrn, int grade, bool isAdmin)
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
    T.TopicTitle = @Topic
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
                command.Parameters.AddWithValue("@Topic", topic);
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

}