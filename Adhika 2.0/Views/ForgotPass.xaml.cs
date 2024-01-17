using Mopups.Services;
using MySqlConnector;
using System.Net;
using System.Net.Mail;
using static Java.Util.Jar.Attributes;

namespace Adhika_2._0.Views;

public partial class ForgotPass 
{
    bool validemail;
    string otp;
    List<Entry> entryopt = new List<Entry>();
    public ForgotPass()
	{
		InitializeComponent();
        entryopt.Add(otp1);
        entryopt.Add(otp2);
        entryopt.Add(otp3);
        entryopt.Add(otp4);
        otp = GenerateRandomString(4);
        
	}
    public string GetStudentUserDataByEmail(string email)
    {
        using (MySqlConnection connection = new MySqlConnection("Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;"))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT * FROM StudentInfo WHERE Email = @Email", connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        validemail = true;
                        return reader["FName"].ToString() + " " + reader["MName"].ToString() + ". " + reader["LName"].ToString();

                    }
                }
            }
        }

        // If no record is found, display an alert
        DisplayAlert("Error", "No record found for the provided email.", "OK");
        validemail = false;
        // Return null or handle it as needed
        return null;
    }
    public bool sendotp(string to,string from, string pass)
	{
		MailMessage message = new MailMessage();
		message.To.Add(to);
		message.Subject = "Adhika - One-Time Password (OTP) for Verification";
        string user = Name; // Replace with the actual user's name
        string generatedOTP = otp; // Replace with the actual 4-digit OTP
        string companyName = "Adhika"; // Replace with your actual company name

        string otpMessage = $@"
Hello {user},

Your One-Time (OTP) to reset your password is: {generatedOTP}.

Use this code to complete the password reset process. If you didn't request this, please ignore this message.

Thank you,
{companyName}
";
        message.Body = otpMessage;
		message.From = new MailAddress(from);
		SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
		smtpClient.EnableSsl = true;
		smtpClient.Port = 587;
		smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtpClient.Credentials = new NetworkCredential(from, pass);
		try
		{
			smtpClient.Send(message);
			return true;
		}
		catch (Exception ex)
		{
			return false;
		}

	}
    public bool sendnotif(string to, string from, string pass)
    {
        MailMessage message = new MailMessage();
        message.To.Add(to);
        message.Subject = "Adhika - One-Time Password (OTP) for Verification";
        string user = Name  ; // Replace with the actual user's name
        string companyName = "Adhika"; // Replace with your actual company name

        string notificationMessage = $@"
Hello {user},

We wanted to inform you that your password has been successfully changed. If you initiated this change, no further action is required. However, if you did not authorize this update, please contact our support team immediately.

For your security, we recommend regularly updating your password and enabling two-factor authentication.

If you have any questions or concerns, feel free to reach out to us.

Thank you for choosing {companyName}.

Best regards,
{companyName}
";
        message.Body = "Password Change";
        message.From = new MailAddress(from);
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        smtpClient.EnableSsl = true;
        smtpClient.Port = 587;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.Credentials = new NetworkCredential(from, pass);
        try
        {
            smtpClient.Send(message);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }
    static string GenerateRandomString(int length)
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();

        // Use Linq to create a random string by selecting characters randomly from the 'characters' string
        string randomString = new string(Enumerable.Repeat(characters, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());

        return randomString;
    }
    private void OnEntryCompleted(object sender, EventArgs e)
    {
       var entry = (Entry)sender;

    // If the entry is empty (backspace pressed), move to the previous entry
    if (string.IsNullOrEmpty(entry.Text))
    {
        MoveToPreviousEntry(entry);
    }
    else
    {
        // Move focus to the next entry when a character is entered
        MoveToNextEntry(entry);
    }
       
    }
    private void MoveToNextEntry(Entry entry)
    {
        for (int i = 0; i < entryopt.Count; i++)
        {
            if (entryopt[i] == entry)
            {
                if (i + 1 == entryopt.Count)
                {
                    return;
                }
                else
                {
                    int next = i + 1;
                    entryopt[next].Focus();
                    return;
                }
            }
        }
    }

    private void MoveToPreviousEntry(Entry entry)
    {
        for (int i = 0; i < entryopt.Count; i++)
        {
            if (entryopt[i] == entry)
            {
                if (i == 0)
                {
                    return;
                }
                else
                {
                    int previous = i - 1;
                    entryopt[previous].Focus();
                    return;
                }
            }
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        string otpuserinput = "";
        foreach (var item in entryopt)
        {
            otpuserinput += item.Text;
        }
        if (otp == otpuserinput)
        {
            // OTP verification successful
            DisplayAlert("Success", "OTP verification successful. You can now change your password.", "OK");
            Changepass.IsVisible = true;
            otpmode.IsVisible = false;
        }
        else
        {
            // OTP verification failed
            DisplayAlert("Error", "Invalid OTP. Please try again.", "OK");
        }
    }
    string Name;

    public void ChangePassword(string email, string newPassword)
    {
        using (MySqlConnection connection = new MySqlConnection("Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;"))
        {
            try
            {
                connection.Open();

                string query = "UPDATE StudentInfo SET Password = @newPassword WHERE Email = @email";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newPassword", newPassword);
                    command.Parameters.AddWithValue("@email", email);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        bool a = sendnotif(txtemailrecovery.Text, "adhikamobileapp@gmail.com", "jyxg idxz msmv zrlb");
                        Console.WriteLine("Password updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Email not found. Password update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
    private void email_Clicked(object sender, EventArgs e)
    {
        Name = GetStudentUserDataByEmail(txtemailrecovery.Text);
        if (validemail)
        {
            bool a = sendotp(txtemailrecovery.Text, "adhikamobileapp@gmail.com", "jyxg idxz msmv zrlb");
            if (!a)
            {
                DisplayAlert("Error", "Failed to send OTP. Please try again later.", "OK");
            }
            else
            {
                Username.Text = "Hello " + Name + ",";
                otpmode.IsVisible = true;
                recoveryemail.IsVisible = false;
            }
        }
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtConfirmPassword.Text) || string.IsNullOrEmpty(txtNewPassword.Text))
        {
            DisplayAlert("Error", "Please enter both password and confirm password.", "OK");
        }
        else if (txtConfirmPassword.Text != txtNewPassword.Text)
        {
            DisplayAlert("Error", "Passwords do not match. Please try again.", "OK");
        }
        else
        {
            ChangePassword(txtemailrecovery.Text, txtNewPassword.Text);
            DisplayAlert("Success", "Password Change! You can proceed.", "OK");
            MopupService.Instance.PopAsync();
        }
    }
}