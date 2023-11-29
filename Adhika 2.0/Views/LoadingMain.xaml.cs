namespace Adhika_2._0.Views;

public partial class LoadingMain 
{
	public LoadingMain()
	{
		InitializeComponent();
	}

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        int i = 0;
        while (true)
        {

            await Task.Delay(100);
            _ = TWO.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = THREE.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = FOUR.ScaleTo(1.3, 300);
            await Task.Delay(100);
            _ = FIVE.ScaleTo(1.3, 300);
            await Task.Delay(100);

            await Task.Delay(100);
            _ = TWO.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = THREE.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = FOUR.ScaleTo(1, 300);
            await Task.Delay(100);
            _ = FIVE.ScaleTo(1, 300);
            i++;
        }
        
    }
}