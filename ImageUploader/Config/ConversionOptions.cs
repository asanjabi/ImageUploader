namespace ImageUploader.Config;

public class ConversionOptions
{
    public const string ConfigSectionName = "ConversionOptions";

    public bool Convert { get; set; }
    public string Format { get; set; } = string.Empty;
    public int maxWidth { get; set; }
    public int maxHeight { get; set; }
}
