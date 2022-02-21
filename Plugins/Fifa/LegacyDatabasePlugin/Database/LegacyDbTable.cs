using System.Collections.Generic;

namespace LegacyDatabasePlugin.Database
{
    public class LegacyDbTable
    {
        public string Name => name;
        public string ShortName => shortName;
        public IEnumerable<LegacyDbColumn> Columns => columns;
        public IEnumerable<LegacyDbRow> Rows => rows;

        private string shortName;
        private string name;
        private List<LegacyDbColumn> columns = new List<LegacyDbColumn>();
        private List<LegacyDbRow> rows = new List<LegacyDbRow>();

        public LegacyDbRow this[int key] => rows[key];

        public LegacyDbTable(string inShortName, string inName = "")
        {
            shortName = inShortName;
            name = (inName == "") ? shortName : inName;
        }

        public void AddColumn(LegacyDbColumn column)
        {
            columns.Add(column);
        }

        public void AddRow(LegacyDbRow row)
        {
            rows.Add(row);
        }
    }
}
