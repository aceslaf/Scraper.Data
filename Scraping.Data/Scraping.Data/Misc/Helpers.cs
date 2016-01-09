
using OfficeOpenXml;
using Scraper.Data.DbRowManipulation;
using Scraper.Data.ExcellRowManipulation;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Scraper.Common
{
    public static class Helpers
    {
        public static string ConnectionString
        {
            get
            {
                //return ConfigurationManager.ConnectionStrings["hs4db"].ConnectionString);
                return "asd";
            }
        }

        public static ExcelPackage DownloadAndLoadAsExcel(this HttpClient client, string url)
        {
            var response = client.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }

            Stream stream = response.Content.ReadAsStreamAsync().Result;

            return new ExcelPackage(stream);
        }

        public static void DownloadExcelToTable(this HttpClient client, string url, string tableName)
        {
            var package = client.DownloadAndLoadAsExcel(url);
            SaveToTable(package, tableName);
        }

        public static void DownloadExcelToTable(this HttpClient client, string url, HttpContent postData, string tableName)
        {
            var response = client.PostAsync(url, postData).Result;

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return;
            }
            using (var package = new ExcelPackage(response.Content.ReadAsStreamAsync().Result))
            {
                SaveToTable(package, tableName);
            }
        }

        public static void SaveToTable(ExcelPackage package, string tableName)
        {
            //TODO aleksandar add saving of all the sheets.

            var sheet = new SheetWrapper(package.Workbook.Worksheets[1]);
            var dbTable = new DapperRowContainer(tableName, new SqlConnection(ConnectionString));
            for (int i = 2; i < sheet.RowCount; i++)
            {
                dbTable.AddRow(sheet.GetRow(i));
            }
        }


        public static ExcelPackage DownloadAndLoadAsExcel(this HttpClient client, string url, HttpContent postData)
        {
            var response = client.PostAsync(url, postData).Result;

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }

            Stream stream = response.Content.ReadAsStreamAsync().Result;

            return new ExcelPackage(stream);
        }
    }
}
