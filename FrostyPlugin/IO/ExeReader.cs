namespace Frosty.Core.IO
{
    public class ExeReader : MemoryReader
    {
        public override long Position
        {
            get => position;
            set => position = theExe.getOffset(value);
        }

        private readonly Executable theExe;
        public ExeReader(Executable exe)
        {
            theExe = exe;
        }

        public override void Dispose()
        {
            theExe.Dispose();
        }

        protected override void FillBuffer(int numBytes)
        {
            theExe.getBytes(position, buffer, numBytes);
            position += numBytes;
        }
    }
}
