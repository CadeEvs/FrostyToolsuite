namespace Frosty.Sdk.Utils;

public interface ILogger
{
    public void Report(string category, string message);
    public void Report(string category, string message, double progress);
    public void Report(double progress);
}