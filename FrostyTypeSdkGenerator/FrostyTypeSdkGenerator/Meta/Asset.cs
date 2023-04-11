namespace Frosty.Sdk.Ebx;

public partial class Asset
{
    [global::Frosty.Sdk.Attributes.IsReadOnlyAttribute()]
    [global::Frosty.Sdk.Attributes.CategoryAttribute("Annotations")]
    public CString Name
    {
        get => _Name;
        set => _Name = value;
    }
}