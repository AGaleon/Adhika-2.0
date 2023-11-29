using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_2._0.Models
{
    public class Story_ : INotifyPropertyChanged
    {
        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        private string _imageStory;
        public string ImageStory
        {
            get { return _imageStory; }
            set
            {
                if (_imageStory != value)
                {
                    _imageStory = value;
                    OnPropertyChanged(nameof(ImageStory));
                }
            }
        }

        private string _storyTitle;
        public string StoryTitle
        {
            get { return _storyTitle; }
            set
            {
                if (_storyTitle != value)
                {
                    _storyTitle = value;
                    OnPropertyChanged(nameof(StoryTitle));
                }
            }
        }

        private string _descriptions;
        public string Descriptions
        {
            get { return _descriptions; }
            set
            {
                if (_descriptions != value)
                {
                    _descriptions = value;
                    OnPropertyChanged(nameof(Descriptions));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
