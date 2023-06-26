using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SwashbucklerDiary.Components
{
    public partial class ExportDialog : DialogComponentBase
    {
        private const string strFileName = "SwashbucklerDiaryExport";
        private List<DynamicListItem> ListItemModels = new();

        [Inject]
        protected ISystemService SystemService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this,"TXT","mdi-format-text",CreateTxtFile),
                new(this,"MD","mdi-language-markdown-outline",CreateMDFile),
                new(this,"JSON","mdi-code-json",CreateJsonFile),
                new(this,"PDF","mdi-file-pdf-box",ToDo),
            };
        }

        private async Task CreateTxtFile()
        {
            await InternalValueChanged(false);

            StringBuilder text = new ();
            foreach (var item in Diaries)
            {
                text.AppendLine(item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if(string.IsNullOrEmpty(item.Title))
                {
                    text.AppendLine(item.Title);
                }

                if (!string.IsNullOrEmpty(item.Weather))
                {
                    text.AppendLine(I18n.T("Weather." + item.Weather));
                }

                if (!string.IsNullOrEmpty(item.Mood))
                {
                    text.AppendLine(I18n.T("Weather." + item.Weather));
                }

                if (!string.IsNullOrEmpty(item.Location))
                {
                    text.AppendLine(item.Location);
                }

                text.AppendLine(item.Content);
                if(item.Tags is not null && item.Tags.Count > 0)
                {
                    foreach (var tag in item.Tags)
                    {
                        text.Append(tag + " ");
                    }
                    text.AppendLine();
                }

                text.AppendLine();
            }

            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{strFileName}.txt");
            await File.WriteAllTextAsync(filePath, text.ToString());

            string saveFilePath = strFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            await SaveFile(saveFilePath, filePath);
        }

        private async Task SaveFile(string targetFilePath, string sourceFilePath)
        {
            using FileStream stream = File.OpenRead(sourceFilePath);
            //Cannot save an existing file
            //https://github.com/CommunityToolkit/Maui/issues/1049
            var filePath = await SystemService.SaveFileAsync(targetFilePath, stream);
            if (filePath == null)
            {
                return;
            }

            await AlertService.Success(I18n.T("Export.ExportSuccess"));
        }

        private async Task CreateJsonFile()
        {
            await InternalValueChanged(false);

            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{strFileName}.json");

            var options = new JsonSerializerOptions { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            
            string jsonString = JsonSerializer.Serialize(Diaries, options);
            await File.WriteAllTextAsync(filePath, jsonString);

            string saveFilePath = strFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
            await SaveFile(saveFilePath, filePath);
        }

        private Task CreatePDFFile()
        {
            return Task.CompletedTask;
        }

        private async Task CreateMDFile()
        {
            await InternalValueChanged(false);

            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Markdown");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{strFileName}.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                SystemService.ClearFolder(outputFolder);
            }

            foreach (var item in Diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss") + ".md";
                string filePath = Path.Combine(outputFolder, fileName);
                WriteToFile(filePath, item.Content);
            }

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有.md文件添加到压缩包中
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            string saveFilePath = strFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip"; ;
            await SaveFile(saveFilePath, zipFilePath);
        }

        public static void WriteToFile(string fileName, string? content)
        {
            int suffix = 1;
            string newFileName = fileName;
            string fileDirectoryName = Path.GetDirectoryName(fileName) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string pathWithoutExtension = Path.Combine(fileDirectoryName, fileNameWithoutExtension);
            string fileExtension = Path.GetExtension(fileName);

            while (File.Exists(newFileName))
            {
                suffix++;
                newFileName = $"{pathWithoutExtension}({suffix}){fileExtension}";
            }

            using StreamWriter writer = new (newFileName);
            writer.Write(content);
        }
    }
}
