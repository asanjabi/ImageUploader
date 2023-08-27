namespace ImageUploader.Config;

public class StorageOptions
{
    public const string ConfigSectionName = "StorageOptions";

    public string StorageBaseUrl { get; set; } = string.Empty;
    public string FileNameFormatString { get; set; } = string.Empty;
    public int MaxFileSizeInMB { get; set; }
}
