// Generated with https://json2csharp.com/

namespace Asus;

public class DownloadUrl
{
    public string Global { get; set; }
    public string China { get; set; }
}

public class File
{
    public string Id { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileSize { get; set; }
    public string ReleaseDate { get; set; }
    public string IsRelease { get; set; }
    public string PosType { get; set; }
    public DownloadUrl DownloadUrl { get; set; }
    public List<object> HardwareInfoList { get; set; }
    public string INFDate { get; set; }
}

public class Obj
{
    public string Name { get; set; }
    public int? Count { get; set; }
    public List<File> Files { get; set; }
    public bool? IsDescShow { get; set; }
}

public class Result
{
    public int? Count { get; set; }
    public List<Obj> Obj { get; set; }
}

public class Root
{
    public Result Result { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}

