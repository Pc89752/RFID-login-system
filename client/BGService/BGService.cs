namespace App.WindowsService;

using System.IO.Pipes;
using Newtonsoft.Json;

public sealed class BGService
{
    // TODO: Get the Uri of server
    private readonly static string _serverUrl = Settings.URI + Settings.CloseReport_endpoint;
    // TODO: Get the computer ID
    // Set Global to communicate within sessions
    private const string PIPE_NAME = @"\\.\pipe\Global\LoginSystem_UI";
    // private readonly string exe_path = @"\GitHub\RFID-login-system\LoginSystem\UI\bin\Debug\net7.0-windows\LoginUI.exe";
    private int usageRecordID = -1;
    public int UsageRecordID {get {return usageRecordID;}}

    public async Task reciveReordID()
    {
        // Console.WriteLine("Recieving");
        usageRecordID = await ReceiveDataAsync(PIPE_NAME);
        // Console.WriteLine($"Recieved: {usageRecordID}");
    }

    public async Task returnClosing()
    {
        try
        {
            using(var client = new HttpClient())
            {
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"usageRecordID", Convert.ToString(usageRecordID)}
                };
                string payload = JsonConvert.SerializeObject(dict);

                await client.PostAsync(_serverUrl, new StringContent(payload));
            }
        }
        catch (Exception)
        {
        }

    }

    private async Task<int> ReceiveDataAsync(string pipe_name)
    {
        using (var pipeServer = new NamedPipeServerStream(pipe_name, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
        {
            // Wait for a client to connect asynchronously.
            await pipeServer.WaitForConnectionAsync();

            // When a client connects, continue with further actions.
            int value = await readCodeFromPipe(pipeServer);
            await pipeServer.DisposeAsync();
            return value;
        }
    }

    private async Task<int> readCodeFromPipe(NamedPipeServerStream pipeServer)
    {
        while (true)
        {
            int value = -1;
            try
            {
                byte[] dataArr = new byte[4];
                await pipeServer.ReadAsync(dataArr.AsMemory(0, 4));
                value = BitConverter.ToInt32(dataArr);
            }
            catch (Exception)
            {
                break;
            }
            return value;
        }
        return -1;
    }
    
    [STAThread]
    void Main()
    {
        reciveReordID().Wait();
    }

}
