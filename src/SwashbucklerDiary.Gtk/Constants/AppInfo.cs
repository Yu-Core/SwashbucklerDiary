using System.Reflection;

namespace SwashbucklerDiary.Gtk
{
    public static class AppInfo
    {
        static readonly Assembly _launchingAssembly = Assembly.GetEntryAssembly()!;
        public static string PackageName
            => _launchingAssembly.GetMetadataAttributeValue("PackageName") ?? _launchingAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

        static string GetMetadataAttributeValue(this Assembly assembly, string key)
        {
            foreach (var attr in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (attr.Key == key)
                    return attr.Value;
            }

            return null;
        }
    }
}
