using System;

namespace Frosty.Sdk;

public static class DbObjectExtensions
{
    public static T As<T>(this object? obj)
    {
        if (obj is T t)
        {
            return t;
        }

        object? objToConvert = obj;

        if (obj is sbyte b)
        {
            if (typeof(T) == typeof(byte))
            {
                objToConvert = (byte)b;
            }
            else if (typeof(T) == typeof(ushort))
            {
                objToConvert = (ushort)b;
            }
            else if (typeof(T) == typeof(uint))
            {
                objToConvert = (uint)b;
            }
            else if (typeof(T) == typeof(ulong))
            {
                objToConvert = (ulong)b;
            }
        }
        else if (obj is short s)
        {
            if (typeof(T) == typeof(byte))
            {
                objToConvert = (byte)s;
            }
            else if (typeof(T) == typeof(ushort))
            {
                objToConvert = (ushort)s;
            }
            else if (typeof(T) == typeof(uint))
            {
                objToConvert = (uint)s;
            }
            else if (typeof(T) == typeof(ulong))
            {
                objToConvert = (ulong)s;
            }
        }
        else if (obj is int i)
        {
            if (typeof(T) == typeof(byte))
            {
                objToConvert = (byte)i;
            }
            else if (typeof(T) == typeof(ushort))
            {
                objToConvert = (ushort)i;
            }
            else if (typeof(T) == typeof(uint))
            {
                objToConvert = (uint)i;
            }
            else if (typeof(T) == typeof(ulong))
            {
                objToConvert = (ulong)i;
            }
        }
        else if (obj is long l)
        {
            if (typeof(T) == typeof(byte))
            {
                objToConvert = (byte)l;
            }
            else if (typeof(T) == typeof(ushort))
            {
                objToConvert = (ushort)l;
            }
            else if (typeof(T) == typeof(uint))
            {
                objToConvert = (uint)l;
            }
            else if (typeof(T) == typeof(ulong))
            {
                objToConvert = (ulong)l;
            }
        }
        
        T? retVal = (T?)Convert.ChangeType(objToConvert, typeof(T));

        if (retVal == null)
        {
            throw new InvalidCastException($"Couldn't cast {objToConvert?.GetType().Name} to type {typeof(T).Name}");
        }

        return retVal;
    }
}