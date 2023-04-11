using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        foreach (object obj in objects)
            createCount++;

        while (true)
        {
            // generate a deterministic unique guid
            using (DataStream writer = new (new MemoryStream()))
            {
                writer.WriteGuid(fileGuid);
                writer.WriteInt32(++createCount);

                using (MD5 md5 = MD5.Create())
                {
                    outGuid = new Guid(md5.ComputeHash(writer.ToByteArray()));
                    bool bFound = false;
                    foreach (dynamic obj in objects)
                    {
                        AssetClassGuid objGuid = obj.GetInstanceGuid();
                        if (objGuid.ExportedGuid == outGuid)
                        {
                            // try again
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                        break;
                }
            }
        }

        return outGuid;
    }

    public static Sha1 GenerateSha1(byte[] buffer)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            Sha1 newSha1 = new Sha1(sha1.ComputeHash(buffer));
            return newSha1;
        }
    }

    public static ulong GenerateResourceId()
    {
        Random random = new Random();

        const ulong min = ulong.MinValue;
        const ulong max = ulong.MaxValue;

        const ulong uRange = (ulong)(max - min);
        ulong ulongRand;

        do
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            ulongRand = (ulong)BitConverter.ToInt64(buf, 0);

        } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

        return ((ulongRand % uRange) + min) | 1;
    }
}