using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Set Environment
var driverUrl = Environment.GetEnvironmentVariable("DRIVER_URL");
var biosUrl = Environment.GetEnvironmentVariable("BIOS_URL");
var downloadUrls = new string[] { driverUrl, biosUrl };
var nvidiaUrl = Environment.GetEnvironmentVariable("NVIDIA_URL");
var checkAmd = false;
Boolean.TryParse(Environment.GetEnvironmentVariable("CHECK_AMD"), out checkAmd);
var timeout = TimeSpan.FromSeconds(20);

// Validate env variables set 
foreach (var stringToValidate in new string[] {driverUrl, biosUrl}) 
{
    if(String.IsNullOrEmpty(stringToValidate))
    {
        Console.WriteLine("FATAL - these inputs are required; set environment variables DRIVER_URL, BIOS_URL");
        return;
    }
}

// Set route
app.MapGet("feed.xml", GenerateFeed);

// Run
app.Run();

async Task<IResult> GenerateFeed()
{
    HttpClientHandler handler = new HttpClientHandler()
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
    };
    var http = new HttpClient(handler);
    http.Timeout = timeout;
    http.DefaultRequestHeaders.Add("Accept", "*/*");
    http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.37");
    http.DefaultRequestHeaders.Add("Connection", "keep-alive");

    var feed = new SyndicationFeed("ASUS Driver Feed", "Live updates of ASUS drivers", new Uri("https://rog.asus.com/support"));
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

    // AMD
    // https://www.amd.com/en/support
    // <a href="https://drivers.amd.com/drivers/installer/22.40/whql/amd-software-adrenalin-edition-23.5.2-minimalsetup-230531_web.exe">DOWNLOAD WINDOWS DRIVERS </a></div>
    if (checkAmd)
    {
        // try
        // {
            var amdUrl = "https://www.amd.com/en/support";

            // Very picky server - will just hang without headers being just so.
            // Need a timeout handler that also emits a SyndicationItem
        
            var task = http.GetStringAsync(amdUrl);
            // await task.WaitAsync(timeout);
            // if (task.IsCanceled)
            // {
            //     // error handling
            //     int i = 0;
            // }

            task.Wait();
            var body = task.Result;


            var match = Regex.Match(body, @"href=""([^""]+)"">DOWNLOAD WINDOWS DRIVERS", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var downloadUrl = match.Groups[1].Value;
            var version = Regex.Match(downloadUrl, @"(\d+\.\d+\.\d+)").Groups[1].Value;


            var headReq = new HttpRequestMessage(HttpMethod.Head, downloadUrl);
            headReq.Headers.Add("Accept", http.DefaultRequestHeaders.Accept.ToString());
            headReq.Headers.Add("User-Agent", "PostmanRuntime/7.32.2");
            headReq.Headers.Add("Connection", http.DefaultRequestHeaders.Connection.ToString());
            var headResp = await http.SendAsync(headReq);
            var lastModified = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(headResp.Headers.ETag!.Tag.Trim('"')));

            var item = new SyndicationItem(System.Net.WebUtility.UrlDecode("AMD Windows Drivers") + " - " + version,
                    $"Version {version} - {downloadUrl}", new Uri("https://www.amd.com/en/support"), downloadUrl,
                    lastModified);


            items.Add(item);
        // }
        // catch (Exception ex)//System.TimeoutException - from Wait? System.AggregateException w/inner System.Threading.Tasks.TaskCanceledException
        // {
        //     ex.ToString();
        // }
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