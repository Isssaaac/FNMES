using SqlSugar;
using static System.Net.Mime.MediaTypeNames;

namespace 测试
{
    public partial class Form1 : Form
    {
        public int successCount = 0;
        public int failCount = 0;
        public bool stopFlag = false;
        public delegate void UpdateRichTextBoxDelegate(string text);
        public delegate void UpdateNumberDelegate(bool b);


        public Form1()
        {
            InitializeComponent();
            success.Text = "0";
            fail.Text = "0";
            stopFlag = false;
           
        }


        private void UpdateRichTextBox(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new UpdateRichTextBoxDelegate(UpdateRichTextBox), text);
            }
            else
            {
                richTextBox1.AppendText(text);
                richTextBox1.ScrollToCaret();
            }
        }

        private void UpdateNumber(bool v)
        {
            if (success.InvokeRequired)
            {
                success.Invoke(new UpdateNumberDelegate(UpdateNumber), v);
            }
            else
            {
                if (v)
                {
                    int old = int.Parse(success.Text);
                    old++;
                    success.Text = old.ToString();
                }
                else
                {
                    int old = int.Parse(fail.Text);
                    old++;
                    fail.Text = old.ToString();
                }

            }
        }


        private void LogMessage(string message)
        {
            // 模拟一些耗时的操作

            // 生成日志消息
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";

            // 使用委托在UI线程上更新RichTextBox
            UpdateRichTextBox(logMessage);
        }
        private void LogNumber(bool v)
        {
            // 使用委托在UI线程上更新RichTextBox
            UpdateNumber(v);
        }





        private void start_Click(object sender, EventArgs e)
        {
            // 启动20个并行任务
            for (int i = 1; i <= 20; i++)
            {
                int taskNumber = i * 100; // Capture the current value of i for the lambda expression
                Task.Run(() => SimulateTaskExecution(taskNumber));
            }
        }


        // 定义要执行的任务
        private void SimulateTaskExecution(int taskNumber)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                //准备添加分表分库
                ConfigId = "1",
                ConnectionString = "Server=192.168.68.200;Database=Test;User ID=sa;Password=ABCabc123r;Trusted_Connection=False;",
                IsAutoCloseConnection = true,
                DbType = DbType.SqlServer,
                MoreSettings = new ConnMoreSettings()

                {
                    DisableNvarchar = true
                },
                SlaveConnectionConfigs = new List<SlaveConnectionConfig>()
                {
                    new SlaveConnectionConfig(){
                        ConnectionString =  "Server=192.168.68.200;Database=Test;User ID=sa;Password=ABCabc123r;Trusted_Connection=False;;ApplicationIntent=READONLY;",
                        HitRate = 10,
                    }
                },
                //设置codefirst非空值判断
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        //// int?  decimal?这种 isnullable=true
                        if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }
                        //高版C#写法 支持string?和string  
                        //if (new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                        //{
                        //    p.IsNullable = true;
                        //}
                    }
                }
            }, db =>
            {
                db.Aop.DataExecuting = (oldValue, entityInfo) =>
                {
                    //var httpcontext = ServiceLocator.Instance.GetService<IHttpContextAccessor>().HttpContext;
                    switch (entityInfo.OperationType)
                    {
                        case DataFilterType.InsertByObject:
                            if (entityInfo.PropertyName == "CreateUser")
                            {
                                //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                            }
                            else if (entityInfo.PropertyName == "CreateTime")
                            {
                                //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["TenantId"].ToString()));
                            }
                            else if (entityInfo.PropertyName == "ModifyUser")
                            {
                                //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                            }
                            else if (entityInfo.PropertyName == "ModifyTime")
                            {
                                //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["TenantId"].ToString()));
                            }
                            break;
                        case DataFilterType.UpdateByObject:
                            if (entityInfo.PropertyName == "ModifyUser")
                            {
                                //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                            }
                            else if (entityInfo.PropertyName == "ModifyTime")
                            {
                                //entityInfo.SetValue(DateTime.Now);
                            }
                            break;
                    }

                };
                db.Aop.OnLogExecuting = (s, p) =>
                {
                    //if (GobalModel.SqlLogEnable)
                    //{
                    //    var _logger = ServiceLocator.Instance?.GetRequiredService<ILogger<SqlSugarClient>>();
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("执行SQL:" + s.ToString());
                    //foreach (var i in p)
                    //{
                    //    sb.Append($"\r\n参数:{i.ParameterName},参数值:{i.Value}");
                    //}
                    //Console.WriteLine(sb.ToString());
                    //    _logger?.LogInformation(sb.ToString());
                    //}
                };
            });
            string message = $"Task {taskNumber} started.";
            LogMessage(message);
            while (!stopFlag)
            {
                taskNumber++;
                Test test = new Test()
                {
                    Id = taskNumber.ToString()
                };
                db.Insertable<Test>(test).ExecuteCommand();
                Thread.Sleep((int)delay.Value);
                int v = db.SlaveQueryable<Test>().Where(it => it.Id == taskNumber.ToString()).Count();
                LogNumber(v == 0);
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            stopFlag = true;
        }
    }



    [SugarTable("test")]
    public class Test
    {
        [SugarColumn(ColumnName = "id")]
        public string? Id { get; set; }
        [SugarColumn(ColumnName = "name")]
        public string? Name { get; set; }
    }

}