using System;
using System.Collections.Generic;

namespace LegacyDatabasePlugin.Database
{
    public class LegacyDb
    {
        public IEnumerable<LegacyDbTable> Tables => tables;
        private List<LegacyDbTable> tables = new List<LegacyDbTable>();

        public LegacyDbTable this[string tableName]
        {
            get
            {
                foreach (LegacyDbTable table in tables)
                {
                    if (table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        return table;
                }
                return null;
            }
        }

        public LegacyDbTable this[int idx] => tables[idx];

        public LegacyDb()
        {
        }

        public void AddTable(LegacyDbTable table)
        {
            tables.Add(table);
        }
    }
}
