using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SwashbucklerDiary.Components
{
    public partial class ExportDialog : DialogComponentBase
    {
        private const string fileName = "SwashbucklerDiaryExport";
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
                new(this,"PDF","mdi-file-pdf-box",ToDo),
                new(this,"JSON","mdi-code-json",CreateJsonFile),
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

            var targetFileName = fileName + ".txt";
            string targetFile = Path.Combine(FileSystem.CacheDirectory, targetFileName);
            await File.WriteAllTextAsync(targetFile, text.ToString());
            using FileStream stream = File.OpenRead(targetFile);
            targetFileName = fileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            await SaveFile(targetFileName, stream);
        }

        private async Task SaveFile(string fileName,Stream stream)
        {
            //Cannot save an existing file
            //https://github.com/CommunityToolkit/Maui/issues/1049
            var filePath = await SystemService.SaveFileAsync(fileName, stream);
            if (filePath == null)
            {
                return;
            }

            await AlertService.Success(I18n.T("Export.ExportSuccess"));
        }

        private async Task CreateJsonFile()
        {
            await InternalValueChanged(false);

            var targetFileName = fileName + ".json";
            string targetFile = Path.Combine(FileSystem.CacheDirectory, targetFileName);

            var options = new JsonSerializerOptions { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            
            string jsonString = JsonSerializer.Serialize(Diaries, options);
            await File.WriteAllTextAsync(targetFile, jsonString);
            using FileStream stream = File.OpenRead(targetFile);

            targetFileName = fileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
            await SaveFile(targetFileName, stream);
        }

        private Task CreatePDFFile()
        {
            return Task.CompletedTask;
        }
    }
}
