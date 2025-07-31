using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class BrightnessContrastModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public float Brightness { get; set; }  // ערכים בין -1 ל־1
        public float Contrast { get; set; }    // ערכים בין 0 ל־2
    }
}
