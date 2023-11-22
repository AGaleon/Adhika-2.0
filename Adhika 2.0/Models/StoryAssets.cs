﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_Final_Build.Models
{
    public class StoryAssets
    {
        public int StoryID { get; set; }

        public byte[] ImageData { get; set; }

        public ImageSource Image { get; set; }
    }
}
