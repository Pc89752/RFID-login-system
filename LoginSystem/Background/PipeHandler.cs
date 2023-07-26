using System.IO.Pipes;
using Serilog;

namespace BGService
{
    class PipeHandler
    {
        public static int ReceiveDataAsync(string pipe_name)
        {
            var pipeServer = new NamedPipeServerStream(pipe_name, PipeDirection.In);
            pipeServer.WaitForConnectionAsync().Wait();
            var reader = new StreamReader(pipeServer);
            while (true)
            {
                // Read data from the named pipe asynchronously
                int value = -1;
                try
                {
                    value = pipeServer.ReadByte();
                    pipeServer.Disconnect();
                }
                catch (Exception ex)
                {
                    BGService.logger.Error("An error occured during reading value from pipe", ex.ToString());
                    Thread.CurrentThread.Interrupt();
                }
                return value;
            }
        }
    }
}