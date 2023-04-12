namespace Frosty.Sdk.Ebx;

public partial class DataContainer
{
}

public partial class Asset : DataContainer
{
    [global::Frosty.Sdk.Attributes.EbxFieldMeta(global::Frosty.Sdk.Sdk.TypeFlags.TypeEnum.CString, 0u)]
    private global::Frosty.Sdk.Ebx.CString _Name;
}

public partial class DataContainerAsset : DataContainer
{
    private global::Frosty.Sdk.Ebx.CString _Name;
}