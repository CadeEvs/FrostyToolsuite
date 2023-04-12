namespace Frosty.Sdk.Ebx;

public partial class Asset
{
    [global::Frosty.Sdk.Attributes.IsReadOnlyAttribute()]
    [global::Frosty.Sdk.Attributes.CategoryAttribute("Annotations")]
    public Frosty.Sdk.Ebx.CString Name
    {
        get => _Name;
        set => _Name = value;
    }

    public string ShortName
    {
        get => ((string)Name).Substring(1);
    }
}