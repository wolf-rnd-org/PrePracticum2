using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class RotateVideoModel
    {       
            public string InputPath { get; set; } = string.Empty;
            public string OutputPath { get; set; } = string.Empty;
            public int Angle { get; set; }

    }
}
