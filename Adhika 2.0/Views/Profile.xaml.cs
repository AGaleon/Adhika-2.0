
using MySqlConnector;
using System.Data;

namespace Adhika_2._0;

public partial class Profile 
{
    ImageSource pfp_ = null;
    byte[] picData { get; set; }
    string userid;
	public Profile( string userID, byte[] pfps , string fullname)
	{
		InitializeComponent();
        userid = userID;
        Name.Text = fullname;
        picData = pfps;
        pfp.Source = ImageSource.FromStream(() => new MemoryStream(picData));
        pfp_ = pfp.Source;
    }
    public static event EventHandler<byte[]> newpic;
    private void Changepfpbtn_Clicked(object sender, EventArgs e)
    {
        Mainv.IsVisible = false;
        ChangeProfile.IsVisible = true;
    }
    byte[] fileBytes;
    private async void pickImage_Clicked(object sender, EventArgs e)
    {
        if (pickImage.Text == "Save")
        {
            UpdateStudentImageData(userid, fileBytes);
            Mainv.IsVisible = true;
            ChangeProfile.IsVisible = false;
            Checkpass.IsVisible = false;
            ChangePass.IsVisible = false;
            Next.Text = "Next";
            pickImage.Text = "Pick Profile Picture";
            Pass.Placeholder = "Input New Password";
            CurrentPass.Text = "";
            Pass.Text = "";
            newpic?.Invoke(this, fileBytes);
        }
        else
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images, // Specify the file types you want to allow
                    PickerTitle = "Select a file"
                });

                if (result != null)
                {
                    // Handle the selected file
                    var selectedFilePath = result.FullPath;
                    fileBytes = await File.ReadAllBytesAsync(selectedFilePath);

                    // Convert the byte[] to ImageSource
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(fileBytes));
                    pfp.Source = imageSource;
                    pickImage.Text = "Save";
                }
            }
            catch (Exception ex)
            {
                return;
            }
            
        }
    }
    public void UpdateStudentImageData(string userId, byte[] newImageData)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE StudentInfo SET StudentImageData = @NewImageData WHERE Id = @UserId";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
                command.Parameters.Add("@NewImageData", MySqlDbType.LongBlob).Value = newImageData;

                int rowsAffected = command.ExecuteNonQuery();

                // Display alert based on the result
                if (rowsAffected > 0)
                {
                    // Data updated successfully
                    DisplayAlert("Success", "Student image data updated successfully", "OK");
                }
                else
                {
                    // Failed to update data
                    DisplayAlert("Error", "Failed to update student image data", "OK");
                }
            }
        }
    }

    private void btncancelpic_Clicked(object sender, EventArgs e)
    {
        Mainv.IsVisible = true;
        ChangeProfile.IsVisible = false;
        Checkpass.IsVisible = false;
        ChangePass.IsVisible = false;
        Next.Text = "Next";
        pickImage.Text = "Pick Profile Picture";
        Pass.Placeholder = "Input New Password";
        pfp.Source = pfp_;
        CurrentPass.Text = "";
        Pass.Text = "";
    }
    string connectionString = "Server=mysql-156307-0.cloudclusters.net;Port=19890;Database=Adhika;Uid=admin;Password=NymIxFjs;SslMode=None;";
    private void changpassbtn_Clicked(object sender, EventArgs e)
    {
        Mainv.IsVisible = false;
        Checkpass.IsVisible = true;
    }

    private void currentpassbtn_Clicked(object sender, EventArgs e)
    {
        string currentPassword = CurrentPass.Text;
        if (string.IsNullOrEmpty(currentPassword))
        {
            // Display an alert if the CurrentPass text is empty
            DisplayAlert("Error", "Please enter the current password.", "OK");
            return;
        }
        if (ValidatePassword(userid,currentPassword))
        {
            Checkpass.IsVisible = false;
            ChangePass.IsVisible = true;
        }
        else
        {
            DisplayAlert("Error", "Incorrect current password. Please try again.", "OK");
        }
    }
    public bool ValidatePassword(string inputId, string inputPassword)
    {
        bool isValid = false;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM StudentInfo WHERE Id = @InputId AND Password = @InputPassword";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@InputId", inputId);
                command.Parameters.AddWithValue("@InputPassword", inputPassword);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // A matching record was found, password is valid
                        isValid = true;
                    }
                }
            }
        }

        return isValid;
    }
    public void ChangePassword(string userId, string newPassword)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE StudentInfo SET Password = @NewPassword WHERE Id = @UserId";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.Add("@NewPassword", MySqlDbType.VarChar).Value = newPassword;
                command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Password changed successfully
                    DisplayAlert("Success", "Password changed successfully.", "OK");
                }
                else
                {
                    // Failed to change password
                    DisplayAlert("Error", "Failed to change password. Please try again.", "OK");
                }
            }
        }
    }
    string fpass;
    private async void Next_Clicked(object sender, EventArgs e)
    {
        if (Next.Text=="Next")
        {
            fpass = Pass.Text;
            Pass.Placeholder = "Confirm Password";
            Next.Text = "Save";
            Pass.Text = "";
        }
        else
        {
            if (fpass == Pass.Text)
            {
                ChangePassword(userid, Pass.Text);
                Mainv.IsVisible = true;
                ChangeProfile.IsVisible = false;
                Checkpass.IsVisible = false;
                ChangePass.IsVisible = false;
                Next.Text = "Next";
                pickImage.Text = "Pick Profile Picture";
                Pass.Placeholder = "Input New Password";
                pfp.Source = pfp_;
                CurrentPass.Text = "";
                Pass.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "New password and confirm password do not match. Please try again.", "OK");
                Pass.Placeholder = "Input New Password";
                fpass ="";
                Pass.Text = "";
                Next.Text = "Next";
            }
        }
    }
}