using SqlSugar;
using FNMES.TEST;
using FNMES.Entity.Record;
using FNMES.Utility.Core;
using FNMES.Entity.DTO.ApiData;
using FNMES.Utility.Network;
// See https://aka.ms/new-console-template for more information


Console.WriteLine("Hello, World!");

RetMessage<GetLabelData> r =  new RetMessage<GetLabelData>()
{
    messageType = RetCode.error,
    message = "工厂接口超时或无响应",
    data = null
};

Console.WriteLine(r.ToJson());

RetMessage<GetLabelData> getLabelData = r.ToJson().ToObject< RetMessage<GetLabelData >>();
Console.WriteLine(getLabelData.ToJson());
string s = "{\"messageType\":\"E\",\"message\":\"工厂接口超时或无响应\",\"data\":null}";
Console.WriteLine(s);


Console.ReadLine();


public class Api
{
    public string url { get; set; }

    public string requestBody { get; set; }

    public string ResponseBody { get; set; }

    public int elapsed { get; set; }
}









