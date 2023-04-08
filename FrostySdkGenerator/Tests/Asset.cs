using Frosty.Sdk.Attributes;

namespace Frosty.Sdk.Ebx;

public partial class Asset
{
    [IsReadOnly]
    [Category("Annotations")]
    public CString Name
    {
        get => _Name;
        set => _Name = value;
    }
}