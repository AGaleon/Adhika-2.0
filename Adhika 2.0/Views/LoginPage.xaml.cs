using Adhika_2._0;
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;

namespace Adhika_Final_Build.Views;

public partial class LoginPage 
{
   
    string connectionString = "Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;";
    public string username;
    public LoginPage()
	{
		InitializeComponent();

	}
    public async Task<ImageSource> GetImageForStoryAsync(int storyId)
    {
        using (var connection = new MySqlConnection("Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=AdhikaStoryAssests;Uid=admin;Password=3dqlDDv9;SslMode=None;"))
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

    public async Task<ObservableCollection<StoryData>> GetStoriesForStudentAsync( string lrn, int grade, bool isAdmin)
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
                App.Updatestatus(null, false);
                Application.Current.Quit();
            }
            // If user clicked "No", do nothing
        });

        // Return true to indicate that the back button press has been handled
        return true;
    }
    public bool alreadyloggedin = false;
    private async void btnLogin_Click(object sender, EventArgs e)
    {
        StudentInfo studentInfo = AuthenticateUser(txtEmail.Text, txtPassword.Text);
        if (studentInfo != null)
        {
            // Login successful, display welcome message
            await DisplayAlert("Welcome", $"Welcome, {studentInfo.FName} {studentInfo.LName}!", "OK");
            await MopupService.Instance.PushAsync(new LoadingMain());
            var data = await GetStoriesForStudentAsync( studentInfo.Lrn, studentInfo.Grade, studentInfo.IsAdmin);
            for (int i = 0; i < data.Count; i++)
            {
                data[i].ImageStory =await GetImageForStoryAsync(data[i].StoryID);
            }
            await MopupService.Instance.PopAsync();
            await MopupService.Instance.PushAsync(new Home(data,studentInfo));
            txtEmail.Text = ""; txtPassword.Text = "";
        }
        else
        {
            if (alreadyloggedin)
            {
                await DisplayAlert("Login Failed", "Account is active in other device", "OK");
            }
            else
            {

                // Login failed, display error message
                await DisplayAlert("Login Failed", "Invalid email or password", "OK");
            }
        }
    }

    public StudentInfo AuthenticateUser(string email, string password)
    {
        alreadyloggedin = false;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new MySqlCommand())
            {
                command.Connection = connection;

                // Build your SQL query with an update statement for LoginLog
                command.CommandText = @"
                SELECT *
                FROM StudentInfo
                WHERE Email = @Email AND Password = @Password;
                
                 UPDATE StudentInfo
                SET LoginLog = CASE WHEN IsActive = 0 THEN CURRENT_TIMESTAMP ELSE LoginLog END
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
                        if (reader["IsActive"].ToString() == "True")
                        {
                            alreadyloggedin = true;
                            return null;
                        }

                        byte[] data_;
                        try
                        {
                            data_ = (byte[])reader["StudentImageData"];
                        }
                        catch (Exception)
                        {
                            data_ = null;
                        }

                        // Map the data to the StudentInfo object
                        var studentInfo = new StudentInfo
                        {
                            Id = (int)reader["Id"],
                            Lrn = reader["Lrn"].ToString(),
                            LName = reader["LName"].ToString(),
                            FName = reader["FName"].ToString(),
                            MName = reader["MName"].ToString(),
                            StudentImageData = data_,
                            Email = reader["Email"].ToString(),
                            IsAdmin = reader["IsAdmin"].ToString() == "True",
                            Grade = Convert.ToInt32(reader["Grade"]),
                        };

                        // Update LoginLog with the current timestamp
                        App.Updatestatus(studentInfo.Lrn, true);

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


    private void btnfgtpw_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new ForgotPass());
    }
}