using Adhika_Final_Build.Models;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Adhika_2._0.Views;

public partial class LoadingApp : ContentPage
{
   ObservableCollection<UserData> userDatas = new ObservableCollection<UserData>();
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    StudentInfo studentInfo = new StudentInfo();
    ObservableCollection<StudentUserdata> studentUserdatas = new ObservableCollection<StudentUserdata>();

    public LoadingApp(StudentInfo Userinfo, ObservableCollection<StudentUserdata> Userdata)
	{
		InitializeComponent();

        studentInfo = Userinfo;
        studentUserdatas = Userdata;

    }

    public ObservableCollection<Topic> GetTopicsByUserGrade(int userGrade)
    {
        ObservableCollection<Topic> topics = new ObservableCollection<Topic>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Topic WHERE Grade = @UserGrade";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@UserGrade", userGrade);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Topic topic = new Topic
                        {
                            TopicId = Convert.ToInt32(reader["TopicId"]),
                            Grade = Convert.ToInt32(reader["Grade"]),
                            TopicTitle = reader["TopicTitle"].ToString(),
                            TopicDescription = reader["TopicDescription"].ToString()
                            // Add other properties as needed
                        };

                        topics.Add(topic);
                    }
                }
            }
        }

        return topics;
    }

    public (ObservableCollection<Story>, ObservableCollection<Topic>) GetWithLockStatus(ObservableCollection<Topic> userTopics, ObservableCollection<StudentUserdata> userData)
    {
        ObservableCollection<Story> stories = GetStories(userTopics);

        foreach (Story story in stories)
        {
            // Check if the StoryTitle is in Userdata.Stories
            bool isStoryUnlocked = userData.Any(data => data.Stories == story.StoryTitle);

            // Set the IsUnlocked property based on the check
            story.IsUnlocked = isStoryUnlocked;
        }

        // Check if all stories are unlocked
        bool areAllStoriesUnlocked = stories.All(story => story.IsUnlocked);

        // Set IsUnlocked property of related topics based on all stories being unlocked
        foreach (Topic topic in userTopics)
        {
            topic.IsUnlocked = areAllStoriesUnlocked;
        }

        return (stories, userTopics);
    }

    public ObservableCollection<Story> GetStories( ObservableCollection<Topic> userTopics)
    {
        ObservableCollection<Story> stories = new ObservableCollection<Story>();

        if (userTopics.Count > 0)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Story WHERE StoryTopic = @StoryTopic";

                // Assuming userTopics[0] contains the first item of userTopics
                string storyTopic = userTopics[0].TopicTitle;

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StoryTopic", storyTopic);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Story story = new Story
                            {
                                StoryID = Convert.ToInt32(reader["StoryID"]),
                                StoryTitle = reader["StoryTitle"].ToString(),
                                Descriptions = reader["Descriptions"].ToString(),
                                StoryTopic = reader["StoryTopic"].ToString(),
                                StoryReadingUrl = reader["StoryReadingUrl"].ToString(),
                                StoryVideoUrl = reader["StoryVideoUrl"].ToString(),
                                QuizData = reader["QuizData"].ToString()
                                // Add other properties as needed
                            };

                            stories.Add(story);
                        }
                    }
                }
            }
        }

        return stories;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        UserData userData = new UserData();

        userData.Topics = 
           userData.SudentInfomation =
            userData.Stories = 
    }
}