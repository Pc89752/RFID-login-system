using System.IO;

namespace LoginSystem
{
    class Log
    {
        public static void log(string logLevel, string message, Exception? exception, string? additionalData)
        // public static void LogError(string errorMessage)
        {
            string dateString = DateTime.Now.ToString("YYYY-MM-DD");
            string timeString = DateTime.Now.ToString("HH:mm-:s");
            string logFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/LoginSystem/error/";
            string logFilePath = $"{logFolder}/{dateString}.txt";

            Directory.CreateDirectory(logFolder);

            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    // Build the log entry with timestamp, log level, message, and additional data
                    string logEntry = $"{timeString} [{logLevel}] - {message}";
                    if (!string.IsNullOrEmpty(additionalData))
                    {
                        logEntry += $" - Additional Data: {additionalData}";
                    }

                    // If an exception is provided, append its details to the log entry
                    if (exception != null)
                    {
                        logEntry += $" - Exception: {exception.ToString()}";
                    }
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while logging: {ex.Message}");
            }
        }
    }
}