using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
<<<<<<< HEAD
<<<<<<< HEAD
    public class BlurEffectModel
=======
      public class BlurEffectModel
>>>>>>> 8123c5ffbc441a5304c1804bfbcc1d11e2a7ac1d
=======
    public class BlurEffectModel
>>>>>>> a0db67b507baf769d9f2be5f8116ff8947a0549d
    {
        [Required]
        public string VideoName { get; set; } = string.Empty;

        [Range(0.1, 100)]
        public double Sigma { get; set; } = 10;

        [Required]
        public string OutputName { get; set; } = string.Empty;
    }
}
