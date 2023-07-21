namespace LoginSystem
{
    class Log
    {
        public static void LogError(string errorMessage)
        {
            string dateString = DateTime.Now.ToString("YYYY-MM-DD");
            string timeString = DateTime.Now.ToString("HH:mm-:s");
            string logFilePath = $"error/{dateString}.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    // Add timestamp to the error message
                    string logMessage = $"{timeString}: {errorMessage}";
                    sw.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during logging
                Console.WriteLine($"Failed to log the error: {ex.Message}");
            }
        }
    }
}