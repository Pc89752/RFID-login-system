using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App.WindowsService;
public class Settings
{
    private const string SETTINGS_PATH = @"/LoginSystem/settings.json";
    public static readonly string URI;
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
            URI = GetValue<string>(data, "URL.URI")!;
            CloseReport_endpoint = GetValue<string>(data, "URL.endpoint.CloseReport")!;
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