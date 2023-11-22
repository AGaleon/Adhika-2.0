using Adhika_Final_Build.Models;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Adhika_Final_Build.Views;

public partial class LoginPage : ContentPage
{
    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    public LoginPage()
	{
		InitializeComponent();

	}
    private async void btnLogin_Click(object sender, EventArgs e)
    {
        string email = txtEmail.Text; // Assuming txtEmail is the name of your email entry control
        string password = txtPassword.Text; // Assuming txtPassword is the name of your password entry control
        // Authenticate user and retrieve UserData
        (StudentInfo studentInfo, ObservableCollection<StudentUserdata> userData) = AuthenticateAndGetUserData(email, password);

        if (studentInfo != null && userData != null)
        {
            // Authentication successful, do something with the data
            // For example, display a welcome message

            // Access properties like studentInfo.Lrn, studentInfo.Email, userData.Points, etc.

            // Example: Display a welcome message using DisplayAlert
            await DisplayAlert("Welcome", $"Welcome, {studentInfo.FName} {studentInfo.LName}!", "OK");
        }
        else
        {
            // Authentication failed or no matching UserData found
            // Handle the failure, for example, display an error message
            await DisplayAlert("Error", "Invalid email or password. Please try again.", "OK");
        }
    }

    public (StudentInfo, ObservableCollection<StudentUserdata>) AuthenticateAndGetUserData(string email, string password)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Authentication query
            string authQuery = "SELECT * FROM StudentInfo WHERE Email = @Email AND Password = @Password";

            using (MySqlCommand authCmd = new MySqlCommand(authQuery, connection))
            {
                authCmd.Parameters.AddWithValue("@Email", email);
                authCmd.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader authReader = authCmd.ExecuteReader())
                {
                    if (authReader.Read())
                    {
                        // If authentication is successful, retrieve UserData using a new connection
                        string lrn = authReader["Lrn"].ToString();
                        StudentInfo studentInfo = new StudentInfo
                        {
                            LName = authReader["LName"].ToString(),
                            FName = authReader["FName"].ToString(),
                            MName = authReader["MName"].ToString(),
                            Email = authReader["Email"].ToString(),
                            Lrn = lrn,
                            IsAdmin = Convert.ToBoolean(authReader["IsAdmin"]),
                            Password = authReader["Password"].ToString(),
                            Grade = Convert.ToInt32(authReader["Grade"])
                        };

                        // Use a new connection for UserData query
                        using (MySqlConnection userDataConnection = new MySqlConnection(connectionString))
                        {
                            userDataConnection.Open();

                            // UserData query
                            string userDataQuery = "SELECT Lrn, Points, Stories FROM StudentUserdata WHERE Lrn = @Lrn";

                            using (MySqlCommand userDataCmd = new MySqlCommand(userDataQuery, userDataConnection))
                            {
                                userDataCmd.Parameters.AddWithValue("@Lrn", lrn);

                                using (MySqlDataReader userDataReader = userDataCmd.ExecuteReader())
                                {
                                    ObservableCollection<StudentUserdata> userDataCollection = new ObservableCollection<StudentUserdata>();

                                    while (userDataReader.Read())
                                    {
                                        // If UserData is found, construct and add to the ObservableCollection
                                        StudentUserdata userData = new StudentUserdata
                                        {
                                            Lrn = userDataReader["Lrn"].ToString(),
                                            Points = Convert.ToInt32(userDataReader["Points"]),
                                            Stories = userDataReader["Stories"].ToString()
                                        };

                                        userDataCollection.Add(userData);
                                    }

                                    return (studentInfo, userDataCollection);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Return null if authentication fails or no matching UserData is found
        return (null, null);
    }

   
}