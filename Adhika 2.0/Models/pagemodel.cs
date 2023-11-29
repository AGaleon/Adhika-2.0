using Adhika_2._0.Views;
using Adhika_Final_Build.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_2._0.Models
{
    public class Pagemodel : INotifyPropertyChanged
    {
        private ObservableCollection<StoryData> storydata;
        public ObservableCollection<StoryData> storydataItemsSource
        {
            get { return storydata; }
            set
            {
                if (storydata != value)
                {
                    storydata = value;
                    OnPropertyChanged(nameof(storydataItemsSource));
                }
            }
        }

        public Pagemodel()
        {
            // Initialize with sample data
            storydataItemsSource = new ObservableCollection<StoryData>();
          
           
        }
        public void EditItem(StoryData editedItem)
        {
            // Find the index of the existing item
            int index = storydataItemsSource.IndexOf(editedItem);

            if (index != -1)
            {
                // Replace the existing item with the edited item
                storydataItemsSource[index] = editedItem;

                // Notify about the change
                OnPropertyChanged(nameof(storydataItemsSource));
            }
        }
        public void DeleteItem(StoryData itemToDelete)
        {
            storydataItemsSource.Remove(itemToDelete);
            OnPropertyChanged(nameof(storydataItemsSource));
        }
     
        public void AddNewItem(StoryData newItem)
        {
            storydataItemsSource.Add(newItem);
        }

        public void InsertItem(int index, StoryData newItem)
        {
            storydataItemsSource.Insert(index, newItem);
            OnPropertyChanged(nameof(storydataItemsSource));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
