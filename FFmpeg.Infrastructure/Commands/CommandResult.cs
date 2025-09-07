namespace FFmpeg.Infrastructure.Commands
{
    public class CommandResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string CommandExecuted { get; set; }
        public string OutputLog { get; set; }
    }
}
