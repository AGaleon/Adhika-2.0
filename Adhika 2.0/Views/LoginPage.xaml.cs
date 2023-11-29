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
    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync(string lrn, int grade, bool isAdmin)
    {
        DataTable dt = new DataTable();

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
                    dt.Load(reader);
                }
            }
        }

        var stories = new ObservableCollection<StoryData>();

        foreach (DataRow row in dt.Rows)
        {
            
           

            var story = new StoryData
            {
                TopicTitle = row["TopicTitle"].ToString(),
                StoryID = Convert.ToInt32(row["StoryID"]),
                StoryTitle = row["StoryTitle"].ToString(),
                Descriptions = row["Descriptions"].ToString(),
                StoryTopic = row["StoryTopic"].ToString(),
                StoryReadingUrl = row["StoryReadingUrl"].ToString(),
                StoryVideoUrl = row["StoryVideoUrl"].ToString(),
                QuizData = row["QuizData"].ToString(),
                Points = Convert.ToInt32(row["Points"]),
                StudentLRN = row["StudentLRN"].ToString(),
                IsLocked = Convert.ToBoolean(row["Locked"]),
                isAdminmode = isAdmin
            };

           

            stories.Add(story);
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
            var data = await GetStoriesForStudentAsync(studentInfo.Lrn, studentInfo.Grade, studentInfo.IsAdmin);
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