using DevinKott.RPi;
using Microsoft.Extensions.Logging;

/// Create a logging factory
using var loggerFactory = LoggerFactory.Create(
    builder =>
    {
        builder.AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddConsole();
    }
);

/// Create ILoggers
ILogger<RaspberryPiDriver> logger = loggerFactory.CreateLogger<RaspberryPiDriver>();

/// Instantiate our custom driver with a logger
RaspberryPiDriver driver = new(logger);

// Trigger: board 7, gpio 4
// Echo: board 11, gpio 17
await driver.Setup(7, 11);

CancellationTokenSource cts = new();
await Task.Run(() => driver.Start(cts.Token));
