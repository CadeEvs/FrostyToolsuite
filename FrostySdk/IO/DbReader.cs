using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Interfaces;

namespace FrostySdk.IO
{
    internal enum DbType
    {
        Invalid = 0,
        List = 1,
        Object = 2,
        Boolean = 6,
        String = 7,
        Int = 8,
        Long = 9,
        Float = 11,
        Double = 12,
        Guid = 15,
        Sha1 = 16,
        ByteArray = 19
    }

    public class DbReader : NativeReader
    {
        public DbReader(Stream inStream, IDeobfuscator inDeobfuscator)
            : base(inStream, inDeobfuscator)
        {
        }

        public virtual DbObject ReadDbObject() => (DbObject)ReadDbObject(out string tmpName);

        protected object ReadDbObject(out string objName)
        {
            objName = "";
            byte tmp = ReadByte();

            DbType objType = (DbType)(tmp & 0x1F);
            if (objType == DbType.Invalid)
                return null;

            if ((tmp & 0x80) == 0)
                objName = ReadNullTerminatedString();

            switch (objType)
            {
                case DbType.List:
                    {
                        long size = Read7BitEncodedLong();
                        long offset = Position;

                        List<object> values = new List<object>();
                        while (Position - offset < size)
                        {
                            string tmpName = "";
                            object subValue = ReadDbObject(out tmpName);

                            if (subValue == null)
                                break;

                            values.Add(subValue);
                        }
                        return new DbObject(values);
                    }

                case DbType.Object:
                    {
                        long size = Read7BitEncodedLong();
                        long offset = Position;

                        Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        while (Position - offset < size)
                        {
                            object subValue = ReadDbObject(out string tmpName);

                            if (subValue == null)
                                break;

                            values.Add(tmpName, subValue);
                        }
                        return new DbObject(values);
                    }

                case DbType.Boolean: return ReadByte() == 1;
                case DbType.String: return ReadSizedString(Read7BitEncodedInt());
                case DbType.Int: return ReadInt();
                case DbType.Long: return ReadLong();
                case DbType.Float: return ReadFloat();
                case DbType.Double: return ReadDouble();
                case DbType.Guid: return ReadGuid();
                case DbType.Sha1: return ReadSha1();
                case DbType.ByteArray: return ReadBytes(Read7BitEncodedInt());
            }

            return null;
        }
    }
}
