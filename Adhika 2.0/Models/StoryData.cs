using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_2._0.Models
{
    public class StoryData
    {//a
        public string TopicTitle { get; set; }
        public int StoryID { get; set; }
        public string StoryTitle { get; set; }
        public string Descriptions { get; set; }
        public string StoryTopic { get; set; }
        public string StoryReadingUrl { get; set; }
        public string StoryVideoUrl { get; set; }
        public string QuizData { get; set; }
        public int Points { get; set; }
        public bool IsLocked { get; set; }

        public bool isAdminmode { get; set; }

        public ImageSource ImageStory { get; set; }
        public string StudentLRN { get; internal set; }
    }
}
