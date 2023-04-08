using System.Collections.Generic;

namespace Frosty.Sdk.IO.Ebx;

internal class EbxTypeResolver
{
    private readonly List<EbxTypeDescriptor> m_typeDescriptors;
    private readonly List<EbxFieldDescriptor> m_fieldDescriptors;

    internal EbxTypeResolver(List<EbxTypeDescriptor> inTypeDescriptors, List<EbxFieldDescriptor> inFieldDescriptors)
    {
        EbxSharedTypeDescriptors.Initialize();
        m_typeDescriptors = inTypeDescriptors;
        m_fieldDescriptors = inFieldDescriptors;
    }

    public EbxTypeDescriptor ResolveType(int index)
    {
        EbxTypeDescriptor typeDescriptor = m_typeDescriptors[index];
        if (typeDescriptor.IsSharedTypeDescriptorKey())
        {
            return EbxSharedTypeDescriptors.GetTypeDescriptor(typeDescriptor.ToKey());
        }

        return typeDescriptor;
    }

    public EbxTypeDescriptor ResolveType(EbxTypeDescriptor typeDescriptor, int index)
    {
        if (typeDescriptor.IsSharedTypeDescriptorKey())
        {
            return EbxSharedTypeDescriptors.GetTypeDescriptor(typeDescriptor.ToKey(), (short)index);
        }

        return m_typeDescriptors[index];
    }

    public virtual EbxFieldDescriptor ResolveField(int index)
    {
        if (m_fieldDescriptors.Count == 0)
        {
            return EbxSharedTypeDescriptors.GetFieldDescriptor(index);
        }

        return m_fieldDescriptors[index];
    }
}