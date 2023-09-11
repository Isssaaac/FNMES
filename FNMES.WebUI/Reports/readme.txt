该目录存放报表文件FastReport.OpenSource 的frx文件

利用FastReport.OpenSource生成Pdf文件
再通过命令行PDFtoPrinter实现打印功能

PDFtoPrinter工具，详见：http://www.columbia.edu/~em36/pdftoprinter.html 或者见目录下PDFtoPrinter.pdf文件

要将 PDF 文件打印到默认的 Windows 打印机，请使用以下命令:
PDFtoPrinter.exe filename.pdf

要打印到特定打印机，请在引号中添加打印机名称:
PDFtoPrinter.exe filename.pdf "Name of Printer"

如果要打印到网络打印机，请使用出现在 Windows 打印对话框中的名称，如下所示(并注意名称开头的两个反斜杠和服务器名称后的单个反斜杠):
PDFtoPrinter.exe filename.pdf "\\SERVER\PrinterName"

FastReport导出pdf如下：

Report report = new Report();
report.Load(path);//加载frx文件
DataTable dt = ？ ;//数据源
dt.TableName = "data";
report.SetParameterValue("AAA", "123456");//设置报表参数
report.RegisterData(dt, "data");        //注册数据
report.Prepare();                       //准备数据
PDFSimpleExport export = new PDFSimpleExport();//导出文件
report.Export(export, "d:/my.pdf");
report.Dispose();


frx报表文件编辑，请使用Other下的FastReport.Community.2022.2.0.zip