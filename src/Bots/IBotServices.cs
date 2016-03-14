namespace Bots
{
    public interface IBotServices
    {
        INetwork Network { get; }
        ILogger Logger { get; }
    }
}