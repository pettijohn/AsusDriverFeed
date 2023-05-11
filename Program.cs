var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", HelloWorld);

app.Run();

string HelloWorld()
{
    return "Hello World";
}
