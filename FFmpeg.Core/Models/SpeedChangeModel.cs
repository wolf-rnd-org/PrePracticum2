using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class SpeedChangeModel
    {
        public string InputFile { get; set; }
        public double SpeedFactor { get; set; } // מהירות חדשה (למשל, 0.5 להאטה, 2.0 להאצה)
        public string OutputFile { get; set; }
    }
}
