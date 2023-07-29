namespace App.WindowsService;

using System.IO.Pipes;

public sealed class BGService
{
    // TODO: Get the Uri of server
    private const string _serverUrl = @"http://127.0.0.1:5000/closeReport/";
    // TODO: Get the computer ID
    private const string _computerID = @"MyComputer";
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

    public async void returnClosing()
    {
        try
        {
            using(var client = new HttpClient())
            {
                await client.PostAsync(_serverUrl, new StringContent(Convert.ToString(usageRecordID)));
            }
        }
        // catch (Exception ex)
        catch (Exception)
        {
            // logger.Error(ex, "An exception occurred during returning close report", ex.ToString());
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
                await pipeServer.ReadAsync(dataArr, 0 , 4);
                value = BitConverter.ToInt32(dataArr);
            }
            // catch (Exception ex)
            catch (Exception)
            {
                // Program._logger.Error(ex, "An error occured during reading value from pipe", ex.Message);
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
