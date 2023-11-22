using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_Final_Build.Models
{
    public class Story
    {
        public int StoryID { get; set; }

        public bool IsUnlocked { get; set; }
        public string StoryTitle { get; set; }

        public string Descriptions { get; set; }

        public string StoryTopic { get; set; }

        public string StoryReadingUrl { get; set; }

        public string StoryVideoUrl { get; set; }

        public string QuizData  { get; set; }
    }
}
