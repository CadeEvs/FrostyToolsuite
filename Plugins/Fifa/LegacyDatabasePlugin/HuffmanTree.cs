using FrostySdk.IO;
using System.Text;

namespace LegacyDatabasePlugin
{
    public class HuffmannTree
    {
        public int NNodes
        {
            get => this.m_NNodes;
            set => this.m_NNodes = value;
        }
        public int Size => this.m_NNodes * 4;

        public HuffmannTree(int nNodes)
        {
            this.m_NNodes = nNodes;
            this.m_Child = new byte[this.m_NNodes, 2];
            this.m_Leaf = new byte[this.m_NNodes, 2];
            for (int i = 0; i < this.m_NNodes; i++)
            {
                this.m_Child[i, 0] = 0;
                this.m_Child[i, 1] = 0;
                this.m_Leaf[i, 0] = 0;
                this.m_Leaf[i, 1] = 0;
            }
        }

        public void Load(NativeReader r)
        {
            for (int i = 0; i < this.m_NNodes; i++)
            {
                this.m_Child[i, 0] = r.ReadByte();
                this.m_Leaf[i, 0] = r.ReadByte();
                this.m_Child[i, 1] = r.ReadByte();
                this.m_Leaf[i, 1] = r.ReadByte();
            }
            this.BuildEncodingTable();
        }

        private void BuildEncodingTable()
        {
            ushort[] array = new ushort[this.m_NNodes];
            byte[] array2 = new byte[this.m_NNodes];
            for (int i = 0; i < this.m_NNodes; i++)
            {
                ushort num = (ushort)(array[i] * 2);
                byte b = (byte)(array2[i] + 1);
                byte b2 = this.m_Child[i, 0];
                if (b2 != 0)
                {
                    array[(int)b2] = num;
                    array2[(int)b2] = b;
                }
                else
                {
                    byte b3 = this.m_Leaf[i, 0];
                    this.m_EncodingValue[(int)b3] = num;
                    this.m_nBitsForEncoding[(int)b3] = b;
                }
                num += 1;
                byte b4 = this.m_Child[i, 1];
                if (b4 != 0)
                {
                    array[(int)b4] = num;
                    array2[(int)b4] = b;
                }
                else
                {
                    byte b5 = this.m_Leaf[i, 1];
                    this.m_EncodingValue[(int)b5] = num;
                    this.m_nBitsForEncoding[(int)b5] = b;
                }
            }
        }

        public string ReadString(NativeReader r, int outputLength)
        {
            int num = 0;
            int num2 = 0;
            if (outputLength <= 0)
            {
                return string.Empty;
            }
            byte[] array;
            if (this.m_NNodes == 0)
            {
                array = r.ReadBytes(outputLength);
            }
            else
            {
                array = new byte[outputLength];
                do
                {
                    byte b = r.ReadByte();
                    for (int i = 7; i >= 0; i--)
                    {
                        int num3 = b >> i & 1;
                        int num4 = (int)this.m_Child[num2, num3];
                        if (num4 == 0)
                        {
                            array[num++] = this.m_Leaf[num2, num3];
                            if (num == outputLength)
                            {
                                break;
                            }
                            num2 = 0;
                        }
                        else
                        {
                            num2 = num4;
                        }
                    }
                }
                while (num < outputLength);
            }
            return Encoding.UTF8.GetString(array);
        }

        private byte[,] m_Child;
        private byte[,] m_Leaf;
        private ushort[] m_EncodingValue = new ushort[256];
        private byte[] m_nBitsForEncoding = new byte[256];
        private int m_NNodes;
    }
}
