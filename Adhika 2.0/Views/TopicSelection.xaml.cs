using Adhika_2._0.Models;
using Adhika_Final_Build.Models;
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
        ObservableCollection<StoryData> getstories = await GetStoriesForStudentAsync(_studentInfo.Lrn, selected);
        Data?.Invoke(this, getstories);

    }
    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync(string lrn ,string SelectedTopic)
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
    END AS IsAdmin,
    StoryAssets.ImageData
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
LEFT JOIN StoryAssets ON Story.StoryID = StoryAssets.StoryID
WHERE
    Topic.TopicTitle = @SelectedTopic
    AND (StudentInfo.Lrn = @LRN OR StudentInfo.Lrn IS NULL);
";

                // Add parameters
                command.Parameters.AddWithValue("@LRN", lrn);
                command.Parameters.AddWithValue("@SelectedTopic", SelectedTopic);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var stories = new ObservableCollection<StoryData>();

                    while (await reader.ReadAsync())
                    {
                        byte[] imageData = null;
                        try
                        {
                             imageData = (byte[])reader["ImageData"];
                        }
                        catch (Exception)
                        {
                            isLoaded = true;
                            return null;
                        }
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
                        if (imageData != null)
                        {
                            story.ImageStory = ImageSource.FromStream(() => new MemoryStream(imageData));
                        }
                        stories.Add(story);
                    }
                   isLoaded = true;
                    return stories;
                }
            }
        }
    }
}