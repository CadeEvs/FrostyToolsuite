using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaPlugin
{
    public class CompiledLuaResource : Resource
    {
        public ushort Unknown1;
        public ushort Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint ParameterCount;
        public uint BytecodeLength;
        public string EntrypointName; // Null-terminated
        public string Parameters; // Null-terminated, separated by ',' or ', '
        public byte[] Bytecode;

        public CompiledLuaResource()
        {
        }

        /// <summary>
        /// Loads a Frostbite compiled Lua resource from a stream.
        /// </summary>
        /// <param name="s"></param>
        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            Unknown1 = reader.ReadUShort();
            Unknown2 = reader.ReadUShort();
            Unknown3 = reader.ReadUInt();
            Unknown4 = reader.ReadUInt();
            Unknown5 = reader.ReadUInt();
            ParameterCount = reader.ReadUInt();
            BytecodeLength = reader.ReadUInt();
            EntrypointName = reader.ReadNullTerminatedString();
            Parameters = reader.ReadNullTerminatedString();
            Bytecode = reader.ReadBytes((int)BytecodeLength);
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(ParameterCount);
                writer.Write(BytecodeLength);
                writer.Write(Encoding.ASCII.GetBytes(EntrypointName));
                writer.Write((byte)0);
                writer.Write(Encoding.ASCII.GetBytes(Parameters));
                writer.Write((byte)0);
                writer.Write(Bytecode);

                return writer.ToByteArray();
            }
        }

        #region --Decompilation--
        /// <summary>
        /// Decompiles and prettifies the stored bytecode.
        /// </summary>
        /// <returns></returns>
        public string DecompileBytecode(ILogger logger = null)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(Bytecode)))
            {
                reader.Position = 0x0C;
                int codeSize = reader.ReadInt();
                string code = reader.ReadSizedString(codeSize);

                string[] lines = code.Split('\n');

                code = "";
                foreach (string line in lines)
                    code += line.Trim(' ') + "\n";

                return code.TrimEnd();
            }
        }
        #endregion

        #region --Recompilation--
        private class LuaDec
        {
            [DllImport("thirdparty/luacmp.dll", EntryPoint = "Compile")]
            public static extern bool Compile(
                [MarshalAs(UnmanagedType.LPStr)]
                string code,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
                out byte[] data,
                out int bytecount,
                [MarshalAs(UnmanagedType.LPStr)]
                out string errors
                );
        }

        public void CompileSource(string[] source, ILogger logger = null)
        {
            string totalSource = "";
            foreach (string line in source)
                totalSource += line + "\n";

            if (!LuaDec.Compile(totalSource, out byte[] data, out int bytecount, out string errors))
            {
                logger?.Log("Lua compilation error:");
                logger?.Log(errors);
                return;
            }
            
            logger?.Log("Lua compilation successful");

            Bytecode = data;
            BytecodeLength = (uint)bytecount;
        }
        #endregion
    }
}
