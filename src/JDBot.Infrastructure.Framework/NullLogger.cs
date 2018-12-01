namespace JDBot.Infrastructure.Framework
{
    public class NullLogger : ILogger
    {
        public void Debug(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }
    }
}
