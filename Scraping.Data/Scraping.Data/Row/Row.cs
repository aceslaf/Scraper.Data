using Scraper.Common;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Data.RowManipulation
{
    public class Row
    {
        private Dictionary<string,string> _columns;
        public List<KeyValuePair<string, string>> Columns {
            get {
                return _columns.ToList();
            }
        }
        public Row()
        {
            _columns = new Dictionary<string, string>();
        }

        public Row(string key, string value)
        {
            _columns = new Dictionary<string, string>();
            _columns.AddOrOvewrite(key, value);
        }

        public Row Add(string key, string value)
        {
            _columns.AddOrOvewrite(key, value);
            return this;
        }

        public Row Add(KeyValuePair<string, string> keyVal)
        {
            return Add(keyVal.Key, keyVal.Value);
        }

        public Row Add(List<KeyValuePair<string, string>> newColumns)
        {
            newColumns.ForEach(x => Add(x.Key,x.Value));
            return this;
        }

        public Row Add(Row row)
        {
            foreach (var item in row.Columns)
            {
                Add(item.Key, item.Value);
            }
            return this;
        }

        public string ColumnValue(string key)
        {
            string str;
            if (_columns.TryGetValue(key, out str))
            {
                return str;
            }

            return null;
        }

        public bool IsEmpty
        {
            get { return _columns.Count <= 0; }
        }
    }
}
