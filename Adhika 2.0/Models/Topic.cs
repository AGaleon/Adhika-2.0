using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_Final_Build.Models
{
    public class Topic
    {
        public int TopicId { get; set; }

        public int Grade { get; set; }

        public string TopicTitle { get; set; }

        public string TopicDescription { get; set; }

        public bool IsUnlocked { get; set; }

        public ImageSource TopicImage { get; set; }
        public bool Cleared { get;  set; }
    }
}
