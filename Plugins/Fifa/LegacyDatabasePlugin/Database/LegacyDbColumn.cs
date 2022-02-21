namespace LegacyDatabasePlugin.Database
{
    public enum LegacyDbColumnType
    {
        String,
        Unk1,
        Unk2,
        Integer,
        Float,
        ShortCompressedString = 13,
        LongCompressedString
    }

    public class LegacyDbColumn
    {
        public string Name => name;
        public string ShortName => shortName;
        public LegacyDbColumnType Type => type;
        public bool IsKey => isKey;

        private string shortName;
        private string name;
        private LegacyDbColumnType type;
        private bool isKey;
        private LegacyDbTable table;

        public LegacyDbColumn(LegacyDbTable inTable, LegacyDbColumnType inType, string inShortName, string inName, bool key)
        {
            shortName = inShortName;
            name = (inName == "") ? inShortName : inName;
            type = inType;
            isKey = key;
            table = inTable;
        }
    }
}
