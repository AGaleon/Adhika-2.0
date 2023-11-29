namespace Adhika_2._0.Views;

public partial class LoadingAnim 
{
	public LoadingAnim()
	{
		InitializeComponent();
        
    }

    private async    void PopupPage_Appearing(object sender, EventArgs e)
    {
        while (true)
        {
            await load.RelRotateTo(360, 800);
        }
    }
}