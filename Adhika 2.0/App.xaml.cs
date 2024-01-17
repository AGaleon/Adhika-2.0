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
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App_OnException);
        Application.Current.UserAppTheme = AppTheme.Light;
        MainPage = new Splash();
       
    }
    private void App_OnException(object sender, UnhandledExceptionEventArgs e)
    {
        // Handle the unhandled exception here
        Exception exception = e.ExceptionObject as Exception;
        try
        {
            Updatestatus(Home._studentInfo.Lrn, false);
        }
        catch (Exception)
        {

        }
        MainPage = new Splash();
    }
    protected override void OnStart()
    {
    }
    public static void Updatestatus(string username, bool Activestatus)
    {
        string connectionString = "Server=mysql-161002-0.cloudclusters.net;Port=12808;Database=Adhika;Uid=admin;Password=3dqlDDv9;SslMode=None;";
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string combinedQuery = @"
            UPDATE StudentInfo SET IsActive = @IsActive WHERE Lrn = @username;
            UPDATE StudentInfo SET LoginLog = CURRENT_TIMESTAMP WHERE Lrn = @username;
        ";

                using (MySqlCommand cmd = new MySqlCommand(combinedQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@IsActive", Activestatus);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
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
