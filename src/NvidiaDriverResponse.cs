// Generated with https://json2csharp.com/

namespace Nvidia;
public class DownloadInfo
{
    public string Success { get; set; }
    public string ID { get; set; }
    public string DownloadTypeID { get; set; }
    public string DownloadStatusID { get; set; }
    public string Name { get; set; }
    public string NameLocalized { get; set; }
    public string ShortDescription { get; set; }
    public string DeviceToProductFamilyName { get; set; }
    public string Release { get; set; }
    public string Version { get; set; }
    public string DisplayVersion { get; set; }
    public string GFE_DisplayVersion { get; set; }
    public string CDKitUSBEmitterDriverVersion { get; set; }
    public string CDKitGPUDriverVersion { get; set; }
    public string IsBeta { get; set; }
    public string IsWHQL { get; set; }
    public string IsRecommended { get; set; }
    public string IsFeaturePreview { get; set; }
    public string IsNewest { get; set; }
    public string IsDC { get; set; }
    public string IsCRD { get; set; }
    public string HasNetInst { get; set; }
    public string IsArchive { get; set; }
    public string IsActive { get; set; }
    public string IsEmailRequired { get; set; }
    public string ReleaseDateTime { get; set; }
    public string DetailsURL { get; set; }
    public string DownloadURL { get; set; }
    public string EMAN_REVRES_BD { get; set; }
    public string EMITR_BD { get; set; }
    public string DownloadURLFileSize { get; set; }
    public string BannerURL { get; set; }
    public string BannerURLGfe { get; set; }
    public string ReleaseNotes { get; set; }
    public string OtherNotes { get; set; }
    public string InstallationNotes { get; set; }
    public string Overview { get; set; }
    public string LanguageName { get; set; }
    public string OSName { get; set; }
    public string OsCode { get; set; }
    public List<OSList> OSList { get; set; }
    public string Is64Bit { get; set; }
    public List<Messaging> Messaging { get; set; }
    public List<Series> series { get; set; }
}

public class ID
{
    public DownloadInfo downloadInfo { get; set; }
}

public class Messaging
{
    public string MessageCode { get; set; }
    public string MessageValue { get; set; }
}

public class OSList
{
    public string OSName { get; set; }
    public string OsCode { get; set; }
}

public class Product
{
    public string productName { get; set; }
}

public class Request
{
    public string psid { get; set; }
    public string pfid { get; set; }
    public string osID { get; set; }
    public string languageCode { get; set; }
    public string beta { get; set; }
    public string isWHQL { get; set; }
    public string dltype { get; set; }
    public string sort1 { get; set; }
    public string numberOfResults { get; set; }
}

public class Root
{
    public string Success { get; set; }
    public List<ID> IDS { get; set; }
    public string spcall { get; set; }
    public List<Request> Request { get; set; }
}

public class Series
{
    public string seriesname { get; set; }
    public List<Product> products { get; set; }
}

