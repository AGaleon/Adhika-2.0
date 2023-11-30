using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika.Models
{
    //topic_id topic   topic_image topic_description

    public class ClassTopics
    {
        public int topic_id { get; set; }
        public byte[] topic_image { get; set; }
        public ImageSource topicImagesource { get; set; }
        public string topic_description { get; set; }
        public string topic { get; set;}
        public string SelectedIndicator { get; set; }
        public bool Ischecked { get; set; }
    }
}
