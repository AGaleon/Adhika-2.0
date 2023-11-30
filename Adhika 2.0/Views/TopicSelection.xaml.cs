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
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    public TopicSelection(ObservableCollection<Topic> topics , StudentInfo studentInfo ,string Seltopic)
	{
		InitializeComponent();
       _studentInfo = studentInfo;
        for (int i = 0; i < topics.Count; i++)
        {
            if (topics[i].Cleared == false)
            {
                try
                {
                    topics[i + 1].locked = true;
                }
                catch (Exception)
                {

                    
                }
            }
            else
            {
                topics[i + 1].locked = false;
            }
        }
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
           
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _ = MopupService.Instance.PushAsync(new LoadingAnim());
        ObservableCollection<StoryData> getstories = await GetStoriesForStudentAsync(_studentInfo.Lrn, selected);
       
        for (int i = 0; i < getstories.Count; i++)
        {
            getstories[i].ImageStory = await GetImageForStoryAsync(getstories[i].StoryID);
        }
        _ = MopupService.Instance.PopAsync();
        Data?.Invoke(this, getstories);
    }
    public async Task<ImageSource> GetImageForStoryAsync(int storyId)
    {
        using (var connection = new MySqlConnection("Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=AdhikaStoryAssests;Uid=admin;Password=UA6fLM7T;SslMode=None;"))
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

    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync(string lrn, string selectedTopic)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query
                command.CommandText = @"
SELECT
    Topic.TopicTitle,
    Story.StoryID,
    Story.StoryTitle,
    Story.Descriptions,
    Story.StoryTopic,
    Story.StoryReadingUrl,
    Story.StoryVideoUrl,
    Story.QuizData,
    COALESCE(StudentUserdata.Points, 0) AS Points,
    COALESCE(StudentInfo.Lrn, @LRN) AS StudentLRN,
    CASE
        WHEN StudentUserdata.Lrn IS NOT NULL THEN 0
        ELSE 1
    END AS Locked,
    CASE
        WHEN StudentInfo.IsAdmin = 1 THEN 1
        ELSE 0
    END AS IsAdmin
FROM
    Topic
JOIN
    Story ON Topic.TopicTitle = Story.StoryTopic
LEFT JOIN (
    SELECT Stories, MAX(Points) AS Points, Lrn
    FROM StudentUserdata
    GROUP BY Stories, Lrn
) StudentUserdata ON Story.StoryTitle = StudentUserdata.Stories
LEFT JOIN StudentInfo ON StudentUserdata.Lrn = StudentInfo.Lrn
WHERE
    Topic.TopicTitle = @SelectedTopic
    AND (StudentInfo.Lrn = @LRN OR StudentInfo.Lrn IS NULL);
";

                // Add parameters
                command.Parameters.AddWithValue("@LRN", lrn);
                command.Parameters.AddWithValue("@SelectedTopic", selectedTopic);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var stories = new ObservableCollection<StoryData>();
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
                            StudentLRN = reader["StudentLRN"].ToString(),
                            IsLocked = Convert.ToBoolean(reader["Locked"]),
                            isAdminmode = Convert.ToBoolean(reader["IsAdmin"])
                        };
                        if (count == 0)
                        {
                            story.IsLocked = false;
                        }
                       count++;
                        stories.Add(story);
                    }

                    return stories;
                }
            }
        }
    }

}