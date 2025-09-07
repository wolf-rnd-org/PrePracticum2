using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class AudioMixModel
    {
        public List<string> InputFiles { get; set; } = new List<string>();
        public string OutputFile { get; set; }
        public int InputsCount => InputFiles?.Count ?? 0;
    }
}
