using System.Net;
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
var nvidiaUrl = Environment.GetEnvironmentVariable("NVIDIA_URL");
// TODO - fetch NVIDIA 
// https://www.nvidia.com/en-us/geforce/drivers/

// Validate env variables set 
foreach (var stringToValidate in new string[] {driverUrl, biosUrl, feedUrl})
{
    if(String.IsNullOrEmpty(stringToValidate))
    {
        Console.WriteLine("FATAL - these inputs are required; set environment variables DRIVER_URL, BIOS_URL, FEED_URL");
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
    feed.ImageUrl = new Uri("https://www.asus.com/favicon.ico");
    feed.TimeToLive = TimeSpan.FromHours(12);
    feed.LastUpdatedTime = DateTimeOffset.UtcNow;
    feed.Language = "en-us";

    // TODO - catch exceptions and publish as feed item
    var items = new List<SyndicationItem>();
    foreach (var url in downloadUrls)
    {
        var body = await http.GetStringAsync(url);
        var driverList = JsonSerializer.Deserialize<Asus.Root>(body);

        foreach (var category in driverList.Result.Obj)
        {
            foreach (var driverItem in category.Files)
            {
                TextSyndicationContent textContent = new TextSyndicationContent(driverItem.Description, TextSyndicationContentKind.Html);
                SyndicationItem item = new SyndicationItem(driverItem.Title, textContent, new Uri(driverItem.DownloadUrl.Global), driverItem.Id, DateTimeOffset.Parse(driverItem.ReleaseDate));

                items.Add(item);
            }
        }
    }

    if(!String.IsNullOrEmpty(nvidiaUrl))
    {
        var body = await http.GetStringAsync(nvidiaUrl);
        var driverList = JsonSerializer.Deserialize<Nvidia.Root>(body);

        foreach (var driverItem in driverList.IDS)
        {
            var downloadInfo = driverItem.downloadInfo;
            var textContent = new TextSyndicationContent(System.Net.WebUtility.UrlDecode(downloadInfo.ReleaseNotes), TextSyndicationContentKind.Html);
            var item = new SyndicationItem(System.Net.WebUtility.UrlDecode(downloadInfo.ShortDescription) + " - " + downloadInfo.Version, 
                textContent, new Uri(downloadInfo.DownloadURL), downloadInfo.DetailsURL, 
                DateTimeOffset.Parse(downloadInfo.ReleaseDateTime));
            

            items.Add(item);
        }
    }


    feed.Items = items;

    var sb = new StringBuilder();
    using (XmlWriter feedWriter = XmlWriter.Create(sb))
    {
        var feedFormatter = new Atom10FeedFormatter(feed);
        feedFormatter.WriteTo(feedWriter);
        feedWriter.Flush();
        return Results.Text(sb.ToString(), "application/atom+xml");
    }
}