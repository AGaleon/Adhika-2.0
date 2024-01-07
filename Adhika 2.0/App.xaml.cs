using Adhika;
using Adhika_2._0.Views;
using Adhika_Final_Build.Views;
using MySqlConnector;

namespace Adhika_2._0;

public partial class App : Application
{
	public App()
	{
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx0QXxbf1xzZFZMZV1bRndPMyBoS35RdURiW3tedHVSRWNYUUV+");
        InitializeComponent();
		MainPage = new Splash();
	}
    protected override void OnStart()
    {

    }
    public static void Updatestatus(string username, bool Activestatus)
    {
        string connectionString = "Server=mysql-159972-0.cloudclusters.net;Port=10008;Database=Adhika;Uid=admin;Password=lZknW95N;SslMode=None;";
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE StudentInfo SET IsActive = @IsActive WHERE Lrn = @username", connection);
            cmd.Parameters.AddWithValue("@IsActive", Activestatus);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    protected override void OnSleep()
    {
        try
        {
            Updatestatus(Home._studentInfo.Lrn, false);
        }
        catch (Exception)
        {

        }
    }

   

    protected override void OnResume()
    {
        try
        {
            Updatestatus(Home._studentInfo.Lrn, true);
        }
        catch (Exception)
        {


        }
    }
}
