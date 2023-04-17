using System.Reflection;
using Frosty.Sdk.Attributes;
using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.Ebx;

[EbxTypeMeta(TypeFlags.TypeEnum.Class)]
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

[EbxTypeMetaAttribute(73, 8, 16, "Entitlements")]
[DisplayNameAttribute("EntitlementConfigData")]
[GuidAttribute("77ecf47c-74b6-bf7b-fcf7-428496d73457")]
[ArrayGuidAttribute("ee8fd798-4335-95bd-4460-aed4f5666a03")]
public partial struct EntitlementConfigData
{
    [FieldIndexAttribute(0)]
    [EbxArrayMetaAttribute(73)]
    [EbxFieldMetaAttribute(145, 0, null)]
    private global::System.Collections.Generic.List<Asset> _Groups;
    [FieldIndexAttribute(1)]
    [EbxFieldMetaAttribute(49677, 8, null)]
    private uint _PageSize;
}

public partial struct EntitlementConfigData
{
    [Frosty.Sdk.Attributes.FieldIndexAttribute(0)]
    [Frosty.Sdk.Attributes.EbxArrayMetaAttribute(73)]
    [Frosty.Sdk.Attributes.EbxFieldMetaAttribute(145, 0, null)]
    public global::System.Collections.Generic.List<global::Frosty.Sdk.Ebx.Asset> Groups
    {
        get => _Groups;
        set => _Groups = value;
    }
    [Frosty.Sdk.Attributes.FieldIndexAttribute(1)]
    [Frosty.Sdk.Attributes.EbxFieldMetaAttribute(49677, 8, null)]
    public uint PageSize
    {
        get => _PageSize;
        set => _PageSize = value;
    }

    public EntitlementConfigData()
    {
        _Groups = new();
    }
}