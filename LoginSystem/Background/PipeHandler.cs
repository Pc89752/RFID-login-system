using System.IO.Pipes;
using Serilog;

namespace BGService
{
    class PipeHandler
    {
        public static async Task<int> ReceiveDataAsync(string pipe_name)
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

        private static async Task<int> readCodeFromPipe(NamedPipeServerStream pipeServer)
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
                catch (Exception ex)
                {
                    BGService.logger.Error(ex, "An error occured during reading value from pipe", ex.Message);
                    break;
                }
                return value;
            }
            return -1;
        }
    }
}