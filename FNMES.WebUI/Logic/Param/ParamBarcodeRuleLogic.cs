using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Base;
using System.Diagnostics.Metrics;
using System;
using System.Collections.Generic;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamBarcodeRuleLogic : BaseLogic
    {
        Dictionary<int, string> yearTable = new Dictionary<int, string>
        {
            { 2011, "1" },
            { 2012, "2" },
            { 2013, "3" },
            { 2014, "4" },
            { 2015, "5" },
            { 2016, "6" },
            { 2017, "7" },
            { 2018, "8" },
            { 2019, "9" },
            { 2020, "A" },
            { 2021, "B" },
            { 2022, "C" },
            { 2023, "D" },
            { 2024, "E" },
            { 2025, "F" },
            { 2026, "G" },
            { 2027, "H" },
            { 2028, "J" },
            { 2029, "K" },
            { 2030, "L" },
            { 2031, "M" },
            { 2032, "N" },
            { 2033, "P" },
            { 2034, "R" },
            { 2035, "S" },
            { 2036, "T" },
            { 2037, "V" },
            { 2038, "W" },
            { 2039, "X" },
            { 2040, "Y" },
            { 2041, "1" },
            { 2042, "2" },
            { 2043, "3" },
            { 2044, "4" },
            { 2045, "5" },
            { 2046, "6" },
            { 2047, "7" },
            { 2048, "8" },
            { 2049, "9" },
            { 2050, "A" },
        };

        Dictionary<int, string> monthTable = new Dictionary<int, string>
        {
            { 1, "1" },
            { 2, "2" },
            { 3, "3" },
            { 4, "4" },
            { 5, "5" },
            { 6, "6" },
            { 7, "7" },
            { 8, "8" },
            { 9, "9" },
            { 10, "A" },
            { 11, "B" },
            { 12, "C" },
            { 13, "D" },
            { 14, "E" },
            { 15, "F" },
            { 16, "G" },
            { 17, "H" },
            { 18, "J" },
            { 19, "K" },
            { 20, "L" },
            { 21, "M" },
            { 22, "N" },
            { 23, "P" },
            { 24, "R" },
            { 25, "S" },
            { 26, "T" },
            { 27, "V" },
            { 28, "W" },
            { 29, "X" },
            { 30, "Y" },
            { 31, "0" },
        };
        public bool GenBarcode(string configId,out string barcode)
        {
            try
            {
                var db = GetInstance(configId);
                ParamBarcodeRule items = new ParamBarcodeRule();
                var paramBarcodeRule = db.Queryable<ParamBarcodeRule>().First();
                var anotherDay = paramBarcodeRule.CreateTime.Date.AddHours(32);
                if (DateTime.Now > anotherDay)
                {
                    paramBarcodeRule.CreateTime = DateTime.Now;
                    paramBarcodeRule.SerialNumber = 0;
                    db.Updateable(paramBarcodeRule).ExecuteCommand();
                }
                paramBarcodeRule.SerialNumber++;
                barcode = paramBarcodeRule.SerialNumber.ToString();
                db.Updateable(paramBarcodeRule).ExecuteCommand();
                string yearCode = yearTable[DateTime.Now.Year];
                string monthCode = monthTable[DateTime.Now.Month];
                string dayCode = monthTable[DateTime.Now.Day];
                barcode = $"08IPB{paramBarcodeRule.StandardCode}{paramBarcodeRule.TraceInfoCode}{paramBarcodeRule.VendorAddress}{yearCode}{monthCode}{dayCode}{int.Parse(configId).ToString("D2")}{paramBarcodeRule.SerialNumber.ToString("D5")}";

                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                barcode = "";
                return false;
            }
        }
    }
}
