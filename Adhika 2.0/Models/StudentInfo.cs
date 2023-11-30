using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhika_Final_Build.Models
{
    public class StudentInfo
    {
        public int Id { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }

        public string MName { get; set; }

        public string Email { get; set; }

        public string Lrn { get; set; }

        public bool IsAdmin { get; set; }

        public string Password { get; set; }

        public int Grade { get; set; }

        public ImageSource StudentImage { get; set; }

        public byte[] StudentImageData { get; set; }
    }
}
