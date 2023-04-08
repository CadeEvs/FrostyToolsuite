using System;
using System.Collections.Generic;

namespace Frosty.Sdk;

public static class TypeLibrary
{
    private static readonly Dictionary<string, int> s_nameMapping = new();
    private static readonly Dictionary<uint, int> s_nameHashMapping = new();
    private static readonly Dictionary<Guid, int> s_guidMapping = new();
    private static readonly List<Type> s_types = new();

    public static Type? GetType(string name)
    {
        if (!s_nameMapping.TryGetValue(name, out int index))
        {
            return null;
        }
        return s_types[index];
    }
    
    public static Type? GetType(uint nameHash)
    {
        if (!s_nameHashMapping.TryGetValue(nameHash, out int index))
        {
            return null;
        }
        return s_types[index];
    }
    
    public static Type? GetType(Guid guid)
    {
        if (!s_guidMapping.TryGetValue(guid, out int index))
        {
            return null;
        }
        return s_types[index];
    }
    
    public static object? CreateObject(string name)
    {
        Type? type = GetType(name);
        return type == null ? null : Activator.CreateInstance(type);
    }

    public static object? CreateObject(uint nameHash)
    {
        Type? type = GetType(nameHash);
        return type == null ? null : Activator.CreateInstance(type);
    }
    
    public static object? CreateObject(Guid guid)
    {
        Type? type = GetType(guid);
        return type == null ? null : Activator.CreateInstance(type);
    }
    
    public static object? CreateObject(Type type)
    {
        return Activator.CreateInstance(type);
    }
}