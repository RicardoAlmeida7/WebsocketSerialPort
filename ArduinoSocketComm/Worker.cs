using System.IO.Ports;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketSerialPort
{
    public class Worker : BackgroundService
    {
        private readonly WebSocketServer _server;
        private readonly SerialPort _serialPort;
        private readonly WebMessageHandler _webMessageHandler;

        public Worker(IConfiguration configuration)
        {
            string portName = configuration.GetSection("Port").Value;
            int baudRate = int.Parse(configuration.GetSection("BaudRate").Value);

            _serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            int websocketPort = int.Parse(configuration.GetSection("WebsocketPort").Value);

            _webMessageHandler = new WebMessageHandler(_serialPort);

            _server = new(websocketPort);
            _server.AddWebSocketService("/serial", () => _webMessageHandler);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _server.Start();
                _serialPort.Open();
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

                while (!stoppingToken.IsCancellationRequested) { }
            }
            catch (Exception ex)
            {
                RestartWebsocketConnection();
                _webMessageHandler.SendMessage(ex.Message);
            }
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string data = serialPort.ReadExisting();
            if (!string.IsNullOrWhiteSpace(data))
                if (_webMessageHandler.State == WebSocketState.Open)
                    _webMessageHandler.SendMessage(data);
        }

        private void RestartWebsocketConnection()
        {
            if (_server.IsListening)
            {
                _server.Stop();
                _server.Start();
            }
        }
    }
}
