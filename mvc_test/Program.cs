using SqlSugar;
using FNMES.TEST;
using FNMES.Entity.Record;
using FNMES.Utility.Core;
// See https://aka.ms/new-console-template for more information


Console.WriteLine("Hello, World!");

RecordApi recordApi = new();
Api api = new Api
{
    url = "qer",
    requestBody = "weq",
    ResponseBody = "234",
    elapsed = 10
};

recordApi.CopyField(api);


Console.WriteLine(recordApi.ToJson());

Console.ReadLine();


public class Api
{
    public string url { get; set; }

    public string requestBody { get; set; }

    public string ResponseBody { get; set; }

    public int elapsed { get; set; }
}









