using FNMES.Utility.Core;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace FNMES.Utility.Files
{
    public class ExcelUtils
    {
        public static string ExcelContentType
        {
            get
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
        }
        public static ExcelPackage ExportSheet<T>(ExcelPackage package, List<T> data, Dictionary<string, string> headDict, string sheetName = "", bool showSrNo = false)
        {
            return ExportSheet(package, data.ToDataTable<T>(), headDict, sheetName, showSrNo);
        }

        public static ExcelPackage ExportSheet(ExcelPackage package, DataTable dt, Dictionary<string, string> headDict, string sheetName = "", bool showSrNo = false)
        {
            List<string> keyList = new List<string>();
            if (showSrNo)
            {
                keyList.Add("RowNum");
                dt.Columns.Add("RowNum");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["RowNum"] = i + 1;
                }
            }
            //通过键的集合取
            foreach (string key in headDict.Keys)
            {
                keyList.Add(key);
            }
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(sheetName.IsNullOrEmpty() ? "Sheet1" : sheetName);
            if (showSrNo)
            {
                headDict.Add("RowNum", "序号");
            }
            for (int i = 0; i < keyList.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = headDict[keyList[i]];
            }
            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < keyList.Count; j++)
                    {

                        sheet.Cells[i + 2, j + 1].Value = dt.Rows[i][keyList[j]].ToString();
                    }
                }
            }
            ExcelRange cells = sheet.Cells[1, 1, 1 + dt.Rows.Count, keyList.Count];
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
            cells.AutoFitColumns();//自适应列宽
            return package;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="headDict"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static byte[] ExportExcel<T>(List<T> data, Dictionary<string, string> headDict, string sheetName = "", bool showSrNo = false)
        {
            return ExportExcel(data.ToDataTable<T>(), headDict, sheetName, showSrNo);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataTable">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">//是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        public static byte[] ExportExcel(DataTable dt, Dictionary<string, string> headDict, string sheetName = "", bool showSrNo = false)
        {
            Dictionary<string, Type> typeDict = new Dictionary<string, Type>();
            foreach (DataColumn column in dt.Columns)
            {
                typeDict.Add(column.ColumnName, column.DataType);
            }
            typeDict.Add("RowNum", typeof(int));
            byte[] result = null;
            List<string> keyList = new List<string>();
            if (showSrNo)
            {
                keyList.Add("RowNum");
                dt.Columns.Add("RowNum", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["RowNum"] = i + 1;
                }

            }
            //通过键的集合取
            foreach (string key in headDict.Keys)
            {
                keyList.Add(key);
            }
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add(sheetName.IsNullOrEmpty() ? "Sheet1" : sheetName);
                if (showSrNo)
                {
                    headDict.Add("RowNum", "序号");
                }
                for (int i = 0; i < keyList.Count; i++)
                {
                    sheet.Cells[1, i + 1].Value = headDict[keyList[i]];//第一行 某列 表头
                    Type type = typeDict[keyList[i]];//得到类型
                    if (type == typeof(int))
                    {
                        sheet.Column(i + 1).Style.Numberformat.Format = "#,##0";
                    }
                    else if (type == typeof(float) || type == typeof(double))
                    {
                        sheet.Column(i + 1).Style.Numberformat.Format = "#,##0.00";
                    }
                    else if (type == typeof(DateTime))
                    {
                        sheet.Column(i + 1).Style.Numberformat.Format = "yyyy/mm/dd hh:mm:ss";
                    }

                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < keyList.Count; j++)
                        {
                            sheet.Cells[i + 2, j + 1].Value = dt.Rows[i][keyList[j]];
                        }
                    }
                }
                ExcelRange cells = sheet.Cells[1, 1, 1 + dt.Rows.Count, keyList.Count];
                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                cells.AutoFitColumns();//自适应列宽
                result = package.GetAsByteArray();
            }
            return result;
        }


        public static DataTable ImportExcel(string filePath)
        {
            DataTable dt = new DataTable();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.Rows;
                int ColCount = sheet.Dimension.Columns;
                for (int i = 1; i <= ColCount; i++)
                {
                    DataColumn datacolum = new DataColumn(Null2Empty(sheet.Cells[1, i].Value));
                    dt.Columns.Add(datacolum);
                }
                //填充内容
                for (int i = 2; i <= rowCount; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < ColCount; j++)
                    {
                        row[j] = Null2Empty(sheet.Cells[i, j + 1].Value);
                    }
                    dt.Rows.Add(row); //把每行追加到DataTable
                }
            }
            return dt;
        }

        public static DataSet ImportExcel2DataSet(string filePath)
        {
            DataSet ds = new DataSet();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                foreach (ExcelWorksheet sheet in package.Workbook.Worksheets)
                {
                    DataTable dt = new DataTable(sheet.Name);
                    int rowCount = sheet.Dimension.Rows;
                    int ColCount = sheet.Dimension.Columns;
                    for (int i = 1; i <= ColCount; i++)
                    {
                        DataColumn datacolum = new DataColumn(Null2Empty(sheet.Cells[1, i].Value));
                        dt.Columns.Add(datacolum);
                    }
                    //填充内容
                    for (int i = 2; i <= rowCount; i++)
                    {
                        DataRow row = dt.NewRow();
                        for (int j = 0; j < ColCount; j++)
                        {
                            row[j] = Null2Empty(sheet.Cells[i, j + 1].Value);
                        }
                        dt.Rows.Add(row); //把每行追加到DataTable
                    }
                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }

        private static string Null2Empty(object value)
        {
            if (value == null)
                return string.Empty;
            try
            {
                return Convert.ToString(value);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
