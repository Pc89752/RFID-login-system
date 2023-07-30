using System.Net.Http;
using Newtonsoft.Json;

namespace LoginUI
{
    public class ServerHandler
    {
        private string _serverUri;
        private string _computerID;
        private enum ReturningCode: int
        {
            CONNECT_FAILED = -1,
            SUCCESS = 0,
            INVALID_USERNAME = 1,
            INVALID_PASSWORD = 2,
            INVALID_INNERCODE = 3,
            INVALID_TOKEN = 4,
            TOKEN_SUCCESS = 5,
            USER_ID_INUSE = 6,
            NO_RESPONOSE,
            INVALID_RESPONSE,
            NO_STATUS_CODE,
            INVALID_STATUS_CODE,
            NO_USAGE_RECORD_ID
        }

        private static readonly Dictionary<ReturningCode, Tuple<Color, string>> ReturningMessage = new Dictionary<ReturningCode, Tuple<Color, string>>()
        {
            // Internet issue
            {ReturningCode.CONNECT_FAILED, new Tuple<Color, string>(Color.Orange, "Connect failed!")},

            // Login success
            {ReturningCode.SUCCESS, new Tuple<Color, string>(Color.Blue, "Success!")},
            {ReturningCode.TOKEN_SUCCESS, new Tuple<Color, string>(Color.Blue, "Success!")},

            // Login failed
            {ReturningCode.INVALID_USERNAME, new Tuple<Color, string>(Color.Red, "Invalid username!")},
            {ReturningCode.INVALID_PASSWORD, new Tuple<Color, string>(Color.Red, "Invalid password!")},
            {ReturningCode.INVALID_INNERCODE, new Tuple<Color, string>(Color.Red, "Invalid inner code!")},
            {ReturningCode.INVALID_TOKEN, new Tuple<Color, string>(Color.Red, "Invalid token!")},

            // Server internal error
            {ReturningCode.NO_RESPONOSE, new Tuple<Color, string>(Color.Orange, "No response!")},
            {ReturningCode.INVALID_RESPONSE, new Tuple<Color, string>(Color.Orange, "Invalid response!")},
            {ReturningCode.NO_STATUS_CODE, new Tuple<Color, string>(Color.Orange, "No status code!")},
            {ReturningCode.INVALID_STATUS_CODE, new Tuple<Color, string>(Color.Orange, "Invalid status code!")},
            {ReturningCode.NO_USAGE_RECORD_ID, new Tuple<Color, string>(Color.Orange, "No usage record ID response!")},
        };

        private static readonly List<ReturningCode> successCodes = new List<ReturningCode>()
        {
            ReturningCode.SUCCESS,
            ReturningCode.TOKEN_SUCCESS,
        };

        public ServerHandler(string serverUri, string computerID)
        {
            _serverUri = serverUri;
            _computerID = computerID;
        }

        private (bool, Color, string) getReturningData(ReturningCode code)
        {
            var vt = ReturningMessage[code];
            return (successCodes.Contains(code), vt.Item1, vt.Item2);
        }

        public async Task<(bool, Color, string)> submitAsync(Dictionary<string, object> payloadDict, string endPoint)
        {
            // Connecting
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
                catch (Exception ex)
                {
                    LoginUI.logger.Error(ex, "Recieving message from server, {StackTrace}", ex.Message);
                    return getReturningData(ReturningCode.CONNECT_FAILED);
                }
            }

            // Handling response
            if(result == null) return getReturningData(ReturningCode.NO_RESPONOSE);
            Dictionary<string, object>? rDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            if(rDict == null) return getReturningData(ReturningCode.INVALID_RESPONSE);

            if(!rDict.ContainsKey("status_code")) return getReturningData(ReturningCode.NO_STATUS_CODE);

            var status_code = Convert.ToInt32(rDict["status_code"]);
            if(!Enum.IsDefined(typeof(ReturningCode), status_code))
                return getReturningData(ReturningCode.INVALID_STATUS_CODE);
            
            ReturningCode returningCode = (ReturningCode)status_code;
            switch(returningCode)
            {
                case ReturningCode.SUCCESS:
                    if(rDict.ContainsKey("usageRecordID"))
                        try
                        {
                            LoginUI.usageRecordID = Convert.ToInt32(rDict["usageRecordID"]);
                        } catch(Exception) {}
                    else return getReturningData(ReturningCode.NO_USAGE_RECORD_ID);
                    break;
                case ReturningCode.TOKEN_SUCCESS:
                    break;
            }
            return getReturningData(returningCode);
        }
    }
}