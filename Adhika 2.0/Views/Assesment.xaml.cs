using Adhika.Models;
using Adhika_2._0.Models;
using Microsoft.Maui.Layouts;
using Mopups.Services;
using MySqlConnector;
using Newtonsoft.Json;
using System.Data;

namespace Adhika;

public partial class Assesment
{
    int progressb =0;
    bool prevq =false;
    bool quizing_ = false;
    int corrects = 0;
	List<quizItems> quiz=new List<quizItems>();
	int count = 0;
    StoryData unlocked = new StoryData();
    StoryData result = new StoryData();
    public Assesment( bool qz , string jsn , StoryData unlocked_, StoryData _result)
	{
		InitializeComponent();
        result = _result;
        unlocked = unlocked_;
        if (qz)
        {
            quizing_ = qz ;
            quiz = JsonConvert.DeserializeObject<List<quizItems>>(jsn);
           quiz = RandomizeList(quiz);
            quizing(count,prevq);
            count = 1;
        }
        else
        {
            count = 1;
        }
    }
    public static List<quizItems> RandomizeList(List<quizItems> quizItemsList)
    {
        Random random = new Random();

        // Shuffle the entire list
        List<quizItems> randomizedList = quizItemsList.OrderBy(x => random.Next()).ToList();

        // Randomize choices for each quiz item
        foreach (var randomizedQuizItem in randomizedList)
        {
            // Create a list of choices and shuffle them
            List<string> choices = new List<string> { randomizedQuizItem.choice1, randomizedQuizItem.choice2, randomizedQuizItem.choice3, randomizedQuizItem.choice4 };
            choices = choices.OrderBy(x => random.Next()).ToList();

            // Update quiz item with randomized choices
            randomizedQuizItem.choice1 = choices[0];
            randomizedQuizItem.choice2 = choices[1];
            randomizedQuizItem.choice3 = choices[2];
            randomizedQuizItem.choice4 = choices[3];
        }

        return randomizedList;
    }

    private static Random random = new Random();
   
    public static event EventHandler<string> topicquiz;
    public static event EventHandler<bool> ispassed;
    private async void Next_Clicked(object sender, EventArgs e)
    {
        if (quizing_ )
        {
            quizing(count,prevq);
            count++;
            countq.Text = count.ToString();
            progressb = progressb + 30;
            progress.WidthRequest = Convert.ToDouble(progressb);
            CChoice1.IsChecked = false;
            CChoice2.IsChecked = false;
            CChoice3.IsChecked = false;
            CChoice4.IsChecked = false;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(AQuestion.Text) ||
   string.IsNullOrWhiteSpace(AChoice1.Text) ||
   string.IsNullOrWhiteSpace(AChoice2.Text) ||
   string.IsNullOrWhiteSpace(AChoice3.Text) ||
   string.IsNullOrWhiteSpace(AChoice4.Text) ||
   string.IsNullOrWhiteSpace(Aanswer.Text))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }
            else
            {
               
                if (count <= 10)
                {
                    quizItems question = new quizItems
                    {
                        question = AQuestion.Text,
                        choice1 = AChoice1.Text,
                        choice2 = AChoice2.Text,
                        choice3 = AChoice3.Text,
                        choice4 = AChoice4.Text,
                        Answer = Aanswer.Text,
                    };
                    quiz.Add(question);
                    AQuestion.Text = "";
                    AChoice1.Text = "";
                    AChoice2.Text = "";
                    AChoice3.Text = "";
                    AChoice4.Text = "";
                    Aanswer.Text = "";
                    count++;
                    progressb = progressb + 30;
                    progress.WidthRequest=Convert.ToDouble(progressb);
                    countq.Text = count.ToString();
                    if (count == 11)
                    {
                        countq.Text = "10";
                        Next.Text = "Done";
                        string jsonString = JsonConvert.SerializeObject(quiz);
                        bool userResponse = await DisplayAlert("Done", "Do you want to preview questions?", "Yes", "No");
                        if (userResponse)
                        {
                            quizing_ = true;
                            prevq = true;
                            quiz = JsonConvert.DeserializeObject<List<quizItems>>(jsonString);
                            count = 0;
                            quizing(count, prevq);
                            count = 1;
                            topicquiz?.Invoke(this, jsonString);
                        }
                        else
                        {
                            topicquiz?.Invoke(this, jsonString);
                            await MopupService.Instance.PopAsync();
                        }
                    }
                }
                else
                {

                }

            }
           
        }
    }
   async void quizing( int cnt ,bool isprev)
    {
      
        try
        {
            if (CChoice1.IsChecked && Choice1.Text == quiz[count-1].Answer)
            {
                corrects++;
                CChoice1.IsChecked = false;
            }
            if (CChoice2.IsChecked && Choice2.Text == quiz[count-1].Answer)
            {
                corrects++;
                CChoice2.IsChecked = false;
            }
            if (CChoice3.IsChecked && Choice3.Text == quiz[count-1].Answer)
            {
                corrects++;
                CChoice3.IsChecked = false;
            }
            if (CChoice4.IsChecked && Choice4.Text == quiz[count-1].Answer)
            {
                corrects++;
                CChoice4.IsChecked = false;
            }
            Question.Text = quiz[count].question;
            Choice1.Text = quiz[count].choice1;
            Choice2.Text = quiz[count].choice2;
            Choice3.Text = quiz[count].choice3;
            Choice4.Text = quiz[count].choice4;

        }
        catch (Exception)
        {
            count = 9;
            if (isprev)
            {
                await DisplayAlert("Done", "Save", "OK");
               await MopupService.Instance.PopAsync ();
                
            }
            else
            {
                string connectionString = "Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;";
                await DisplayAlert("Done", "Your Score is :" + corrects.ToString(), "OK");
                await MopupService.Instance.PopAsync();
                if (corrects > 6 )
                {
                    ispassed.Invoke(this, true);
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertDataQuery = $@"
INSERT INTO StudentUserdata (Lrn, Points, Passed, Stories, Topic, QuizResult)
VALUES ('{unlocked.StudentLRN}', '{corrects.ToString()}', '1', '{unlocked.StoryTitle}', '{unlocked.TopicTitle}' ,'{ result.StoryTitle }')";

                        using (MySqlCommand command = new MySqlCommand(insertDataQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertDataQuery = $@"
INSERT INTO StudentUserdata (Lrn, Points, Passed, Stories, Topic, QuizResult)
VALUES ('{unlocked.StudentLRN}', '{corrects.ToString()}', '0', '{null}', '{unlocked.TopicTitle}' ,'{result.StoryTitle}')";

                        using (MySqlCommand command = new MySqlCommand(insertDataQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }

            }
        }
        Quizing.IsVisible = true;
        Addquiz.IsVisible = false;
        AQuestion.IsVisible = false;
        Question.IsVisible = true;
        Next.Text = "Next";
      
    }
   
    private void CChoice1_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) // If the checkbox is being checked (not unchecked)
        {
            // Uncheck the other checkboxes in the group
            CChoice2.IsChecked = false;
            CChoice3.IsChecked = false;
            CChoice4.IsChecked = false;
        }
    }

    private void CChoice2_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CChoice1.IsChecked = false;
            CChoice3.IsChecked = false;
            CChoice4.IsChecked = false;
        }
    }

    private void CChoice3_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CChoice1.IsChecked = false;
            CChoice2.IsChecked = false;
            CChoice4.IsChecked = false;
        }
    }

    private void CChoice4_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CChoice1.IsChecked = false;
            CChoice2.IsChecked = false;
            CChoice3.IsChecked = false;
        }
    }

    private void Choice1_tapped(object sender, TappedEventArgs e)
    {
       
        if (CChoice1.IsChecked == true)
        {
            CChoice1.IsChecked = false ;

        }
        else
        {
            CChoice1.IsChecked = true;
        }
    }
    private void Choice2_tapped(object sender, TappedEventArgs e)
    {
       
        if (CChoice2.IsChecked == true)
        {
            CChoice2.IsChecked = false;

        }
        else
        {
            CChoice2.IsChecked = true;
        }
    }
    private void Choice3_tapped(object sender, TappedEventArgs e)
    {
     
        if (CChoice3.IsChecked == true)
        {
            CChoice3.IsChecked = false;

        }
        else
        {
            CChoice3.IsChecked = true;
        }
    }
    private void Choice4_tapped(object sender, TappedEventArgs e)
    {
     
        if (CChoice4.IsChecked == true)
        {
            CChoice4.IsChecked = false;

        }
        else
        {
            CChoice4.IsChecked = true;
        }
    }
}