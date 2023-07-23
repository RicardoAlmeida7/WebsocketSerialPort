using System.IO.Ports;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketSerialPort
{
    public class WebMessageHandler : WebSocketBehavior
    {
        private readonly SerialPort _serialPort;

        public WebMessageHandler(SerialPort serialPort)
            => _serialPort = serialPort;

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                string message = e.Data;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();

                    _serialPort.Write(message);
                }
            }
            catch (Exception ex)
            {
                Send(ex.Message);
            }
        }

        protected override void OnClose(CloseEventArgs e)
            => base.OnClose(e);

        protected override void OnOpen()
        {
            Send("Successfully connected.");
            base.OnOpen();
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
            => Send($"Error: {e.Message}.");

        public void SendMessage(string message) => Send(message);
    }
}
