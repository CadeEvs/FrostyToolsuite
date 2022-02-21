using System;
using System.Collections.Generic;

namespace LegacyDatabasePlugin.Database
{
    public class LegacyDbRow
    {
        public IEnumerable<LegacyDbColumnValue> Values => values;
        private List<LegacyDbColumnValue> values = new List<LegacyDbColumnValue>();

        public object this[string column]
        {
            get
            {
                foreach (LegacyDbColumnValue colValue in values)
                {
                    if (colValue.Column.Name.Equals(column, StringComparison.OrdinalIgnoreCase))
                        return colValue.Value;
                }
                return null;
            }
        }

        public object this[int idx] => values[idx].Value;

        public LegacyDbRow(LegacyDbTable table)
        {
            foreach (LegacyDbColumn column in table.Columns)
                values.Add(new LegacyDbColumnValue(column, null));
        }

        public void SetValue(LegacyDbColumn column, object value)
        {
            foreach (LegacyDbColumnValue colValue in values)
            {
                if (colValue.Column == column)
                {
                    colValue.Value = value;
                    break;
                }
            }
        }
    }
}
