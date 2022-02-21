namespace FrostySdk.Interfaces
{
    public interface ILoggable
    {
        void SetLogger(ILogger inLogger);
        void ClearLogger();
    }
}
