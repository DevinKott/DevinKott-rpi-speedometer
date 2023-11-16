namespace DevinKott.RPi
{
    public interface IRaspberryPiOperations
    {
        public Task Setup(int TriggerPin, int EchoPin);
        public Task TearDown();
        public Task Start(CancellationToken cancellationToken);
    }
}