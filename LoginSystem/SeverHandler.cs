using System.Net.Http;
using Newtonsoft.Json;

namespace LoginSystem
{
    public class ServerHandler
    {
        private string _serverUri;
        public ServerHandler(string serverUri)
        {
            _serverUri = serverUri;
        }

        // post login request
        public async Task<Dictionary<string, object>> submit(Dictionary<string, object> payloadDict, string endPoint)
        {
            Dictionary<string, object> connectFailedDict = new Dictionary<string, object>()
            {
                {"status_code", -1}
            };
            string? result = null;
            string serverUrl = _serverUri + endPoint;
            string payload = JsonConvert.SerializeObject(payloadDict);
            using(var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(serverUrl, new StringContent(payload));
                    if(response!=null) result = await response.Content.ReadAsStringAsync();
                }
                catch (System.Exception ex)
                {
                    Log.LogError(ex.Message);
                    return connectFailedDict;
                }

                if(result == null) return connectFailedDict;
                Dictionary<string, object>? rDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                if(rDict != null) return rDict;
            }
            return connectFailedDict;
        }
    }
}