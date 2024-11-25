using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CCS.WebUI;
using FNMES.Service.WebService;
using FNMES.Utility.Core;
using FNMES.Utility.Extension;
using FNMES.Utility.Extension.SqlSugar;
using FNMES.Utility.Logs;
using FNMES.Utility.MiddleWare;
using FNMES.Utility.Other;
using FNMES.WebUI;
using FNMES.WebUI.Filters;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SoapCore;
using SqlSugar;
using UEditorNetCore;
using SoapCore.Extensibility;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args);
builder.WebHost.UseUrls("http://*:8080");
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.Init(builder.Configuration);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    builder.Services.AddModule(containerBuilder, builder.Configuration);
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.ValueLengthLimit = int.MaxValue;

});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    //options.AllowSynchronousIO = true;
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50M
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024; // 50M
    options.MaxRequestBodyBufferSize = 50 * 1024 * 1024;
    options.AllowSynchronousIO = true;
});
builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings = { [".properties"] = "application/octet-stream" }
    };
});




builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(AppSetting.SessionTimeout);
});
//使用缓存
builder.Services.AddMemoryCache();
builder.Services.AddMvc();
//添加基础数据库实例
builder.Services.AddSqlsugarServer(AppSetting.sysConnection);

//配置AP 使用json.net
builder.Services.AddControllers().AddNewtonsoftJson(opt =>
{
    //忽略循环引用
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    //不改变字段大小
    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions( o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddHostedService<BackgroundServices>();
builder.Services.AddUEditorService("Configs/ueditor.json");


//设置雪花ID的WorkId,每台必须不一样。
SnowFlakeSingle.WorkId = AppSetting.WorkId;

//services.AddRazorPages();
//注册Swagger生成器，定义一个和多个Swagger 文档
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        // {ApiName} 定义成全局变量，方便修改
        Version = "v1",
        Title = "My API"
    });
    //在接口类、方法标记属性 [HiddenApi]，可以阻止【Swagger文档】生成 
    c.DocumentFilter<HiddenApiFilter>();
    c.OrderActionsBy(o => o.RelativePath);
    //var basePath = System.AppDomain.CurrentDomain.BaseDirectory;//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
    //var xmlPath = Path.Combine(basePath, "FNMES.WebUI.Core.xml");
    //if (File.Exists(xmlPath))//避免没有该文件时报错
    //    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<WebServiceContract>();



var app = builder.Build();
MyHttpContext.ServiceProvider = app.Services;
//app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseStatusCodePagesWithReExecute("/error/notFound/{0}");
    app.UseHsts();
}
//启用Session
app.UseSession();

//配置WebService

app.UseSoapEndpoint<WebServiceContract>("/WebService.asmx", new SoapEncoderOptions());




app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
});


app.UseHttpsRedirection();

//为了IIS添加
//app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();