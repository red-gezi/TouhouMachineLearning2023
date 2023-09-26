using Blazored.LocalStorage;
using Server;
using Server.Data;
using System;
//�����û����ݿ�
Log.Init();
Log.Summary("��־ϵͳ��ʼ��");
MongoDbCommand.Init();
Log.Summary("���ݿ��ѳ�ʼ��");
HoldListManager.Init();
Log.Summary("ƥ��������ѳ�ʼ��");
HttpServer.Init();
Log.Summary("��Դ�������ѳ�ʼ��");
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
Console.WriteLine("�������Ӧ����");
Console.WriteLine("�����������");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        RoomManager.Rooms.ForEach(room => Console.WriteLine(room.Summary.ToJson()));
        //RoomManager.Rooms.FirstOrDefault()?.Summary.UploadAgentSummary(0, 0);
        //Console.WriteLine("�ϴ����");
    }
});
Timer timer = new Timer(new TimerCallback((o) => HoldListManager.Match()));
timer.Change(0, 5000);
app.Run();
