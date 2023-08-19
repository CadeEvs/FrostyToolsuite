namespace Frosty.Sdk.Managers.Loaders.Helpers;

internal struct BundleInfo
{
    public string Name;
    public string SbName;
    public long Offset;
    public long Size;
    public bool IsDelta;
    public bool IsPatch;
    public bool IsNonCas;
}