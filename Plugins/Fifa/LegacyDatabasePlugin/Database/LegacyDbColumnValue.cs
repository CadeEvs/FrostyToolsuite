namespace LegacyDatabasePlugin.Database
{
    public class LegacyDbColumnValue
    {
        public LegacyDbColumn Column => column;
        public object Value { get => columnValue; set => columnValue = value; }

        private LegacyDbColumn column;
        private object columnValue;

        public LegacyDbColumnValue(LegacyDbColumn inColumn, object inValue)
        {
            column = inColumn;
            columnValue = inValue;
        }
    }
}
