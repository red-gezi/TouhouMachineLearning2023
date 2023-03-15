//�ó������ڿ��Ʒ���˰汾��ֻ��Ҫ��unity�е�����·������汾����
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

