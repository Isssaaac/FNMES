using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace FNMES.Utility.Extension.SqlSugar
{
    public class ExtMethods
    {
        public static List<SqlFuncExternal> GetExpMethods
        {
            get
            {
                var expMethods = new List<SqlFuncExternal>();
                //abs
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Abs",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("ABS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("ABS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("abs({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("ABS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("ABS({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //sin
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Sin",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("SIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("SIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("sin({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("SIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("SIN({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //cos
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Cos",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("COS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("COS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("cos({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("COS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("COS({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //asin
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Asin",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("ASIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("ASIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("asin({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("ASIN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("ASIN({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //acos
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Acos",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("ACOS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("ACOS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("acos({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("ACOS({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("ACOS({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //tan
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Tan",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("TAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("TAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("tan({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("TAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("TAN({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //atan
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Atan",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("ATAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("ATAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("atan({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("ATAN({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("ATAN({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //atan2
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Atan2",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("ATAN2({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("ATAN({0}/{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("atan2({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("ATAN2({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("ATAN2({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //pow
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Pow",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("POW({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("POWER({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("power({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("POWER({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("POWER({0},{1})", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //sqrt
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Sqrt",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("SQRT({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("SQRT({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("sqrt({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("SQRT({0})", expInfo.Args[0].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("SQRT({0})", expInfo.Args[0].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //GetDistance
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "GetDistance",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("((6378.138 * 2 * ASIN(POW(SIN(({0} * PI() / 180 - {2} * PI() / 180) / 2), 2) + COS({0} * PI() / 180) * COS({2} * PI() / 180) * POW(SIN(({1} * PI() / 180 - {3} * PI() / 180) / 2), 2))) * 1000)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName, expInfo.Args[2].MemberName, expInfo.Args[3].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("((6378.138 * 2 * ASIN(POWER(SIN(({0} * PI() / 180 - {2} * PI() / 180) / 2), 2) + COS({0} * PI() / 180) * COS({2} * PI() / 180) * POWER(SIN(({1} * PI() / 180 - {3} * PI() / 180) / 2), 2))) * 1000)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName, expInfo.Args[2].MemberName, expInfo.Args[3].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("((6378.138 * 2 * ASIN(POWER(SIN(({0} * PI() / 180 - {2} * PI() / 180) / 2), 2) + COS({0} * PI() / 180) * COS({2} * PI() / 180) * POWER(SIN(({1} * PI() / 180 - {3} * PI() / 180) / 2), 2))) * 1000)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName, expInfo.Args[2].MemberName, expInfo.Args[3].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("((6378.138 * 2 * ASIN(POWER(SIN(({0} * PI() / 180 - {2} * PI() / 180) / 2), 2) + COS({0} * PI() / 180) * COS({2} * PI() / 180) * POWER(SIN(({1} * PI() / 180 - {3} * PI() / 180) / 2), 2))) * 1000)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName, expInfo.Args[2].MemberName, expInfo.Args[3].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("((6378.138 * 2 * ASIN(POWER(SIN(({0} * PI() / 180 - {2} * PI() / 180) / 2), 2) + COS({0} * PI() / 180) * COS({2} * PI() / 180) * POWER(SIN(({1} * PI() / 180 - {3} * PI() / 180) / 2), 2))) * 1000)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName, expInfo.Args[2].MemberName, expInfo.Args[3].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //Bd09ToGcj02Lat
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Bd09ToGcj02Lat",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("((SQRT( ({0} - 0.0065) *  ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) * (3.14159265358979324 * 3000.0 / 180.0))) * SIN(ATAN2(({1} - 0.006),  ({0} - 0.0065)) - 0.000003 * COS( ({0} - 0.0065) * (3.14159265358979324 * 3000.0 / 180.0))))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("((SQRT( ({0} - 0.0065) *  ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) * (3.14159265358979324 * 3000.0 / 180.0))) * SIN(ATAN(({1} - 0.006)/({0} - 0.0065)) - 0.000003 * COS( ({0} - 0.0065) * (3.14159265358979324 * 3000.0 / 180.0))))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("((SQRT( ({0} - 0.0065) *  ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) * (3.14159265358979324 * 3000.0 / 180.0))) * SIN(ATAN2(({1} - 0.006),  ({0} - 0.0065)) - 0.000003 * COS( ({0} - 0.0065) * (3.14159265358979324 * 3000.0 / 180.0))))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("((SQRT( ({0} - 0.0065) *  ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) * (3.14159265358979324 * 3000.0 / 180.0))) * SIN(ATAN2(({1} - 0.006),  ({0} - 0.0065)) - 0.000003 * COS( ({0} - 0.0065) * (3.14159265358979324 * 3000.0 / 180.0))))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("((SQRT( ({0} - 0.0065) *  ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) * (3.14159265358979324 * 3000.0 / 180.0))) * SIN(ATAN2(({1} - 0.006),  ({0} - 0.0065)) - 0.000003 * COS( ({0} - 0.0065) * (3.14159265358979324 * 3000.0 / 180.0))))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //Bd09ToGcj02Lon
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Bd09ToGcj02Lon",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("(SQRT(({0} - 0.0065) * ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) *  (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2(({1} - 0.006), ({0} - 0.0065)) - 0.000003 * COS(({0} - 0.0065) *  (3.14159265358979324 * 3000.0 / 180.0)))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("(SQRT(({0} - 0.0065) * ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) *  (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN(({1} - 0.006)/({0} - 0.0065)) - 0.000003 * COS(({0} - 0.0065) *  (3.14159265358979324 * 3000.0 / 180.0)))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("(SQRT(({0} - 0.0065) * ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) *  (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2(({1} - 0.006), ({0} - 0.0065)) - 0.000003 * COS(({0} - 0.0065) *  (3.14159265358979324 * 3000.0 / 180.0)))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("(SQRT(({0} - 0.0065) * ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) *  (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2(({1} - 0.006), ({0} - 0.0065)) - 0.000003 * COS(({0} - 0.0065) *  (3.14159265358979324 * 3000.0 / 180.0)))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("(SQRT(({0} - 0.0065) * ({0} - 0.0065) + ({1} - 0.006) * ({1} - 0.006)) - 0.00002 * SIN(({1} - 0.006) *  (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2(({1} - 0.006), ({0} - 0.0065)) - 0.000003 * COS(({0} - 0.0065) *  (3.14159265358979324 * 3000.0 / 180.0)))", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //Gcj02ToBd09Lat
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Gcj02ToBd09Lat",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * SIN( ATAN2({1},{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.006)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * SIN( ATAN({1}/{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.006)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * SIN( ATAN2({1},{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.006)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * SIN( ATAN2({1},{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.006)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * SIN( ATAN2({1},{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.006)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                //Gcj02ToBd09Lon
                expMethods.Add(new SqlFuncExternal()
                {
                    UniqueMethodName = "Gcj02ToBd09Lon",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2({1}, {0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.0065)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.SqlServer)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN({1}/{0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.0065)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.PostgreSQL)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2({1}, {0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.0065)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Oracle)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2({1}, {0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.0065)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else if (dbType == DbType.Sqlite)
                            return string.Format("((SQRT({0} * {0} + {1} * {1}) - 0.00002 * SIN({1} * (3.14159265358979324 * 3000.0 / 180.0))) * COS(ATAN2({1}, {0}) - 0.000003 * COS({0} * (3.14159265358979324 * 3000.0 / 180.0)))+0.0065)", expInfo.Args[0].MemberName, expInfo.Args[1].MemberName);
                        else
                            throw new Exception("未实现");
                    }
                });
                return expMethods;
            }

        }
    }
}
