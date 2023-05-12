using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Set Environment
var driverUrl = Environment.GetEnvironmentVariable("DRIVER_URL");
var biosUrl = Environment.GetEnvironmentVariable("BIOS_URL");
var downloadUrls = new string[] { driverUrl, biosUrl };
var feedUrl = Environment.GetEnvironmentVariable("FEED_URL");

// Validate env variables set 
foreach (var stringToValidate in new string[] {driverUrl, biosUrl, feedUrl})
{
    if(String.IsNullOrEmpty(stringToValidate))
    {
        Console.WriteLine("FATAL - all inputs are required; set environment variables DRIVER_URL, BIOS_URL, FEED_URL");
        return;
    }
}

// Set route
app.MapGet("feed.xml", GenerateFeed);

// Run
app.Run();

async Task<IResult> GenerateFeed()
{
    var http = new HttpClient();
    
    var feed = new SyndicationFeed("ASUS Driver Feed - GA402XI", "Live updates of ASUS drivers", new Uri(feedUrl));
    feed.TimeToLive = TimeSpan.FromHours(12);
    feed.LastUpdatedTime = DateTimeOffset.UtcNow;
    feed.Language = "en-us";

    // TODO - catch exceptions and publish as feed item
    List<SyndicationItem> items = new List<SyndicationItem>();
    foreach (var url in downloadUrls)
    {
        var body = await http.GetStringAsync(url);
        Root driverList = JsonSerializer.Deserialize<Root>(body);

        foreach (var category in driverList.Result.Obj)
        {
            foreach (var driverItem in category.Files)
            {
                TextSyndicationContent textContent = new TextSyndicationContent(driverItem.Description);
                SyndicationItem item = new SyndicationItem(driverItem.Title, textContent, new Uri(driverItem.DownloadUrl.Global), driverItem.Id, DateTimeOffset.Parse(driverItem.ReleaseDate));

                items.Add(item);
            }
        }
    }
    feed.Items = items;

    var sb = new StringBuilder();
    using (XmlWriter rssWriter = XmlWriter.Create(sb))
    {
        Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(feed);
        rssFormatter.WriteTo(rssWriter);
        rssWriter.Flush();
        return Results.Text(sb.ToString(), "application/rss+xml");
    }
}