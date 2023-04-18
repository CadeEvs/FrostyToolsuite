namespace Frosty.Sdk.Managers.Entries;

public class SuperBundleEntry
{
    /// <summary>
    /// The name of this <see cref="SuperBundleEntry"/>.
    /// </summary>
    public string Name { get; }

    public SuperBundleEntry(string inName)
    {
        Name = inName;
    }
}