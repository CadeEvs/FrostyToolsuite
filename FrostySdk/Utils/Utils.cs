using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;
using Frosty.Sdk.Ebx;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Utils;

public static class Utils
{
    public static int HashString(string value, bool toLower = false)
    {
        const uint kOffset = 5381;
        const uint kPrime = 33;
        
        uint hash = kOffset;
        for (int i = 0; i < value.Length; i++)
        {
            hash = (hash * kPrime) ^ (byte)(toLower ? char.ToLower(value[i]) : value[i]);
        }

        return (int)hash;
    }
    
    public static Guid GenerateDeterministicGuid(IEnumerable<object> objects, string type, Guid fileGuid)
    {
        return GenerateDeterministicGuid(objects, TypeLibrary.GetType(type)!, fileGuid);
    }

    public static Guid GenerateDeterministicGuid(IEnumerable<object> objects, Type type, Guid fileGuid)
    {
        Guid outGuid;

        int createCount = 0;
        HashSet<Guid> existingGuids = new();
        foreach (dynamic obj in objects)
        {
            AssetClassGuid objGuid = obj.GetInstanceGuid();
            existingGuids.Add(objGuid.ExportedGuid);
            createCount++;
        }
        
        Block<byte> buffer = new(stackalloc byte[20]);

        Span<byte> result = stackalloc byte[16];
        while (true)
        {
            // generate a deterministic unique guid
            using (DataStream writer = new(buffer.ToStream()))
            {
                writer.WriteGuid(fileGuid);
                writer.WriteInt32(++createCount);
            }

            MD5.HashData(buffer.ToSpan(), result);
            outGuid = new Guid(result);

            if (!existingGuids.Contains(outGuid))
            {
                break;
            }
        }
        
        buffer.Dispose();

        return outGuid;
    }

    public static Sha1 GenerateSha1(ReadOnlySpan<byte> buffer)
    {
        Span<byte> hashed = stackalloc byte[20];
        SHA1.HashData(buffer, hashed);
        Sha1 newSha1 = new(hashed);
        return newSha1;
    }

    public static ulong GenerateResourceId()
    {
        Random random = new();

        const ulong min = ulong.MinValue;
        const ulong max = ulong.MaxValue;

        const ulong uRange = max - min;
        ulong ulongRand;

        Span<byte> buf = stackalloc byte[8];
        do
        {
            random.NextBytes(buf);
            ulongRand = BinaryPrimitives.ReadUInt64LittleEndian(buf);

        } while (ulongRand > max - (max % uRange + 1) % uRange);

        return (ulongRand % uRange + min) | 1;
    }
}