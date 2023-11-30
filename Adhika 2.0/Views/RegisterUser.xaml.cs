
using Adhika.Models;
using Adhika_Final_Build.Models;
using MySqlConnector;

namespace Adhika_2._0.Views;

public partial class RegisterUser 
{
	public RegisterUser()
	{
		InitializeComponent();
        List<string> gradelvl = new List<string>();
        gradelvl.Add("Grade 7");
        gradelvl.Add("Grade 8");
        gradelvl.Add("Grade 9");
        gradelvl.Add("Grade 10");
        pckLevel.ItemsSource = gradelvl;
    }
    private void pckLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        pckLevel.TitleColor = Color.FromRgb(211, 211, 211);
    }

    private async Task txtStudId_TextChangedAsync(object sender, TextChangedEventArgs e)
    {
        txtStudId.PlaceholderColor = Color.FromRgb(211, 211, 211);
        foreach (char c in txtStudId.Text)
        {
            // Check if the character is a letter
            if (char.IsLetter(c))
            {
                await DisplayAlert("Input Error:", "Invalid input: Student Number must contain numbers only", "OK");

                txtStudId.Text = "";
                txtStudId.PlaceholderColor = Color.FromRgb(255, 0, 0);
            }
        }
    }

    private void txtLastname_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtLastname.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void txtFirstname_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtFirstname.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void txtMiddlename_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtMiddlename.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void txtEmailReg_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtEmailReg.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void txtPassReg_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtPassReg.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void showpas_Clicked(object sender, EventArgs e)
    {
        if (txtPassReg.IsPassword)
        {
            txtPassReg.IsPassword = false;
            showpas.Source = "eyeopen.png";
        }
        else
        {
            txtPassReg.IsPassword = true;
            showpas.Source = "eyeclosed.png";
        }
    }

    private void txtConfirmPassReg_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtConfirmPassReg.PlaceholderColor = Color.FromRgb(211, 211, 211);
    }

    private void showpass_Clicked(object sender, EventArgs e)
    {
        if (txtConfirmPassReg.IsPassword)
        {
            txtConfirmPassReg.IsPassword = false;
            showpass.Source = "eyeopen.png";
        }
        else
        {
            txtConfirmPassReg.IsPassword = true;
            showpass.Source = "eyeclosed.png";
        }
    }
    public static List<string> GetEmptyEntries(params Entry[] entries)
    {
        List<string> emptyEntryNames = new List<string>();

        foreach (Entry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Text))
            {
                emptyEntryNames.Add(entry.Placeholder); // Use AutomationId or Name as the identifier.
                entry.PlaceholderColor = Color.FromRgb(255, 0, 0);
            }
        }

        return emptyEntryNames;
    }
    private async void btn_register_Clicked(object sender, EventArgs e)
    {
        List<string> emptyEntries = GetEmptyEntries(
        txtConfirmPassReg,
        txtPassReg,
        txtEmailReg,
        txtFirstname,
        txtLastname,
        txtMiddlename,
        txtStudId
        );
        if (emptyEntries.Count > 0)
        {
            // At least one entry is empty.
            // You can display an error message or perform some action here.
            string Message = "";
            foreach (string emptyEntryName in emptyEntries)
            {
                Message += emptyEntryName + ", ";
            }
            if (pckLevel.SelectedItem == null)
            {
                await DisplayAlert("Please Complete the Form", "Please Select Grade Level and " + Message + " is Empty Registration cant continue Please Try Again", "Ok");
                pckLevel.TitleColor = Color.FromRgb(255, 0, 0);
            }
            else
            {
                await DisplayAlert("Please Complete the Form", Message + " is Empty Registration cant continue Please Try Again", "Ok");
            }


        }
        else
        {
            if (!IsValidEmail(txtEmailReg.Text))
            {
                await DisplayAlert("Invalid Input", " Invalid Email Format Please Try Again", "Ok");
                return;
            }
            if (txtPassReg.Text != txtConfirmPassReg.Text)
            {
                await DisplayAlert("Password Confirmation Error", "Password and Confirmation Password Not Matched ", "Ok");
                return;
            }
            if (pckLevel.SelectedItem == null)
            {
                await DisplayAlert("Please Complete the Form", "Please Select Grade Level", "Ok");
                pckLevel.TitleColor = Color.FromRgb(255, 0, 0);
                return;
            }
            StudentInfo student = new StudentInfo();

            string[] getlvl = pckLevel.SelectedItem.ToString().Split(" ");
            student.Lrn = txtStudId.Text;
            student.Grade = int.Parse(getlvl[1]);
            student.FName = txtFirstname.Text;
            student.LName = txtLastname.Text;
            student.MName = txtMiddlename.Text;
            student.Email = txtEmailReg.Text;
            student.Password = txtPassReg.Text;

            InsertResult result = await InsertAsync(student);

            switch (result)
            {
                case InsertResult.Success:
                    await DisplayAlert("Status", "Registered", "OK");
                    txtConfirmPassReg.Text = "";
                    txtPassReg.Text = "";
                    txtEmailReg.Text = "";
                    txtFirstname.Text = "";
                    txtLastname.Text = "";
                    txtMiddlename.Text = "";
                    txtStudId.Text = "";
                    pckLevel.SelectedItem = null;
                    break;
                case InsertResult.DuplicateStudentId:
                    await DisplayAlert("Failed to insert student. Duplicate studentId.", "", "OK");
                    break;
                case InsertResult.DuplicateEmail:
                    await DisplayAlert("Failed to insert student. Duplicate email", "", "OK");
                    break;
                case InsertResult.DatabaseError:
                    await DisplayAlert("Failed to insert student due to a database error.", "", "OK");
                    break;
            }
        }


    }
    public enum InsertResult
    {
        Success,
        DuplicateStudentId,
        DuplicateEmail,
        DatabaseError
    }
    private async void txtStudId_TextChanged(object sender, TextChangedEventArgs e)
    {
        bool hasletters = false;
        txtStudId.PlaceholderColor = Color.FromRgb(211, 211, 211);
         
            if (txtStudId.Text != "")
            {
                foreach (char c in txtStudId.Text)
                {
                    if (char.IsLetter(c))
                    {
                        hasletters = true;
                    }
                }
            }

    

        if (hasletters)
        {

            await DisplayAlert("Input Error: ", "Student Number must be NUMBER ONLY!", "OK");
            txtStudId.Text = "";
            txtStudId.PlaceholderColor = Color.FromRgb(255, 0, 0);

        }
    }

    string connectionString = "Server=mysql-155140-0.cloudclusters.net;Port=10001;Database=Adhika;Uid=admin;Password=UA6fLM7T;SslMode=None;";
    private bool ExistsInTable(string columnName, object value)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string existsSql = $"SELECT COUNT(*) FROM StudentInfo WHERE {columnName} = @value";

            using (MySqlCommand existsCmd = new MySqlCommand(existsSql, connection))
            {
                existsCmd.Parameters.AddWithValue("@value", value);

                int count = Convert.ToInt32(existsCmd.ExecuteScalar());

                return count > 0;
            }
        }
    }
    public async Task<InsertResult> InsertAsync(StudentInfo students)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Check if the studentId or email already exist in the table
            string checkSql = "SELECT COUNT(*) FROM StudentInfo WHERE Id = @studentId OR email = @email";

            using (MySqlCommand checkCmd = new MySqlCommand(checkSql, connection))
            {
                checkCmd.Parameters.AddWithValue("@studentId", students.Id);
                checkCmd.Parameters.AddWithValue("@email", students.Email);

                int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existingCount > 0)
                {
                    if (existingCount == 2)
                    {
                        return InsertResult.DuplicateStudentId;
                    }
                    else if (existingCount == 1)
                    {
                        if (ExistsInTable("studentId", students.Id))
                        {
                            return InsertResult.DuplicateStudentId;
                        }
                        else
                        {
                            return InsertResult.DuplicateEmail;
                        }
                    }
                }
            }
            // Define the SQL INSERT statement
            string insertSql = "INSERT INTO StudentInfo (LName, FName, MName, Email, Lrn, IsAdmin, Password, Grade, StudentImageData) " +
                    "VALUES (@LName, @FName, @MName, @Email, @Lrn, @IsAdmin, @Password, @Grade, @StudentImageData)";

            using (MySqlCommand cmd = new MySqlCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@LName", students.LName);
                cmd.Parameters.AddWithValue("@FName", students.FName);
                cmd.Parameters.AddWithValue("@MName", students.MName);
                cmd.Parameters.AddWithValue("@Email", students.Email);
                cmd.Parameters.AddWithValue("@Lrn", students.Id); // Assuming StudentId maps to Lrn
                cmd.Parameters.AddWithValue("@IsAdmin", students.IsAdmin); // Assuming IsAdmin is a boolean property
                cmd.Parameters.AddWithValue("@Password", students.Password);
                cmd.Parameters.AddWithValue("@Grade", students.Grade);
                cmd.Parameters.AddWithValue("@StudentImageData", students.StudentImageData);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return InsertResult.Success;
                    }
                    else
                    {
                        return InsertResult.DatabaseError;
                    }
                }
                catch (MySqlException ex)
                {
                    await DisplayAlert("Database Error: " + ex.Message, "", "OK");
                    return InsertResult.DatabaseError;
                }
            }
        }
    }
    static bool IsValidEmail(string email)
    {
        if (email != "")
        {
            if (email.Contains("@"))
            {
                string[] ver;
                ver = email.Split("@");
                if (ver[1].Contains("."))
                {
                    string[] vers = ver[1].Split(".");
                    if (vers[1] != "")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}