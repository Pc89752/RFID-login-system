using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoginUI
{
    public class Settings
    {
        // TODO: set path of UI_settings.json in document folder
        private const string SETTINGS_PATH = @"/LoginSystem/settings.json";
        // private const string SETTINGS_PATH = @"/settings.json";
        public static readonly string ComputerName;
        public static readonly string URI;
        public static readonly string RFIDReader_endpoint;
        public static readonly string LoginForm_endpoint;
        public static readonly string DevPass_endpoint;
        public static readonly string NFcCode_path;

        public static readonly string CloseReport_endpoint;

        static Settings()
        {
            string full_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SETTINGS_PATH;
            // Console.WriteLine(full_path);
            // Console.WriteLine(File.Exists(full_path));
            // Environment.Exit(0);
            using (StreamReader sr = new StreamReader(full_path))
            {
                string jsonStr = sr.ReadToEnd();
                if (JsonConvert.DeserializeObject(jsonStr) is not JObject data)
                {
                    throw new Exception("Failed to deserialize JSON into JObject.");
                }
                ComputerName = GetValue<string>(data, "ComputerName")!;
                URI = GetValue<string>(data, "URL.URI")!;
                RFIDReader_endpoint = GetValue<string>(data, "URL.endpoint.RFIDReader")!;
                LoginForm_endpoint = GetValue<string>(data, "URL.endpoint.LoginForm")!;
                DevPass_endpoint = GetValue<string>(data, "URL.endpoint.DevPass")!;
                CloseReport_endpoint = GetValue<string>(data, "URL.endpoint.CloseReport")!;
                NFcCode_path = GetValue<string>(data, "NFcCode_path")!;
            }
        }

        private static T? GetValue<T>(JObject data, string keyPath)
        {
            JToken? value = data;
            string[] keys = keyPath.Split(".");

            foreach (string key in keys)
            {
                if (value is not JObject obj || !obj.TryGetValue(key, out value))
                {
                    // Optionally throw an exception if any key is required
                    throw new KeyNotFoundException($"Key '{keyPath}' not found in the dictionary.");
                }
            }

            try
            {
                return value.ToObject<T>();
            }
            catch (JsonReaderException ex)
            {
                throw new Exception($"Failed to convert value for key '{string.Join(".", keys)}' to type '{typeof(T).Name}'.", ex);
            }
        }
    }
}