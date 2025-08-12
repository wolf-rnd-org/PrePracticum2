using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FFmpeg.Core.Models
{
    public class AnimatedTextModel
    {
        public string VideoPath { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public int FontSize { get; set; }
        public string OutputPath { get; set; }
    }
}
