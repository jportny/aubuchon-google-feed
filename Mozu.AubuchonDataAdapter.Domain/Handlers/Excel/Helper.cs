using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OfficeOpenXml;
using System.Reflection;
using Newtonsoft.Json;


namespace Mozu.AubuchonDataAdapter.Domain.Handlers.Excel
{
    public class Helper
    {
        public static string STORE_DETAILS_WS_NAME= "Store Details";
        public static string STORE_SERVICES_WS_NAME = "Special Services";
        public static string[] STORE_DETAILS_HEADER_XL = new string[]
        { 
            "Mozu Location Code", 
            "Location Name",
            "Manager Name", 
            "Manager Image", 
            "Store Front Image", 
            "Business Development Representative Name", 
            "BDR Image" 
        };
        public static List<ExcelHeader> GetHeaders(string fileName) {
                
           var assemblyUri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
           var templatePath = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(assemblyUri.Path)), "Handlers\\Excel", "Templates");

            return JsonConvert.DeserializeObject<List<ExcelHeader>>(File.ReadAllText(Path.Combine(templatePath, fileName)));
        }
        public static DataTable GetDataTable(string file, string workSheetName, string primaryColumnName = null)
        {
            var excelFile = new FileInfo(file);

            var tbl = new DataTable();
            using (var package = new ExcelPackage(excelFile))
            {
                var worksheet = package.Workbook.Worksheets.SingleOrDefault(x => x.Name.Equals(workSheetName));


                if (worksheet == null)
                    return new DataTable();

                var primaryColumn = new List<DataColumn>();
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    if (string.IsNullOrEmpty(firstRowCell.Text)) continue;
                    var dataColumn = new DataColumn(firstRowCell.Text);
                    if (!string.IsNullOrEmpty(primaryColumnName) && primaryColumnName.Split(',').ToList().Contains(firstRowCell.Text))
                        primaryColumn.Add(dataColumn);
                    tbl.Columns.Add(dataColumn);
                }
                if (!string.IsNullOrEmpty(primaryColumnName))
                    tbl.PrimaryKey = primaryColumn.ToArray();

                var startRow = 2;
                for (var rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {

                    var wsRow = worksheet.Cells[rowNum, 1, rowNum, tbl.Columns.Count];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        cell.Calculate();
                        row[cell.Start.Column - 1] = cell.Text;
                    }

                    try
                    {
                        tbl.Rows.Add(row);
                    }
                    catch (Exception exc)
                    {
                        //if (logHandler != null)
                        //  logHandler.Log(exc.Message, true, exc);
                    }

                }
            }
            return tbl;
        }
    }
}
