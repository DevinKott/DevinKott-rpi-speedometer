using System.Device.Gpio;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DevinKott.RPi
{
    public class RaspberryPiDriver : IRaspberryPiOperations
    {
        private readonly ILogger<RaspberryPiDriver> _logger;
        private GpioController? gpioController;
        private int TriggerPin;
        private int EchoPin;

        public RaspberryPiDriver(ILogger<RaspberryPiDriver> logger)
        {
            _logger = logger;
        }

        public Task Setup(int TriggerPin, int EchoPin)
        {
            this.TriggerPin = TriggerPin;
            this.EchoPin = EchoPin;

            _logger.LogInformation($"RaspberryPiDriver - Setup: RaspberryPiDriver setting up with {TriggerPin} as the trigger pin and {EchoPin} as the echo pin.");

            _logger.LogDebug("RaspberryPiDriver - Setup: configuring GpioController...");
            try {
                gpioController = new GpioController(PinNumberingScheme.Board);
                gpioController.OpenPin(this.TriggerPin, PinMode.Output);
                gpioController.OpenPin(this.EchoPin, PinMode.Input);

                _logger.LogDebug("RaspberryPiDriver - Setup: Setting the trigger to low...");
                gpioController.Write(this.TriggerPin, PinValue.Low);
            } catch (PlatformNotSupportedException e)
            {
                _logger.LogError("The current platform is not supported.", e);
                // TODO: Return a failed task here
            }

            return Task.CompletedTask;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RaspberryPiDriver - Start: Settling system...");
            await Task.Delay(5_000);
            _logger.LogInformation("RaspberryPiDriver - Start: Starting measuring cycles.");

            if (gpioController == null)
            {
                _logger.LogError("RaspberryPiDriver - Start: Cannot start; GPIO controller is null.");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                gpioController.Write(TriggerPin, PinValue.High);
                await MicroSleep(10);
                gpioController.Write(TriggerPin, PinValue.Low);
                
                _logger.LogInformation("Doing stuff...");
                await Task.Delay(1_000);
            }
        }

        public async Task MicroSleep(int microseconds)
        {
            var stopwatch = Stopwatch.StartNew();
            int delayInMilliseconds = microseconds / 1000;
            await Task.Delay(delayInMilliseconds);
            stopwatch.Stop();
            int actualDelayInMicroseconds = (int)(stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1_000_000));
            int remainingMicroseconds = microseconds - actualDelayInMicroseconds;
            if (remainingMicroseconds > 0)
            {
                await MicroSleep(remainingMicroseconds);
            }
        }

        public Task TearDown()
        {
            throw new NotImplementedException();
        }
    }
}