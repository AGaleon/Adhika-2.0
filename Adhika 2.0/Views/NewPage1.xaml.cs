using Adhika_2._0.Models;
using System.Collections.ObjectModel;

namespace Adhika_2._0.Views;

public partial class NewPage1 
{
    public Pagemodel ViewModel { get; }
    public NewPage1(List<StoryData> storyDatas)
	{
		InitializeComponent();
        ViewModel = new Pagemodel();
        BindingContext = ViewModel;
        foreach (StoryData data in storyDatas)
        {
            ViewModel.AddNewItem(data);
        }
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
       await Task.Delay(2000);
      
        
        for (int i = 0; i < ViewModel.storydataItemsSource.Count; i++)
        {
            var story = ViewModel.storydataItemsSource[i];
            ViewModel.DeleteItem(ViewModel.storydataItemsSource[i]);
            ViewModel.InsertItem(i, story);
        }

    }

    private void col_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}