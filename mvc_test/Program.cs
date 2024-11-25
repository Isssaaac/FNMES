
using CCS.WebUI;
using FNMES.Entity.DTO.ApiData;
using FNMES.Utility.Core;
using FNMES.Utility.Files;
using FNMES.Utility.Network;
using FNMES.Utility.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

String.Empty.ToInt32();



/*string filePath = "E:\\FNMES\\FNMES.WebUI\\wwwroot\\Content\\PDFjs\\web\\aaa.pdf"; // 本地文件路径

try
{
    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
        byte[] buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, buffer.Length);

        // 进一步处理文件流，例如可以上传到FTP服务器或进行其他操作
        // 例如，使用FluentFTP上传文件到FTP服务器
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

stopwatch.Stop();*/
/*List<string> files = new List<string>() { 
"aaa.pdf","aaa1.pdf"};

FtpHelper ftpHelper = new FtpHelper("172.16.1.21", "ftp", "gd123");

files.ForEach(file =>
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    stopwatch.Start();
    _ = ftpHelper.DownloadFileStream(file);
    stopwatch.Stop();

    Console.WriteLine($"{file}获取文件流耗时{stopwatch.Elapsed.TotalMilliseconds}");
});

Stopwatch stopwatch = Stopwatch.StartNew();
string filePath = "E:\\aaa1.pdf";
ftpHelper.DownloadFile(filePath,"aaa.pdf");
try
{
    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
        byte[] buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, buffer.Length);

        // 进一步处理文件流，例如可以上传到FTP服务器或进行其他操作
        // 例如，使用FluentFTP上传文件到FTP服务器
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

stopwatch.Stop();
Console.WriteLine($"获取文件流耗时{stopwatch.Elapsed.TotalMilliseconds}");*/

//Console.WriteLine("请输入待加密明文");
//string? v1 = Console.ReadLine();

//string v = AesHelper.Encrypt(v1);

//Console.WriteLine("加密后密文为："+ Environment.NewLine + v);




//Console.WriteLine("解密后明文为:"+ Environment.NewLine + AesHelper.Decrypt(v));


//Console.ReadLine();

string data = "";
data = ReadJsonFile(@"D:\table\recipe2.json");
RetMessage<GetRecipeData> retMessage =data.ToObject<RetMessage<GetRecipeData>>();


List<string> files = new List<string>();
string fileServer = "";
foreach (var item in retMessage.data.processParamItems)
{
    List<EsopItem> temp = new List<EsopItem>();
    foreach (var esop in item.esopList)
    {
        if (esop.filePath.IsNullOrEmpty()) continue;
        string[] strings = esop.filePath.Split(new[] { '/' }, 2);
        if (strings.Length > 1)
        {
            esop.filePath = strings[1];
            if (!files.Contains(strings[1]))
            {
                fileServer = strings[0];
                files.Add(strings[1]);
            }
        }
        temp.Add(esop);
    }
    item.esopList = temp;
}





static string ReadTextFile(string filePath)
{
    try
    {
        using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
        {
            string line="";
            string temp;
            while (( temp = sr.ReadLine()) != null)
            {
                 line += temp;
            }
            Console.WriteLine(line);
            return line;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred: " + e.Message);
        return "";
    }
}

static string ReadJsonFile(string filePath)
{
    try
    {
        string json = File.ReadAllText(filePath, Encoding.UTF8);
       
        //Console.WriteLine(json);
        return json;
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred: " + e.Message);
        return "";
    }
}



void UploadFileIfNotExistsAsync(string sourceUrl, string ftpFilePath)
{
    FTPparam fTPparam = AppSetting.FTPparam;
    FtpHelper ftpHelper = new FtpHelper(fTPparam.Host, fTPparam.Username, fTPparam.Password);
    // 异步执行文件上传，不等待其完成
    _ = Task.Run(() => ftpHelper.UploadServerFileToFtp(sourceUrl, ftpFilePath));

}
