using NLog;
using Prism.Logging;

namespace ENBManager.Logging.Services
{
    public class PrismLogger : ILoggerFacade
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region ILoggerFacade Implementation

        public void Log(string message, Category category, Priority priority)
        {
            switch(category)
            {
                case Category.Debug:
                    _logger.Debug(message);
                    break;
                case Category.Exception:
                    _logger.Error(message);
                    break;
                case Category.Info:
                    _logger.Info(message);
                    break;
                case Category.Warn:
                    _logger.Warn(message);
                    break;
            }
        }

        #endregion
    }
}
