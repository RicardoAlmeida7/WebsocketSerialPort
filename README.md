# WebsocketSerialPort
Worker service application to establish communication between websocket and a serial port.

## Requirements
This worker service requires .Net 7 SDK.

## Build
### On the same level as the solution.
The `--self-contained=true` flag determines that the necessary packages will be sent to the application directory, this way it will not be necessary to install the runtime on the client machine. [See more](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build)

For Windows
```
dotnet publish -c Release -r win-x64 --self-contained=true -p:PublishSingleFile=true
```
For Linux
```
dotnet publish -c Release -r linux-x64 --self-contained=true -p:PublishSingleFile=true
```

# Configuration
## In the directory where the application was published there will be an `appsettings` file where you can configure some parameters:
* WebsocketPort -> Websocket connection port;
* Port -> Serial port;
* BaudRate -> Transmit bits per second.

# Install
## The application is a worker service that runs in the background.

For Windows
```
sc create <service name> binPath= "your path\WebsocketSerialPort.exe"
net start <service name>
```

For Linux
Installing the service on Linux requires creating a configuration file. [Follow the steps outlined on Maarten Balliauw's blog.](https://blog.maartenballiauw.be/post/2021/05/25/running-a-net-application-as-a-service-on-linux-with-systemd.html)

# Connection endpoint 
```
ws://localhost:3300/serial
```

  
