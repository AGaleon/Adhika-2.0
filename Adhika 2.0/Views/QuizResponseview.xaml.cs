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

        public string  Attemps { get; set; }
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
        string connectionString = "Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"
WITH AttemptCounts AS (
    SELECT
        SU.Lrn,
        COUNT(*) AS Attempts
    FROM
        StudentUserdata SU
    WHERE
        SU.QuizResult = @QuizResult AND SU.Topic = @topic
    GROUP BY
        SU.Lrn
)

, RankedStudentData AS (
    SELECT
        SU.*,
        SI.StudentImageData,
        SI.FName,
        SI.LName,
        SI.MName,
        ROW_NUMBER() OVER (PARTITION BY SU.Lrn ORDER BY SU.Points DESC) AS RowRank
    FROM
        StudentUserdata SU
    JOIN
        StudentInfo SI ON SU.Lrn = SI.Lrn
    WHERE
        SU.QuizResult = @QuizResult AND SU.Topic = @topic
)

SELECT
    RSD.Lrn,
    RSD.StudentImageData,
    RSD.FName,
    RSD.LName,
    RSD.MName,
    RSD.Points,
    RSD.RowRank,
    RSD.Passed,
    AC.Attempts
FROM
    RankedStudentData RSD
JOIN
    AttemptCounts AC ON RSD.Lrn = AC.Lrn
WHERE
    RSD.RowRank = 1
ORDER BY
    RSD.Points DESC, RSD.Lrn";

                command.Parameters.AddWithValue("@QuizResult", story);
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
                            FName = reader["FName"].ToString(),
                            LName = reader["LName"].ToString(),
                            MName = reader["MName"].ToString(),
                            Attemps = reader["Attempts"].ToString()
                        };

                        // Handle StudentImageData conversion
                        if (reader["StudentImageData"] != DBNull.Value)
                        {
                            byte[] imageData = (byte[])reader["StudentImageData"];
                            student._StudentImageData = imageData;
                            student.StudentImageData = ConvertToImageSource(imageData);
                        }
                        else
                        {
                            student.StudentImageData = "logo.png";
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