using System.Collections.Generic;
using System.Text;
using Frosty.Sdk.IO;
using Frosty.Sdk.Sdk.TypeInfos;

namespace Frosty.Sdk.Sdk.TypeInfoDatas;

internal class FunctionInfoData : TypeInfoData
{
    private enum Modifier
    {
        None,
        Virtual,
        Override
    }

    private List<ParameterInfo> m_parameterInfos = new();
    private Modifier m_modifier;

    public override void Read(MemoryReader reader)
    {
        base.Read(reader);

        if (ProfilesLibrary.HasStrippedTypeNames && string.IsNullOrEmpty(m_name))
        {
            m_name = $"Function_{m_nameHash:x8}";
        }

        reader.ReadLong();
        reader.ReadLong();
        reader.ReadLong();
        reader.ReadLong();
        reader.ReadLong();

        long pParameterInfos = reader.ReadLong();
        m_modifier = (Modifier)reader.ReadInt();

        reader.Position = pParameterInfos;
        for (int i = 0; i < m_fieldCount; i++)
        {
            m_parameterInfos.Add(new ParameterInfo());
            m_parameterInfos[i].Read(reader);
        }
    }

    public override void CreateType(StringBuilder sb)
    {
        base.CreateType(sb);

        string returnType = "void";

        StringBuilder inputParams = new();
        bool hasReturn = false;

        foreach (ParameterInfo parameterInfo in m_parameterInfos)
        {
            TypeInfo type = parameterInfo.GetTypeInfo();

            string typeName = type.GetName();

            if (type is ArrayInfo array)
            {
                typeName = $"List<{array.GetTypeInfo().GetName()}>";
            }

            switch (parameterInfo.GetParameterType())
            {
                case 0:
                    inputParams.Append($"{typeName} {parameterInfo.GetName()}, ");
                    break;
                case 1:
                    if (!hasReturn)
                    {
                        hasReturn = true;
                    }
                    else
                    {

                    }
                    returnType = typeName;
                    break;
                case 2:
                    inputParams.Append($"{typeName}* {parameterInfo.GetName()}, ");
                    break;
                case 3:
                    if (!hasReturn)
                    {
                        hasReturn = true;
                    }
                    else
                    {

                    }
                    returnType = $"{typeName}*";
                    break;
            }
        }

        if (inputParams.Length > 0)
        {
            inputParams.Remove(inputParams.Length - 2, 2);
        }

        sb.AppendLine($"public unsafe{(m_modifier == Modifier.Virtual ? " virtual": m_modifier == Modifier.Override ? " override" : "")} {returnType} {m_name} ({inputParams})");

        sb.AppendLine("{");

        sb.AppendLine("throw new NotSupportedException(\"Calling functions is not supported.\");");

        sb.AppendLine("}");
    }
}