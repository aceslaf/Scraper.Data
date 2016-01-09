
namespace Scraper.Data.RowManipulation
{
    interface IRowContainer
    {
        int RowCount { get; }
        int GetOrCreateColumn(string columnName);

       int AddRow(Row row);

       Row GetRow(int i);
    }
}
