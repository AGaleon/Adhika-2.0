using Adhika.Models;
using Adhika_2._0;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Adhika;

public partial class AddTopic 
{
    string connectionString = "Server=mysql-156307-0.cloudclusters.net;Port=19890;Database=Adhika;Uid=admin;Password=NymIxFjs;SslMode=None;";
    FileResult imagedata;
   
    string grade;
	public AddTopic()
	{
		InitializeComponent();
        List<string> gradelvl = new List<string>();
        gradelvl.Add("Grade 7");
        gradelvl.Add("Grade 8");
        gradelvl.Add("Grade 9");
        gradelvl.Add("Grade 10");
        pckLevel.ItemsSource = gradelvl;
    }

    public static  event EventHandler<string> TopicGrade;

    private void entTitle_Completed(object sender, EventArgs e)
    {
        entTitle.IsVisible = false;
        lblTitle.IsVisible = true;
        topictitleimage.IsVisible = true;
    }
    private void lblTitleClicked(object sender, TappedEventArgs e)
    {
        entDescriptions.IsVisible = false;
        lblDescription.IsVisible = true;
        topicDescriptionimage.IsVisible = true;
        entTitle.IsVisible = true;
        lblTitle.IsVisible = false;
        topictitleimage.IsVisible = false;
        entTitle.Focus();
    }
    private void topictitleimage_Clicked(object sender, EventArgs e)
    {
        entDescriptions.IsVisible = false;
        lblDescription.IsVisible = true;
        topicDescriptionimage.IsVisible = true;
        entTitle.IsVisible = true;
        lblTitle.IsVisible = false;
        topictitleimage.IsVisible = false;
        entTitle.Focus();
    }

    private void entDescriptions_Completed(object sender, EventArgs e)
    {
        entDescriptions.IsVisible = false;
        lblDescription.IsVisible = true;
        topicDescriptionimage.IsVisible = true;
    }
    private void lblDescriptionClicked(object sender, TappedEventArgs e)
    {
        entTitle.IsVisible = false;
        lblTitle.IsVisible = true;
        topictitleimage.IsVisible = true;
        lblDescription.IsVisible = false;
        entDescriptions.IsVisible = true;
        topicDescriptionimage.IsVisible = false;
        entDescriptions.Focus();
    }

    private void topicDescriptionimage_Clicked(object sender, EventArgs e)
    {
        entTitle.IsVisible = false;
        lblTitle.IsVisible = true;
        topictitleimage.IsVisible = true;
        lblDescription.IsVisible = false;
        entDescriptions.IsVisible = true;
        topicDescriptionimage.IsVisible = false;
        entDescriptions.Focus();
    }

    private void entTitle_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (entTitle.Text!="")
        {
            lblTitle.Text = entTitle.Text;
        }
    }
    private void entDescriptions_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (entDescriptions.Text!="")
        {
            lblDescription .Text = entDescriptions.Text;
        }
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
      
        byte[] img = null;
        using (var memoryStream = new MemoryStream())
        {
            var stream = await imagedata.OpenReadAsync();
            stream.CopyTo(memoryStream);
            img = memoryStream.ToArray();
        }

        switch (pckLevel.SelectedItem)
        {
            case "Grade 7":
                grade = "7";
                break;
            case "Grade 8":
                grade = "8";
                break;
            case "Grade 9":
                grade = "9";
                break;
            case "Grade 10":
                grade = "10";
                break;
            default:
                break;
        }
        if (entDescriptions.Text != null && entTitle.Text != null && imagedata != null && pckLevel.SelectedItem != null|| imagedata!=null)
        {
;
            if (InsertTopicWithAssets(Convert.ToInt32(grade),entTitle.Text,entDescriptions.Text,img))
            {
                string gr = pckLevel.SelectedItem.ToString();
                await DisplayAlert("", "Success", "ok");
                TopicGrade?.Invoke(this, grade);
                await MopupService.Instance.PopAsync();
            }  

        }
        else
        {
            await DisplayAlert("Incomplete input", "Error", "ok");
        }
        
    }
    
    public bool InsertTopicWithAssets(int grade, string topicTitle, string topicDescription, byte[] imageData)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Insert into Topic table
            int topicId;
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "INSERT INTO Topic (Grade, TopicTitle, TopicDescription) VALUES (@Grade, @TopicTitle, @TopicDescription); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@Grade", grade);
                command.Parameters.AddWithValue("@TopicTitle", topicTitle);
                command.Parameters.AddWithValue("@TopicDescription", topicDescription);

                topicId = Convert.ToInt32(command.ExecuteScalar());
            }

            if (topicId > 0)
            {
                // Insert into TopicAssets table
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO TopicAssets (TopicId, ImageData) VALUES (@TopicId, @ImageData);";
                    command.Parameters.AddWithValue("@TopicId", topicId);
                    command.Parameters.AddWithValue("@ImageData", imageData);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Return true if at least one row was affected
                    return rowsAffected > 0;
                }
            }

            // Return false if the topicId is not valid
            return false;
        }
    }



    private async void AddImage_Tapped(object sender, TappedEventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images, // Specify the file types you want to pick (e.g., Images, PDFs, etc.)
            PickerTitle = "Pick a file",
        });
        imagedata = result;
        if (result != null)
        {
            var stream = await result.OpenReadAsync();
            topicImage.Source = ImageSource.FromStream(() => stream);
            topicImage.IsVisible = true;
            tempImage.IsVisible = false;
        }
        else
        {

        }
    }
}