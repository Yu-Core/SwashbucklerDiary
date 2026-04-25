using System.Text.Json.Serialization;

namespace SwashbucklerDiary.Rcl.Essentials
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$platform")]
    [JsonDerivedType(typeof(WindowsAppPlatform), "Windows")]
    [JsonDerivedType(typeof(AndroidAppPlatform), "Android")]
    [JsonDerivedType(typeof(IOSAppPlatform), "iOS")]
    [JsonDerivedType(typeof(MacOSAppPlatform), "macOS")]
    [JsonDerivedType(typeof(BrowserAppPlatform), "Browser")]
    [JsonDerivedType(typeof(LinuxAppPlatform), "Linux")]
    [JsonDerivedType(typeof(WinUIPackagedAppPlatform), "WinUIPackaged")]
    [JsonDerivedType(typeof(WinUIUnpackagedAppPlatform), "WinUIUnpackaged")]
    [JsonDerivedType(typeof(BlazorWebAssemblyAppPlatform), "BlazorWebAssembly")]
    [JsonDerivedType(typeof(BlazorServerAppPlatform), "BlazorServer")]
    public abstract record AppPlatform(AppOperatingSystem OS);

    public record UnknownAppPlatform() : AppPlatform(AppOperatingSystem.Unknown);
    public record WindowsAppPlatform() : AppPlatform(AppOperatingSystem.Windows);
    public record AndroidAppPlatform() : AppPlatform(AppOperatingSystem.Android);
    public record IOSAppPlatform() : AppPlatform(AppOperatingSystem.iOS);
    public record MacOSAppPlatform() : AppPlatform(AppOperatingSystem.macOS);
    public record BrowserAppPlatform() : AppPlatform(AppOperatingSystem.Browser);
    public record LinuxAppPlatform() : AppPlatform(AppOperatingSystem.Linux);

    public record WinUIPackagedAppPlatform() : WindowsAppPlatform;
    public record WinUIUnpackagedAppPlatform() : WindowsAppPlatform;

    public record BlazorWebAssemblyAppPlatform() : BrowserAppPlatform;
    public record BlazorServerAppPlatform() : BrowserAppPlatform;

    public enum AppOperatingSystem
    {
        Unknown,
        Windows,
        Android,
        iOS,
        macOS,
        Browser,
        Linux
    }
}
