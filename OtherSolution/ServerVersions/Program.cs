//该程序用于控制服务端版本，只需要在unity中点击更新服务器版本即可
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.MaximumReceiveMessageSize = null;
});
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapHub<VersionsHub>("/VersionsHub");
app.Urls.Add("http://*:233");
VersionsHub.Init();
app.Run();

