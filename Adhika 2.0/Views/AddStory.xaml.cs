using Adhika.Models;
using Adhika_2._0.Models;
using Adhika_2._0.Views;
using Adhika_Final_Build.Models;
using Mopups.Services;
using MySqlConnector;
using Syncfusion.Maui.ListView;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Adhika;

public partial class AddStory 
{
    

    FileResult img= null;

    string pdffilename;
   public static string Type_, Text;
    string quizjs;
    string gradedselected;
    string selectedTopic;
    List<ClassTopics> topics = new List<ClassTopics>();
    
	public AddStory(string lrn)
    {
		InitializeComponent();

        Assesment.topicquiz += topicquiz;
        var maxLength = 90;
        FloatingTextbox.GetText += GetText_;
        FloatingTextbox.DataSent += OnDataSent;
        FloatingTextbox.Type += TextType;
        string a = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
        if (a.Length > maxLength)
        {
            // Truncate the text to the maximum length.
            Details.Text = a.Substring(0, maxLength)+"...";   
        }
    }

    public static event EventHandler<StoryData> newvalue;
    private void OnDataSent(object sender, string Text_)
    {
       Text = Text_;
    }
    private void topicquiz(object sender, string Text_)
    {
        quizjs = Text_;
    }
    private void GetText_(object sender, bool textnotnull)
    {
        if (textnotnull)
        {
            if (Type_ == "Title")
            {
                title_.Text = Text;
            }
            if (Type_ == "Detail")
            {
                Details.Text = Text;
            }
        }
    }
    private void TextType(object sender, string Type)
    {
        Type_ = Type;
    }
    //[Obsolete]
    //private async void btnpickpdffileTapped(object sender, TappedEventArgs e)
    //{
       
    //    try
    //    {
    //        var result = await FilePicker.PickAsync(new PickOptions
    //        {
    //            FileTypes = FilePickerFileType.Pdf, // Specify the file types you want to pick (e.g., Images, PDFs, etc.)
    //            PickerTitle = "Pick a file",
    //        });

    //        if (result != null)
    //        {
    //            var stream = await result.OpenReadAsync();
    //            pdf = stream;
    //            pdffilename = result.FileName;
    //            pdfFilenameLbl.Text = pdffilename;
    //            Stack_fileInfo.IsVisible = true;
    //            pdfcontrols.IsEnabled = false;
    //            pdfcontrols.BackgroundColor = Color.FromHex("#696969");
    //        }
    //        else
    //        {
    //            Stack_fileInfo.IsVisible = false;
    //            pdfcontrols.IsEnabled = true;
    //            pdfcontrols.BackgroundColor = Color.FromHex("#E4082A");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Stack_fileInfo.IsVisible = false;
    //        pdfcontrols.IsEnabled = true;
    //        pdfcontrols.BackgroundColor = Color.FromHex("#E4082A");
    //    }
    //}

    [Obsolete]
  

    private async void btnPreviewPdf_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(PdfUrl.Text, false));
    }

    private async void TitleTap(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new FloatingTextbox("Title"));
    }
   

    private async void cancel_Clicked(object sender, EventArgs e)
    {
        var exit = await this.DisplayAlert("Confirm Cancel", "Do you really want to Cancel", "Yes", "No").ConfigureAwait(false);
        if (exit)
            await MopupService.Instance.PopAsync();
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        gradedselected = "7";
        g7s.IsVisible = true;
        g8s.IsVisible = false;
        g9s.IsVisible = false;
        g10s.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        gradedselected = "8";
        g7s.IsVisible = false;
        g8s.IsVisible = true;
        g9s.IsVisible = false;
        g10s.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
    {
        gradedselected = "9";
        g7s.IsVisible = false;
        g8s.IsVisible = false;
        g9s.IsVisible = true;
        g10s.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped_4(object sender, TappedEventArgs e)
    {
        gradedselected = "10";
        g7s.IsVisible = false;
        g8s.IsVisible = false;
        g9s.IsVisible = false;
        g10s.IsVisible = true;
    }
/// <summary>
/// a
/// </summary>
    int tabindex = 0;
    
    private void back_Clicked(object sender, EventArgs e)
    {
        // Check if we can go back to the previous tab
        if (tabindex >=0)
        {
            tabindex--;
            if (tabindex==0)
            {
                gradeSelection.IsVisible = true;
                topicSelection.IsVisible = false;
                SampleTab.IsVisible = false;
                links.IsVisible = false;
                index.Text = " Select Grade Level";
                Next.IsVisible = true;

            }
            if (tabindex == 1)
            {
                gradeSelection.IsVisible = false;
                topicSelection.IsVisible = true;
                SampleTab.IsVisible = false;
                links.IsVisible = false;
                index.Text = "Choose a topic you want to add a story to :";
                Next.IsVisible =false;
                TopicsList.SelectedItem = null;
            }
            if (tabindex == 2)
            {
                gradeSelection.IsVisible = false;
                topicSelection.IsVisible = false;
                SampleTab.IsVisible = true;
                links.IsVisible = true;
                index.Text = "Sample Story View";
                Next.IsVisible = true;
            }
            Next.Text = "Next";
            back.IsVisible = tabindex > 0;
        }
       
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        if (Next.Text == "Done")
        {
            if (title_.Text == "Sample Title" || img == null || Details.Text.Contains("Lorem") || PdfUrl.Text == null || quizjs ==null) 
            {
              await  DisplayAlert("","Fill All Blah blah","ok");
                return;
            }
            else
            {
                byte[] imageadd = null;
                using (var memoryStream = new MemoryStream())
                {
                    var stream = await img.OpenReadAsync();
                    stream.CopyTo(memoryStream);
                    imageadd = memoryStream.ToArray();
                }

                InsertStoryWithAssets(Details.Text, quizjs, PdfUrl.Text, title_.Text, selectedTopic, LumiUrl.Text, imageadd);
                await DisplayAlert("", "savecomplete", "ok");
                await MopupService.Instance.PopAsync();

            }

        }
        if (tabindex < 2)
        {
            if (tabindex == 0)
            {
                if (gradedselected == null)
                {
                    await DisplayAlert("Note :","PleaseSelect Grade Level","Ok");
                    return;
                }
                Next.IsVisible = true;
            }
            tabindex++;
            if (tabindex==1)
            {
                _ = MopupService.Instance.PushAsync(new LoadingAnim());
                TopicsList.ItemsSource =await GetTopicsWithClearedStatusAsync(gradedselected);
                await MopupService.Instance.PopAsync();
                gradeSelection.IsVisible = false;
                topicSelection.IsVisible = true;
                SampleTab.IsVisible = false;
                index.Text = "Choose a topic you want to add a story to :";
               Next.IsVisible = false;
            }

            back.IsVisible = true;

        }
        if (tabindex == 2)
        {
           
        }  
    }
    bool isTopicLoaded = false;
    string connectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
    public async Task<ObservableCollection<Topic>> GetTopicsWithClearedStatusAsync(string grade)
    {
        ObservableCollection<Topic> topics = new ObservableCollection<Topic>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;

                command.CommandText = @"
SELECT
    Topic.*,
    TopicAssets.ImageData AS TopicImageData
FROM
    Topic
LEFT JOIN TopicAssets ON Topic.TopicId = TopicAssets.TopicId
WHERE
                    Topic.Grade = @Grade";

                command.Parameters.AddWithValue("@Grade", grade);
             

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        byte[] imageData = (byte[])reader["TopicImageData"];
                        Topic topic = new Topic
                        {
                            TopicTitle = reader["TopicTitle"].ToString(),
                            TopicDescription = reader["TopicDescription"].ToString(),
                            Grade = (int)reader["Grade"],
                           
                            TopicId = (int)reader["TopicId"],
                            TopicImage = ImageSource.FromStream(() => new MemoryStream(imageData))
                        };

                        topics.Add(topic);
                    }
                }
            }
        }
        isTopicLoaded = true;
        return topics;
    }

    string storyConnectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
    private string storyAssetsConnectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=AdhikaStoryAssests;Uid=admin;Password=lZknW95N;SslMode=None;";

    public bool InsertStoryWithAssets(string descriptions, string quizData, string storyReadingUrl, string storyTitle, string storyTopic, string storyVideoUrl, byte[] imageData)
    {
        var story = new StoryData();
        story.TopicTitle = storyTopic;
        story.Descriptions = descriptions;
        story.QuizData = quizData;
        story.StoryVideoUrl = storyVideoUrl;
        story.StoryReadingUrl = storyReadingUrl;
        story.StoryTitle = storyTitle;
        story.StoryTopic = storyTopic;
        ImageSource imageSource;

        try
        {
            // Convert byte array to ImageSource
            imageSource = ImageSource.FromStream(() => new MemoryStream(imageData));
        }
        catch (Exception)
        {
            // Handle exception if conversion fails
            imageSource = null;
        }
        story.ImageStory = imageSource;
        newvalue?.Invoke(this, story);
        using (MySqlConnection storyConnection = new MySqlConnection(storyConnectionString))
        using (MySqlConnection storyAssetsConnection = new MySqlConnection(storyAssetsConnectionString))
        {
            storyConnection.Open();
            storyAssetsConnection.Open();

            // Insert into Story table
            int storyId;
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = storyConnection;
                command.CommandText = "INSERT INTO Story (Descriptions, QuizData, StoryReadingUrl, StoryTitle, StoryTopic, StoryVideoUrl) VALUES (@Descriptions, @QuizData, @StoryReadingUrl, @StoryTitle, @StoryTopic, @StoryVideoUrl); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@Descriptions", descriptions);
                command.Parameters.AddWithValue("@QuizData", quizData);
                command.Parameters.AddWithValue("@StoryReadingUrl", storyReadingUrl);
                command.Parameters.AddWithValue("@StoryTitle", storyTitle);
                command.Parameters.AddWithValue("@StoryTopic", storyTopic);
                command.Parameters.AddWithValue("@StoryVideoUrl", storyVideoUrl);

                storyId = Convert.ToInt32(command.ExecuteScalar());
            }

            if (storyId > 0)
            {
                // Insert into StoryAssets table
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = storyAssetsConnection;
                    command.CommandText = "INSERT INTO StoryAssets (StoryId, ImageData) VALUES (@StoryId, @ImageData);";
                    command.Parameters.AddWithValue("@StoryId", storyId);
                    command.Parameters.AddWithValue("@ImageData", imageData);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Return true if at least one row was affected
                    return rowsAffected > 0;
                }
            }

            // Return false if the storyId is not valid
            return false;
        }
    }
    private async void DetailTap(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new FloatingTextbox("Detail"));
    }
   

    private async void AddimageTapped(object sender, TappedEventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images, // Specify the file types you want to pick (e.g., Images, PDFs, etc.)
            PickerTitle = "Pick a file",
        });
        img = result;
        if (result != null)
        {
            var stream = await result.OpenReadAsync();
           
            imageAddicon.IsVisible = false;
            imageStory.Source = ImageSource.FromStream(() => stream);

        }
    }

    private async void TopicsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (Next.Text != "Done" )
        {
           var topic = TopicsList.ItemsSource as ObservableCollection<Topic>;
            selectedTopic = topic[e.SelectedItemIndex].TopicTitle;
            gradeSelection.IsVisible = false;
            topicSelection.IsVisible = false;
            SampleTab.IsVisible = true;
            links.IsVisible = true;
            index.Text = "Sample Story View";
            Next.IsVisible = true;
            Next.Text = "Done";
            tabindex++;
        }
    }

    private void PdfUrl_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (PdfUrl.Text!=null || PdfUrl.Text !="")
        {
            btnPreviewPdf.IsVisible = true;
        }
        else
        {
            btnPreviewPdf.IsVisible = false;
        }
    }

    private async void assignquiz_Clicked(object sender, EventArgs e)
    {
        await  MopupService.Instance.PushAsync(new Assesment(false,null,null,null));
    }

    private async void btnPreviewlumi_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new Pdfview(LumiUrl.Text, false));
    }

    private void LumiUrl_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (LumiUrl.Text != null || LumiUrl.Text != "")
        {
            btnPreviewlumi.IsVisible = true;
        }
        else
        {
            btnPreviewlumi.IsVisible = false;
        }
    }

    [Obsolete]
    protected override bool OnBackButtonPressed()
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            var exit = await this.DisplayAlert("Confirm Cancel", "Do you really want to Cancel", "Yes", "No").ConfigureAwait(false);

            if (exit)
                await MopupService.Instance.PopAsync();
        });
        return true;
    }
}