using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FrostySdk.Managers.Entries;

namespace FrostySdk.IO
{
    public class EbxXmlWriter : IDisposable
    {
        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        internal class PropertyComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y)
            {
                return (x.MetadataToken < y.MetadataToken) ? -1 : 1;
            }
        }

        private AssetManager am;
        private List<object> objs = new List<object>();
        private Stream stream;

        public EbxXmlWriter(Stream inStream, AssetManager inAm)
        {
            am = inAm;
            stream = inStream;
        }

        public void WriteObjects(IEnumerable<object> inObjs)
        {
            objs.Clear();
            objs.AddRange(inObjs);

            StringBuilder sb = new StringBuilder();
            foreach (object obj in objs)
                sb.Append(ClassToXml(obj, obj.GetType()));

            string value = sb.ToString();
            byte[] valueBuffer = Encoding.UTF8.GetBytes(value);

            stream.Write(valueBuffer, 0, valueBuffer.Length);
        }

        private string ClassToXml(object Obj, Type ObjType, int TabCount = 0)
        {
            StringBuilder SB = new StringBuilder();
            int TotalCount = ObjType.GetProperties().Length;

            PropertyInfo[] Properties = ObjType.GetProperties(PropertyBindingFlags);
            Array.Sort(Properties, new PropertyComparer());

            string StrGuid = "";
            FieldInfo FI = ObjType.GetField("__Guid", BindingFlags.NonPublic | BindingFlags.Instance);

            if (FI != null)
            {
                AssetClassGuid Guid = (AssetClassGuid)FI.GetValue(Obj);
                StrGuid = " Guid=\"" + Guid.ToString() + "\"";
            }

            if (TotalCount != 0 && (Properties.Length > 0 || (ObjType.BaseType != typeof(object) && ObjType.BaseType != typeof(ValueType))))
            {
                SB.AppendLine("".PadLeft(TabCount) + "<" + ObjType.Name + StrGuid + ">");
                TabCount += 4;

                foreach (PropertyInfo PI in Properties)
                {
                    if (PI.GetCustomAttribute<IsTransientAttribute>() != null)
                        continue;

                    SB.Append("".PadLeft(TabCount) + "<" + PI.Name + "[AddInfo]>");

                    object Value = PI.GetValue(Obj);
                    string Tmp = "";

                    SB.Append(FieldToXml(Value, ref Tmp, TabCount));

                    SB.AppendLine("</" + PI.Name + ">");
                    SB = SB.Replace("[AddInfo]", Tmp);
                }

                TabCount -= 4;
                SB.AppendLine("".PadLeft(TabCount) + "</" + ObjType.Name + ">");
            }
            else
            {
                SB.AppendLine("".PadLeft(TabCount) + "<" + ObjType.Name + StrGuid + "/>");
            }

            return SB.ToString();
        }

        private string FieldToXml(object Value, ref string AdditionalInfo, int TabCount = 0)
        {
            Type FieldType = Value.GetType();
            StringBuilder SB = new StringBuilder();

            if (FieldType.Name == "List`1")
            {
                int Count = (int)FieldType.GetMethod("get_Count").Invoke(Value, null);
                AdditionalInfo = " Count=\"" + Count + "\"";

                if (Count > 0)
                {
                    SB.AppendLine();
                    TabCount += 4;

                    for (int i = 0; i < Count; i++)
                    {
                        SB.Append("".PadLeft(TabCount) + "<member Index=\"" + i.ToString() + "\">");

                        object SubValue = FieldType.GetMethod("get_Item").Invoke(Value, new object[] { i });
                        string Tmp = "";

                        SB.Append(FieldToXml(SubValue, ref Tmp, TabCount));

                        SB.AppendLine("</member>");
                    }

                    TabCount -= 4;
                    SB.Append("".PadLeft(TabCount));
                }
            }
            else
            {
                if (FieldType.Namespace == "FrostySdk.Ebx" && FieldType.BaseType != typeof(Enum))
                {
                    if (FieldType == typeof(CString)) SB.Append(Value.ToString());
                    else if (FieldType == typeof(ResourceRef)) SB.Append(Value.ToString());
                    else if (FieldType == typeof(FileRef)) SB.Append(Value.ToString());
                    else if (FieldType == typeof(TypeRef)) SB.Append(Value.ToString());
                    else if (FieldType == typeof(BoxedValueRef)) SB.Append(Value.ToString());
                    else if (FieldType == typeof(PointerRef))
                    {
                        PointerRef Reference = (PointerRef)Value;
                        if (Reference.Type == PointerRefType.Internal)
                        {
                            Type SubObjType = Reference.Internal.GetType();
                            AssetClassGuid guid = (AssetClassGuid)SubObjType.GetField("__Guid", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Reference.Internal);
                            SB.Append("[" + SubObjType.Name + "] " + guid.ToString());
                        }
                        else if (Reference.Type == PointerRefType.External)
                        {
                            EbxAssetEntry entry = am.GetEbxEntry(Reference.External.FileGuid);
                            if (entry != null)
                            {
                                SB.Append("[Ebx] " + entry.Name + " [" + Reference.External.ClassGuid + "]");
                            }
                            else
                            {
                                SB.Append("[Ebx] BadRef " + Reference.External.FileGuid + "/" + Reference.External.ClassGuid);
                            }
                        }
                        else
                            SB.Append("nullptr");
                    }
                    else
                    {
                        TabCount += 4;
                        SB.AppendLine();

                        SB.Append(ClassToXml(Value, Value.GetType(), TabCount));

                        TabCount -= 4;
                        SB.Append("".PadLeft(TabCount));
                    }
                }
                else
                {
                    if (FieldType == typeof(byte)) SB.Append(((byte)Value).ToString("X2"));
                    else if (FieldType == typeof(ushort)) SB.Append(((ushort)Value).ToString("X4"));
                    else if (FieldType == typeof(uint))
                    {
                        uint value = (uint)Value;
                        string val = Utils.GetString((int)value);

                        if (!val.StartsWith("0x"))
                        {
                            SB.Append(val + " [" + value.ToString("X8") + "]");
                        }
                        else
                        {
                            SB.Append(val);
                        }
                    }
                    else if (FieldType == typeof(int))
                    {
                        int value = (int)Value;
                        string val = Utils.GetString(value);

                        if (!val.StartsWith("0x"))
                        {
                            SB.Append(val + " [" + value.ToString("X8") + "]");
                        }
                        else
                        {
                            SB.Append(val);
                        }
                    }
                    else if (FieldType == typeof(ulong)) SB.Append(((ulong)Value).ToString("X16"));
                    else if (FieldType == typeof(float)) SB.Append(((float)Value).ToString());
                    else if (FieldType == typeof(double)) SB.Append(((double)Value).ToString());
                    else SB.Append(Value.ToString());
                }
            }

            return SB.ToString();
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream copyOfStream = stream;
                stream = null;

                copyOfStream?.Close();
            }

            stream = null;
        }
    }
}
