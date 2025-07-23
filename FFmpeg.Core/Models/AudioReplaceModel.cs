using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class AudioReplaceModel
    {
        public string VideoFile { get; set; }
        public string NewAudioFile { get; set; }
        public string OutputFile { get; set; }
    }
}
