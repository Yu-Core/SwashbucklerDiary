using NoDecentDiary.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageLog
    {
        private List<string> LogTextList = new List<string>();
        protected override Task OnInitializedAsync()
        {
            InitLogText();
            return base.OnInitializedAsync();
        }
        private void InitLogText()
        {
            if(!File.Exists(SerilogConstants.filePath))
            {
                return;
            }
            using var stream = File.Open(SerilogConstants.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string? Line;
            while ((Line = reader.ReadLine()) != null)
            {
                LogTextList.Add(Line);
            }
        }
    }
}
