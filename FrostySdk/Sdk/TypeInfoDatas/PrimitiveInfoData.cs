using System.Text;

namespace Frosty.Sdk.Sdk.TypeInfoDatas;

internal class PrimitiveInfoData : TypeInfoData
{
    public override void CreateType(StringBuilder sb)
    {
        string actualType = string.Empty;

        switch (m_flags.GetTypeEnum())
        {
            case TypeFlags.TypeEnum.String:
                actualType = "System.String";
                break;
            case TypeFlags.TypeEnum.Boolean:
                actualType = "System.Boolean";
                break;
            case TypeFlags.TypeEnum.Int8:
                actualType = "System.SByte";
                break;
            case TypeFlags.TypeEnum.UInt8:
                actualType = "System.Byte";
                break;
            case TypeFlags.TypeEnum.Int16:
                actualType = "System.Int16";
                break;
            case TypeFlags.TypeEnum.UInt16:
                actualType = "System.UInt16";
                break;
            case TypeFlags.TypeEnum.Int32:
                actualType = "System.Int32";
                break;
            case TypeFlags.TypeEnum.UInt32:
                actualType = "System.UInt32";
                break;
            case TypeFlags.TypeEnum.Int64:
                actualType = "System.Int64";
                break;
            case TypeFlags.TypeEnum.UInt64:
                actualType = "System.UInt64";
                break;
            case TypeFlags.TypeEnum.Float32:
                actualType = "System.Single";
                break;
            case TypeFlags.TypeEnum.Float64:
                actualType = "System.Double";
                break;
            case TypeFlags.TypeEnum.Guid:
                actualType = "System.Guid";
                break;
            case TypeFlags.TypeEnum.Sha1:
                actualType = "Frosty.Sdk.Sha1";
                break;
        }

        if (!string.IsNullOrEmpty(actualType))
        {
            sb.Insert(0, $"using {m_name} = {actualType};\n");
        }
    }
}