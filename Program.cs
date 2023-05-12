using System.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", HelloWorld);

app.Run();

async Task<IResult> HelloWorld()
{
    var http = new HttpClient();
    var body = await http.GetStringAsync("https://www.time.gov/zzz__d9216811ad1b84eb0e1053381e5c5b8292cc87e8.cgi?disablecache=1683858378297");

    return Results.Text(body, "text/xml");
}
