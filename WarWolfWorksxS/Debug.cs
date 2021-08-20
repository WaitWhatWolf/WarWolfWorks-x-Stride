using Stride.Core.Diagnostics;
using System;
using static WarWolfWorksxS.WWWResources;

namespace WarWolfWorksxS
{
    /// <summary>
    /// A logger which debugs information anywhere.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Logs info to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="message">The info message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void Log(object message, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Info(message.ToString(), callerInfo);
        }

        /// <summary>
        /// Logs a warning to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="message">The warning message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void LogWarning(object message, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Warning(message.ToString(), callerInfo);
        }

        /// <summary>
        /// Logs an error to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void LogError(object message, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Error(message.ToString(), callerInfo);
        }

        /// <summary>
        /// Logs a fatal message to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="message">The fatal message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void LogFatal(object message, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Fatal(message.ToString(), callerInfo);
        }

        /// <summary>
        /// Logs info to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void LogException(this Exception exception, string message = STRING_EMPTY, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Error(message, exception, callerInfo);
        }
        
        /// <summary>
        /// Logs info to the <see cref="WarWolfWorksxS"/> logger.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerInfo">Information about the caller.</param>
        public static void LogExceptionFatal(this Exception exception, string message = STRING_EMPTY, CallerInfo callerInfo = null)
        {
            pv_DebugLogger.Fatal(message, exception, callerInfo);
        }

        internal static void Init()
        {
            pv_DebugLogger = GlobalLogger.GetLogger(nameof(WarWolfWorksxS));
        }

        private static Logger pv_DebugLogger;
    }
}
