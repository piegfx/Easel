using Easel.Headless.Configs;

namespace Easel.Headless.Data;

public abstract class EaselConfig<T>
{
    public static T LoadedConfig;

    public DisplayConfig DisplayConfig;

    protected EaselConfig() { }

    protected EaselConfig(DisplayConfig displayConfig)
    {
        DisplayConfig = displayConfig;
    }

    public static void LoadConfigFromXml(string xml)
    {
        LoadedConfig = XmlSerializer.Deserialize<T>(xml);
    }
}

public class EaselConfig : EaselConfig<EaselConfig>
{
    public EaselConfig(DisplayConfig displayConfig) : base(displayConfig) { }
}