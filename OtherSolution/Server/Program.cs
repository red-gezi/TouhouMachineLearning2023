using Blazored.LocalStorage;
using Server;
using Server.Data;
using System;
//链接用户数据库
Log.Init();
Log.Summary("日志系统初始化");
MongoDbCommand.Init();
Log.Summary("数据库已初始化");
HoldListManager.Init();
Log.Summary("匹配管理器已初始化");
HttpServer.Init();
Log.Summary("资源服务器已初始化");
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddAntDesign();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<DiyInfo>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.MaximumReceiveMessageSize = null;
    hubOptions.ClientTimeoutInterval = new TimeSpan(0, 5, 0);
}).AddNewtonsoftJsonProtocol();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<TouHouHub>("/TouHouHub");
app.Urls.Add("http://*:495");
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://www.baidu.com/");
    await next.Invoke();
});
//app.Urls.Add("https://*:49514");
Console.WriteLine("已载入回应中心");
Console.WriteLine("服务端已启动");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        RoomManager.Rooms.ForEach(room => Console.WriteLine(room.Summary.ToJson()));
        //RoomManager.Rooms.FirstOrDefault()?.Summary.UploadAgentSummary(0, 0);
        //Console.WriteLine("上传完毕");
    }
});
Timer timer = new Timer(new TimerCallback((o) => HoldListManager.Match()));
timer.Change(0, 5000);
app.Run();
