namespace JDBot.Infrastructure.Framework
{
    public static class Logger
    {
        private static ILogger _logger = new NullLogger();
        private static LogVerbosity _verbosity;

        public static void Initialize(ILogger logger, LogVerbosity verbosity)
        {
            _logger = logger;
            _verbosity = verbosity;
        }

        public static void Debug(string message)
        {
            if(_verbosity == LogVerbosity.Debug)
                _logger.Debug(message);
        }

        public static void Info(string message)
        {
            if (_verbosity <= LogVerbosity.Info)
                _logger.Info(message);
        }

        public static void Warn(string message)
        {
            if (_verbosity <= LogVerbosity.Warn)
                _logger.Warn(message);
        }

        public static void Error(string message)
        {
            if (_verbosity <= LogVerbosity.Error)
                _logger.Error(message);
        }
    }
}
