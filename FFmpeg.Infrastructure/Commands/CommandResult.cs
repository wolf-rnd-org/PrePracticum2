namespace FFmpeg.Infrastructure.Commands
{
    public class CommandResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string CommandExecuted { get; set; }
        public string OutputLog { get; set; }

        public static CommandResult Success()
        {
            return new CommandResult { IsSuccess = true };
        }

        public static CommandResult Failure(string errorMessage)
        {
            return new CommandResult { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
