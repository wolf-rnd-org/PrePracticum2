using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command
{
    public interface ILogger
    {
        void LogInformation(string message);
        void LogDebug(string message);
        void LogWarning(Exception ex, string message);
        void LogError(Exception ex, string message);
    }

    public class Logger : ILogger
    {
        public void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(Exception ex, string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(Exception ex, string message)
        {
            Console.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            Console.WriteLine(message);
        }
    }
}
