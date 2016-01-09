using OfficeOpenXml;
using Scraper.Common;
using Scraper.Data.Common;
using Scraper.Data.RowManipulation;
using System.Collections.Generic;

namespace Scraper.Data.ExcellRowManipulation
{
    class SheetWrapper:IRowContainer
    {
        private int RowCursor;
        private Dictionary<string, int> ColumnMap { get; set; }
        private ExcelWorksheet Sheet { get; set; }
        public SheetWrapper(ExcelWorksheet sheet,bool revriteExising=false)
        {
            ColumnMap = new Dictionary<string, int>();
            Sheet = sheet;

            if (sheet.Dimension == null || revriteExising)
            {
                RowCursor = 2;
                return;
                
            }

            if (sheet.Dimension.Rows != 0)
            {
                RowCursor = sheet.Dimension.Rows + 1;
            }

            if (sheet.Dimension.Columns != 0)
            {
                for (int i = 1; i < sheet.Dimension.Columns; i++)
                {
                    ColumnMap.AddOrOvewrite(sheet.Cells[1, i].Value.ToString(), i);
                }
            }
        }

       

        public int GetOrCreateColumn(string externalColumnName)
        {
            var columnName = externalColumnName.ToColumnName();
            if (Sheet.Dimension == null)
            {
                Sheet.Cells[1, 1].Value = columnName;
                ColumnMap.AddOrOvewrite(columnName, 1);
                return 1;
            }

            if (ColumnMap.ContainsKey(columnName))
            {
                return ColumnMap[columnName];
            }

            int index = Sheet.Dimension.Columns + 1;
            ColumnMap.Add(columnName, index);
            Sheet.Cells[1, index].Value = columnName;
            return index;
        }
        
        public int AddRow(Row row)
        {
            foreach (var item in row.Columns)
            {
                Write(item.Key, item.Value, RowCursor);
            }
            RowCursor++;
            return RowCursor - 1;
        }

        public Row GetRow(int i)
        {
            Row row = new Row();
            for (int j = 1; j < Sheet.Dimension.Columns; j++)
            {
                row.Add(Sheet.Cells[1, j].Value.ToString(), Sheet.Cells[i, j].Value == null ? string.Empty : Sheet.Cells[i, j].Value.ToString());
            }
            return row;
        }

        public int RowCount
        {
            get { return Sheet.Dimension != null ? Sheet.Dimension.Rows : 0; }
        }


        private void Write(string columnName, string val, int row)
        {
            int index = GetOrCreateColumn(columnName);
            Sheet.Cells[row, index].Value = string.IsNullOrEmpty(val)?null:val;
        }

       public void AutoFitCOllumns()
        {
            Sheet.Cells.AutoFitColumns();
        }
    }
}
