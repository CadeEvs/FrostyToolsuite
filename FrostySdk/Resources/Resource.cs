using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using FrostySdk.Managers.Entries;

namespace FrostySdk.Resources
{
    public class Resource
    {
        public ulong ResourceId => resRid;
        public byte[] ResourceMeta => resMeta;

        protected byte[] resMeta;
        protected ulong resRid;

        public Resource()
        {
        }

        public virtual void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            resMeta = entry.ResMeta;
            resRid = entry.ResRid;

        }

        /// <summary>
        /// Saves the resource as a specialized ModifiedResource object
        /// </summary>
        public virtual ModifiedResource SaveModifiedResource() => null;

        /// <summary>
        /// Saves the resource as a byte array
        /// </summary>
        public virtual byte[] SaveBytes() => null;
    }

    public class ModifiedResource
    {
        public ModifiedResource()
        {
        }

        public byte[] Save()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteNullTerminatedString(GetType().AssemblyQualifiedName);
                using (NativeWriter subWriter = new NativeWriter(new MemoryStream()))
                {
                    SaveInternal(subWriter);
                    writer.Write(subWriter.ToByteArray());
                }

                return writer.ToByteArray();;
            }
        }

        public virtual void SaveInternal(NativeWriter writer)
        {
        }

        public static ModifiedResource Read(byte[] buffer)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
            {
                string type = FixupTypes(reader.ReadNullTerminatedString());
                using (NativeReader subReader = new NativeReader(new MemoryStream(reader.ReadToEnd())))
                {
                    ModifiedResource md = (ModifiedResource)Activator.CreateInstance(Type.GetType(type));
                    md.ReadInternal(subReader);

                    return md;
                }
            }
        }

        public virtual void ReadInternal(NativeReader reader)
        {
        }

        private static string FixupTypes(string type)
        {
            return type == "ModifiedShaderBlockDepot" ? "MeshSetPlugin.Resources.ModifiedShaderBlockDepot, MeshSetPlugin" : type;
        }
    }
}
