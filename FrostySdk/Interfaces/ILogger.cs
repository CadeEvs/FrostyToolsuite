namespace FrostySdk.Interfaces
{
    public interface ILogger
    {
        void Log(string text, params object[] vars);
        void LogWarning(string text, params object[] vars);
        void LogError(string text, params object[] vars);
    }
}
