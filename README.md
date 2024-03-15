# Service Library

The Service library provides a framework for building MQTT-based services in C#. This README.md provides an overview of how to use the library and its key components.

## MqttService

The `MqttService` class is an abstract base class that serves as the foundation for implementing MQTT services. It facilitates communication over the MQTT protocol by handling message publishing, subscription, and packet handling.

## Prerequisites

Before running the application or the unit tests, you need to install and run Mosquitto, an open-source message broker that uses the MQTT protocol.

### Installing Mosquitto

You can download Mosquitto from the [official website](https://mosquitto.org/download/). Follow the instructions provided for your specific operating system.

### Running Mosquitto

Please note that the unit tests assume Mosquitto is installed in the default location (`C:\Program Files\mosquitto\mosquitto`).

## Usage

To use the `MqttService` class, you need to create a derived class and implement specific functionality. Here's an example of how to use the `MqttService` class:

```csharp
public class MyService : MqttService
{
	public override string Name => "MyUniqueServiceName";

	public MyService(MyServiceConfig config) : base(config)
	{
		//Add our services subscriptions here with their apropriate lambda callback function
		Subscribe("MySubscriptionId", HandlePacket);
	}

	private void HandlePacket(string message)
	{
		//Deserialise the json data into a struct inheriting from IPacket
		var packet = JsonConvert.DeserializeObject<MyPacket>(message);

		//Publish some data
		Publish("TestSubscriptionId", "We Found A Dummy Packet!");
	}
}
```

or a more direct example from the LoggingService:

```csharp
public class LogService : MqttService
{
    public override string Name => Services.Logger;

    private LogLevel Threshold => ((LoggerConfig)Config).LogThreshold;

    private string LogFolder => ((LoggerConfig)Config).LogFolder;

    public LogService(LoggerConfig config) : base(config)
    {
        Subscribe(LogPacket.Topic, HandleLogMessage);
    }

    private void HandleLogMessage(string message)
    {
        try
        {
            var packet = JsonConvert.DeserializeObject<LogPacket>(message);
            if (packet.Level >= Threshold)
            {
                string logMsg = $"{DateTime.Now:G}: {packet.Level} - {packet.Message}{Environment.NewLine}";
                if (!string.IsNullOrEmpty(packet.StackTrace))
                {
                    logMsg += $"{packet.StackTrace}{Environment.NewLine}";
                }

                LogMessage(logMsg);
            }
        }
        catch (Exception e)
        {
            LogMessage($"{DateTime.Now:G}: {LogLevel.Error} - {e.Message}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}");
        }
    }

    private void LogMessage(string message)
    {
        Directory.CreateDirectory(LogFolder);
        string logFilePath = Path.Combine(LogFolder, $"{DateTime.Now:yyyyMMdd}.log");
        File.AppendAllText(logFilePath, message);
    }
}
```

### Example useage of creating a new service
To create a new service use the ServiceFactory, you can use the following syntax:

```csharp
var config = Config.Load<LoggerConfig>("config.json");
ServiceFactory.ServiceRunner<LogService>(config);
```
This example demonstrates creating a LogService instance and running it with configurations loaded from a JSON file named config.json.

### Configuration
Configuration files are generated if they do not already exist with default values. When creating a new configuration file, make sure to inherit from the Config class located in the Service.Config namespace. This will allow the Service to load the configuration file.

Here is an example of a configuration class and its matching configuration file:
```csharp
public class LoggerConfig : Config
{
    public string LogFolder { get; set; } = "logs";

    public LogLevel LogThreshold { get; set; } = LogLevel.Debug;
}
```
```json
{
  "LogFolder": "logs",
  "LogThreshold": 0,
  "MqttIp": "127.0.0.1"
}
```

## Other Services
For further examples on how to use the library, you can refer to other services such as the LoggingService. This service handles logging messages received over MQTT and writes them to a specified log folder based on the log level. Adjustments can be made according to your project's specific requirements and conventions.

##Using Mock Services for Unit Tests
In software development, unit testing is essential for ensuring that individual components of the codebase function correctly in isolation. However, when dealing with components that rely on external dependencies or services, such as network communication or databases, writing effective unit tests can become challenging.

Mock services provide a solution to this problem by simulating the behavior of external dependencies within a controlled environment, allowing developers to test their code in isolation without relying on real external services. These mock services mimic the behavior of real services but are designed specifically for testing purposes.

###Example: MockTransactionAuthService
In our project, we use mock services extensively for unit testing our MQTT-based services. Here's an example of a mock service, MockTransactionAuthService, which simulates the behavior of a transaction authorization service:

```csharp
public class MockTransactionAuthService : MqttService
{
    private TransactionAuthStatus ExpectedStatus;

    public bool IsTransactionAuthReceived { get; private set; }

    public MockTransactionAuthService(Config config, TransactionAuthStatus status) : base(config)
    {
        ExpectedStatus = status;
        Subscribe(TransactionAuthPacket.Topic, HandleTransactionAuthMessage);
    }

    private void HandleTransactionAuthMessage(string message)
    {
        var packet = JsonConvert.DeserializeObject<TransactionAuthPacket>(message);
        Assert.NotNull(packet);
        Assert.AreEqual(ExpectedStatus, packet.Status);
        IsTransactionAuthReceived = true;
    }
}
```
In this example, MockTransactionAuthService extends MqttService and simulates the behavior of a transaction authorization service. It subscribes to a specific topic and verifies the correctness of received messages according to the expected status. This mock service allows us to test components that rely on transaction authorization without interacting with a real service.

Advantages of Using Mock Services
Isolation: Mock services allow unit tests to run in isolation without relying on external dependencies.
Controlled Environment: Developers can simulate various scenarios and edge cases by controlling the behavior of mock services.
Fast and Reliable: Mock services provide fast and reliable feedback during unit testing, improving the efficiency of the testing process.
By incorporating mock services into our unit testing strategy, we ensure the reliability and robustness of our codebase while maintaining a fast and efficient testing workflow.

## Contributing
To enhance the Service library with new packet types and shared enums, we'll create separate folders and namespaces for enums, packets and globals. This will help define a base API that can be shared among services, ensuring consistency and preventing mismatches in API definitions.

Here's how you can maintain organization to the folders and namespaces:

- Enums: Add new shared Enums to the folder called Enums and to the Services.Enums namespace.
- Packets: Add new shared Packets to the folder called Packets and to the Services.Packets namespace.
- Globals: Add new shared static global variables to the folder called Globals and to the Services.Globals namespace. New services must define their service name within Services.Globals.Services.cs.

Each of these folders will contain C# files defining enums and packet classes, respectively.

Here's an example structure:

markdown
```
Service
│
├── Enums
│   ├── LogLevel.cs
│   └── ...
│
├── Packets
|    ├── LogPacket.cs
│    ├── IPacket.cs
|    └── ...
└── Globals
	 ├── Services.cs
     └── ...
```

And here's how the namespaces should look:

```csharp
namespace Service.Enums
{
    ...
}

namespace Service.Packets
{
    ...
}

namespace Service.Globals
{
    ...
}
```

By organizing enums and packets in separate folders and namespaces, you ensure a clear and structured API for the Service library, making it easier to maintain and share among different services.