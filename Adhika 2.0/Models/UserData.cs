using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_Final_Build.Models
{
    public class UserData
    {
        public StudentInfo SudentInfomation { get; set; }

        public ObservableCollection<StudentUserdata> StudentDatas { get; set; }

        public ObservableCollection<Topic> Topics { get; set; }

        public ObservableCollection<TopicAssets> TopicsAssets { get; set; }

        public ObservableCollection<Story> Stories { get; set; }

        public ObservableCollection<StoryAssets> StoriesAssets { get; set; }
    }
}
