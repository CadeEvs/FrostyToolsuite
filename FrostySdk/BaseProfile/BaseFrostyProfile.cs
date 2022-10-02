using FrostySdk.BaseProfile;
using FrostySdk.Interfaces;
using System;

namespace FrostySdk
{
    public class BaseFrostyProfile : IProfile
    {
        public Type BinarySbReaderType => typeof(BaseBinarySbReader);
        public Type BinarySbWriterType => typeof(BaseBinarySbWriter);
        public Type CompressionUtilsType => typeof(BaseCompressionUtils);

        public IBinarySbReader GetBinarySbReader() { return (IBinarySbReader)Activator.CreateInstance(BinarySbReaderType); }
        public IBinarySbWriter GetBinarySbWriter() { return (IBinarySbWriter)Activator.CreateInstance(BinarySbWriterType); }
        public ICompressionUtils GetCompressionUtils() { return (ICompressionUtils)Activator.CreateInstance(CompressionUtilsType); }

        public Profile CreateProfile()
        {
            return new Profile();
        }
    }
}
