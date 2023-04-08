namespace Frosty.Sdk.Managers.Patch;

public class PatchResult
{
    public PatchInfo Added { get; set; } = new();
    public PatchInfo Modified { get; set; } = new();
    public PatchInfo Removed { get; set; } = new();
}