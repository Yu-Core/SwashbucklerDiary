using System.Diagnostics;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IPlatformIntegration _platformIntegration;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IAppFileSystem appFileSystem,
            IAvatarService avatarService) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets, appFileSystem, avatarService)
        {
            _platformIntegration = platformIntegration;
        }

        public override async Task ToUpdate()
        {
            await _platformIntegration.OpenBrowser("https://github.com/Yu-Core/SwashbucklerDiary/releases");
        }

        public void MigrateAppDataDirectory()
        {
            if (!_versionTracking.IsFirstLaunchEver)
            {
                return;
            }

            string _oldAppDataParentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppInfo.PackageName);
            if (!Path.Exists(_oldAppDataParentDirectory))
            {
                return;
            }

            _appFileSystem.MoveFolder(_oldAppDataParentDirectory, Path.Combine(FileSystem.AppDataDirectory, ".."), SearchOption.AllDirectories, true);
            Directory.Delete(_oldAppDataParentDirectory, true);

            string command = $"sleep 1 && {Environment.ProcessPath} &";

            // 创建一个 ProcessStartInfo 对象来配置进程
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", // 使用 bash 来执行命令
                Arguments = $"-c \"{command}\"", // -c 表示执行后面的字符串作为命令
                RedirectStandardOutput = true, // 重定向标准输出
                RedirectStandardError = true,  // 重定向标准错误
                UseShellExecute = false,      // 不使用系统 shell 执行
                CreateNoWindow = true          // 不创建窗口
            };

            // 创建并启动进程
            using Process process = new Process();
            process.StartInfo = processStartInfo;

            // 启动进程
            process.Start();

            global::Gtk.Application.GetDefault()?.Quit();
        }
    }
}
