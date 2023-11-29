using Adhika_2._0;
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;

namespace Adhika_Final_Build.Views;

public partial class LoginPage : ContentPage
{
   
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    public LoginPage()
	{
		InitializeComponent();

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
    END AS Locked
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
    Topic.TopicTitle = (
        SELECT TopicTitle
        FROM Topic
        WHERE Topic.Grade = @Grade OR Topic.Grade IS NULL
        LIMIT 1
    )
    AND (StudentInfo.Lrn = @LRN OR StudentInfo.Lrn IS NULL);
";

                // Add parameters
                command.Parameters.AddWithValue("@LRN", lrn);
                command.Parameters.AddWithValue("@Grade", grade);

                using (var reader = await command.ExecuteReaderAsync())
                {
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
                            isAdminmode = isAdmin
                        };

                        stories.Add(story);
                    }
                }
            }
        }

        return stories;
    }


    private async void btnLogin_Click(object sender, EventArgs e)
    {
        StudentInfo studentInfo = AuthenticateUser(txtEmail.Text, txtPassword.Text);
        if (studentInfo != null)
        {
            // Login successful, display welcome message
            await DisplayAlert("Welcome", $"Welcome, {studentInfo.FName} {studentInfo.LName}!", "OK");
            await MopupService.Instance.PushAsync(new LoadingMain());
            var data = await GetStoriesForStudentAsync(studentInfo.Lrn, studentInfo.Grade, studentInfo.IsAdmin);
            for (int i = 0; i < data.Count; i++)
            {
                data[i].ImageStory =await GetImageForStoryAsync(data[i].StoryID);
            }
            await MopupService.Instance.PopAsync();
            await MopupService.Instance.PushAsync(new MainPage(data,studentInfo));
        }
        else
        {
            // Login failed, display error message
            await DisplayAlert("Login Failed", "Invalid email or password", "OK");
        }
    }

    public StudentInfo AuthenticateUser(string email, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query
                command.CommandText = @"
                    SELECT *
                    FROM StudentInfo
                    WHERE Email = @Email AND Password = @Password;
                ";

                // Add parameters
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                using (var reader = command.ExecuteReader())
                {
                    // Check if any rows were returned
                    if (reader.Read())
                    {
                        // Map the data to the StudentInfo object
                        var studentInfo = new StudentInfo
                        {
                            Lrn = reader["Lrn"].ToString(),
                            LName = reader["LName"].ToString(),
                            FName = reader["FName"].ToString(),
                            MName = reader["MName"].ToString(),
                            IsAdmin = reader["IsAdmin"].ToString() == "True",
                            Grade = Convert.ToInt32( reader["Grade"]),
                        };

                        return studentInfo;
                    }
                    else
                    {
                        return null; // Return null if login is not successful
                    }
                }
            }
        }
    }


}