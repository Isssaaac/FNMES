using Newtonsoft.Json;
using System.Collections.Generic;

public class DataItem
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class Program
{
    public static void Main()
    {
        DataItem[] dataItems = new DataItem[]
        {
            new DataItem { Name = "DATA22", Value = "123" },
            new DataItem { Name = "DATA23", Value = "456" }
        };

        // 将数组转换成字典
        var dictionary = new Dictionary<string, string>();
        foreach (var item in dataItems)
        {
            dictionary[item.Name] = item.Value;
        }

        // 将字典转换成JSON字符串
        string jsonResult = JsonConvert.SerializeObject(dictionary);
        Console.WriteLine(jsonResult);
    }
}
