using System.Net.Http;
using Newtonsoft.Json;

namespace LoginSystem
{
    public class ServerHandler
    {
        private string _serverUri;
        // TODO: check if usageRecordID normal
        public string? usageRecordID;
        private string _computerID;
        public ServerHandler(string serverUri, string computerID)
        {
            _serverUri = serverUri;
            _computerID = computerID;
        }

        public async Task<int> submit(Dictionary<string, object> payloadDict, string endPoint)
        {
            string? result = null;
            string serverUrl = _serverUri + endPoint;
            payloadDict.Add("computerID", _computerID);
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
                    Log.log("ERROR", "Recieving message from server", ex, null);
                    return -1;
                }

                if(result == null) return -1;
                Dictionary<string, object>? rDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                if(rDict == null) return -1;

                int status_code = -1;
                if(rDict.ContainsKey("status_code"))
                    status_code = Convert.ToInt32(rDict["status_code"]);
                else Log.log("ERROR", "Recieving message from server", new Exception("no status_code"), null);
                if(status_code == 0 && rDict.ContainsKey("usageRecordID"))
                    // TODO: check if usageRecordID normal
                    usageRecordID = Convert.ToString(rDict["usageRecordID"]);
                return status_code;
            }
        }
    }
}