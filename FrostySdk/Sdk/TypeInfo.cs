using System.Collections.Generic;
using System.Text;
using Frosty.Sdk.IO;
using Frosty.Sdk.Profiles;
using Frosty.Sdk.Sdk.TypeInfoDatas;
using Frosty.Sdk.Sdk.TypeInfos;

namespace Frosty.Sdk.Sdk;

internal class TypeInfo
{
    public static int Version
    {
        get
        {
            switch ((ProfileVersion)ProfilesLibrary.DataVersion)
            {
                // Prev TypeInfo
                case ProfileVersion.Fifa21:
                case ProfileVersion.Madden22:
                case ProfileVersion.Fifa22:
                case ProfileVersion.Battlefield2042:
                case ProfileVersion.Madden23:
                case ProfileVersion.Fifa23:
                case ProfileVersion.NeedForSpeedUnbound:
                case ProfileVersion.DeadSpace:
                    return 6;

                // Guid and NameHash in TypeInfoData
                case ProfileVersion.Fifa19:
                case ProfileVersion.Anthem:
                case ProfileVersion.Madden20:
                case ProfileVersion.Fifa20:
                case ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                case ProfileVersion.NeedForSpeedHeat:
                    return 5;

                // Guid in TypeInfo
                case ProfileVersion.Fifa18:
                case ProfileVersion.NeedForSpeedPayback:
                case ProfileVersion.StarWarsBattlefrontII:
                case ProfileVersion.Battlefield5:
                case ProfileVersion.Madden19:
                case ProfileVersion.StarWarsSquadrons:
                    return 4;

                // ArrayInfo
                case ProfileVersion.Fifa17:
                case ProfileVersion.MassEffectAndromeda:
                    return 3;

                // ushort FieldCount
                case ProfileVersion.Battlefield4: // would expect version 1 ???
                case ProfileVersion.Battlefield1:
                case ProfileVersion.StarWarsBattlefront:
                case ProfileVersion.MirrorsEdgeCatalyst:
                case ProfileVersion.NeedForSpeed:
                case ProfileVersion.NeedForSpeedEdge:
                case ProfileVersion.PlantsVsZombiesGardenWarfare2:
                    return 2;

                case ProfileVersion.NeedForSpeedRivals:
                case ProfileVersion.DragonAgeInquisition:
                case ProfileVersion.PlantsVsZombiesGardenWarfare:
                    return 1;
                
                default:
                    return 0;
            }
        }
    }

    public static readonly Dictionary<long, TypeInfo> TypeInfoMapping = new();

    public static bool HasNames => !ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.Battlefield2042);

    protected TypeInfoData m_data;
    protected long p_prev;
    protected long p_next;
    protected ushort m_id;
    protected ushort m_flags;

    public TypeInfo(TypeInfoData data)
    {
        m_data = data;
    }

    public static TypeInfo ReadTypeInfo(MemoryReader reader)
    {
        long startPos = reader.Position;

        long typeInfoDataOffset = reader.ReadLong();

        long curPos = reader.Position;

        reader.Position = typeInfoDataOffset;

        TypeInfoData data = TypeInfoData.ReadTypeInfoData(reader);

        reader.Position = curPos;

        TypeInfo retVal = CreateTypeInfo(data);

        retVal.Read(reader);

        TypeInfoMapping.Add(startPos, retVal);

        return retVal;
    }

    public static TypeInfo CreateTypeInfo(TypeInfoData data)
    {
        if (data is StructInfoData structData)
        {
            return new StructInfo(structData);
        }
        if (data is ClassInfoData classData)
        {
            return new ClassInfo(classData);
        }
        if (data is ArrayInfoData arrayData)
        {
            return new ArrayInfo(arrayData);
        }
        if (data is EnumInfoData enumData)
        {
            return new EnumInfo(enumData);
        }
        if (data is FunctionInfoData functionData)
        {
            return new FunctionInfo(functionData);
        }
        if (data is DelegateInfoData delegateData)
        {
            return new DelegateInfo(delegateData);
        }
        return new TypeInfo(data);
    }

    public virtual void Read(MemoryReader reader)
    {
        if (Version > 5)
        {
            p_prev = reader.ReadLong();
        }

        p_next = reader.ReadLong();

        if (Version == 4)
        {
            m_data.SetGuid(reader.ReadGuid());
            reader.ReadGuid();
        }
        if (Version == 5)
        {
            reader.ReadGuid();
        }

        m_id = reader.ReadUShort();
        m_flags = reader.ReadUShort();
    }

    public TypeInfo? GetNextTypeInfo(MemoryReader reader)
    {
        if (p_next == 0)
        {
            return null;
        }
        reader.Position = p_next;
        return ReadTypeInfo(reader);
    }
    
    public string GetName() => m_data.CleanUpName();

    public TypeFlags GetFlags() => m_data.GetFlags();

    public void CreateType(StringBuilder sb)
    {
        m_data.CreateType(sb);
    }
}