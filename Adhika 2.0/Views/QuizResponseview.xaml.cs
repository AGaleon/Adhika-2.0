using MySqlConnector;

namespace Adhika_2._0.Views;

public partial class QuizResponseview
{
    public class StudentData
    {
        public string Lrn { get; set; }
        public int Points { get; set; }
        public bool Passed { get; set; }
        public string Stories { get; set; }
        public string Topic { get; set; }
        public int Id { get; set; }
        public byte[] _StudentImageData { get; set; }
        public ImageSource StudentImageData { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
    }
    public QuizResponseview( string story, string topic)
	{
		InitializeComponent();
        Quizname.Text = story+" "+topic;
        Students.ItemsSource = GetStudentsWithImageData(story, topic);
    }
    public List<StudentData> GetStudentsWithImageData(string story, string topic)
    {
        List<StudentData> students = new List<StudentData>();
        string connectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"
                   SELECT
     SU.*,
     SI.StudentImageData,
     SI.FName,
     SI.LName,
     SI.MName
 FROM
     StudentUserdata SU
 JOIN
     StudentInfo SI ON SU.Lrn = SI.Lrn
 WHERE
     SU.Stories = @Story AND SU.Topic = @topic;";

                command.Parameters.AddWithValue("@Story", story);
                command.Parameters.AddWithValue("@topic", topic);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StudentData student = new StudentData
                        {
                            Lrn = reader["Lrn"].ToString(),
                            Points = Convert.ToInt32(reader["Points"]),
                            Passed = Convert.ToBoolean(reader["Passed"]),
                            Stories = reader["Stories"].ToString(),
                            Topic = reader["Topic"].ToString(),
                            Id = Convert.ToInt32(reader["Id"]),
                            FName = reader["FName"].ToString(),
                            LName = reader["LName"].ToString(),
                            MName = reader["MName"].ToString(),
                        };

                        // Handle StudentImageData conversion
                        if (reader["StudentImageData"] != DBNull.Value)
                        {
                            byte[] imageData = (byte[])reader["StudentImageData"];
                            student._StudentImageData = imageData;
                            student.StudentImageData = ConvertToImageSource(imageData);
                        }

                        students.Add(student);
                    }
                }
            }
        }

        return students;
    }

    private ImageSource ConvertToImageSource(byte[] imageData)
    {
        Stream stream = new MemoryStream(imageData);
        return ImageSource.FromStream(() => stream);
    }

 
}