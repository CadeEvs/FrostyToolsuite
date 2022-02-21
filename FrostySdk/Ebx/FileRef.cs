namespace FrostySdk.Ebx
{
    public struct FileRef
    {
        private string fileName;

        public FileRef(string value)
        {
            fileName = value;
        }

        public static implicit operator string(FileRef value) => value.fileName;

        public static implicit operator FileRef(string value) => new FileRef(value);

        public override string ToString() => "FileRef '" + fileName + "'";
    }
}
