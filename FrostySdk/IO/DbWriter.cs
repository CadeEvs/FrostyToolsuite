using System;
using System.Collections.Generic;
using System.IO;

namespace FrostySdk.IO
{
    public class DbWriter : NativeWriter
    {
        private readonly bool writeHeader;

        public DbWriter(Stream inStream, bool inWriteHeader = false, bool leaveOpen = false)
            : base(inStream, leaveOpen)
        {
            writeHeader = inWriteHeader;
        }

        public virtual void Write(DbObject inObj)
        {
            if (writeHeader)
            {
                Write((ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals) ? 0x03CED100 : 0x01CED100);
                Write(new byte[0x228]);
            }

            Write(WriteDbObject("", inObj));
        }

        public byte[] WriteDbObject(string name, object inObj)
        {
            MemoryStream ms = new MemoryStream();
            using (NativeWriter writer = new NativeWriter(ms))
            {
                DbType objType = GetDbType(inObj);

                byte dbFlags = (byte)((name == "") ? 0x80 : 0x00);
                writer.Write((byte)(dbFlags | (byte)objType));

                if ((dbFlags & 0x80) == 0)
                    writer.WriteNullTerminatedString(name);

                switch (objType)
                {
                    case DbType.Object:
                        {
                            DbObject dbObj = (DbObject)inObj;
                            MemoryStream subMs = new MemoryStream();

                            byte[] buffer = null;
                            foreach (KeyValuePair<string, object> kvp in dbObj.hash)
                            {
                                buffer = WriteDbObject(kvp.Key, kvp.Value);
                                if(buffer != null)
                                    subMs.Write(buffer, 0, buffer.Length);
                            }

                            buffer = subMs.ToArray();
                            writer.Write7BitEncodedLong(buffer.Length + 1);
                            writer.Write(buffer);
                            writer.Write((byte)0x00);
                        }
                        break;

                    case DbType.List:
                        {
                            DbObject dbObj = (DbObject)inObj;
                            MemoryStream subMs = new MemoryStream();

                            byte[] buffer = null;
                            foreach (object obj in dbObj.list)
                            {
                                buffer = WriteDbObject("", obj);
                                subMs.Write(buffer, 0, buffer.Length);
                            }

                            buffer = subMs.ToArray();
                            writer.Write7BitEncodedLong(buffer.Length + 1);
                            writer.Write(buffer);
                            writer.Write((byte)0x00);
                        }
                        break;

                    case DbType.Boolean: writer.Write((byte)((bool)inObj ? 0x01 : 0x00)); break;
                    case DbType.String: writer.WriteSizedString(((string)inObj) + "\0"); break;
                    case DbType.Int: writer.Write((int)inObj); break;
                    case DbType.Long: writer.Write((long)inObj); break;
                    case DbType.Float: writer.Write((float)inObj); break;
                    case DbType.Double: writer.Write((double)inObj); break;
                    case DbType.Guid: writer.Write((Guid)inObj); break;
                    case DbType.Sha1: writer.Write((Sha1)inObj); break;
                    case DbType.ByteArray:
                        writer.Write7BitEncodedInt((int)((byte[])inObj).Length);
                        writer.Write((byte[])inObj);
                        break;
                    default: throw new InvalidDataException("Unsupported DB type detected");
                }
            }

            return ms.ToArray();
        }

        private DbType GetDbType(object inObj)
        {
            Type objType = inObj.GetType();
            if (objType == typeof(DbObject))
            {
                DbObject obj = (DbObject)inObj;
                if (obj.hash != null)
                    return DbType.Object;
                else if (obj.list != null)
                    return DbType.List;
            }
            else if (objType == typeof(bool))
                return DbType.Boolean;
            else if (objType == typeof(string))
                return DbType.String;
            else if (objType == typeof(int))
                return DbType.Int;
            else if (objType == typeof(long))
                return DbType.Long;
            else if (objType == typeof(float))
                return DbType.Float;
            else if (objType == typeof(double))
                return DbType.Double;
            else if (objType == typeof(Guid))
                return DbType.Guid;
            else if (objType == typeof(Sha1))
                return DbType.Sha1;
            else if (objType == typeof(byte[]))
                return DbType.ByteArray;

            return DbType.Invalid;
        }
    }
}
