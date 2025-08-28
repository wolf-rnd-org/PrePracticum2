using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CutSectionModel
    {
    
            public string InputFilePath { get; set; } = string.Empty;
            public string OutputFilePath { get; set; } = string.Empty;
            public string StartTime { get; set; } = string.Empty;
            public string EndTime { get; set; } = string.Empty;  
    }
}

