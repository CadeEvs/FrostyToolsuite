namespace Frosty.Sdk.Ebx;

public partial class DataContainer
{
}

public partial class Asset : DataContainer
{
    [Attributes.EbxFieldMeta(Sdk.TypeFlags.TypeEnum.CString, 0u)]
    private CString _Name;
}

public partial class DataContainerAsset : DataContainer
{
    private CString _Name;
}