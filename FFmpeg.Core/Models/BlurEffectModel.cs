using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class BlurEffectModel
    {
        [Required]
        public string VideoName { get; set; } = string.Empty;

        [Range(0.1, 100)]
        public double Sigma { get; set; } = 10;

        [Required]
        public string OutputName { get; set; } = string.Empty;
    }
}
