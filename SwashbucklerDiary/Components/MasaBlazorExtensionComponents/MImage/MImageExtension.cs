using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary.Components
{
    public class MImageExtension : MImage
    {
        private string? _mysrc;

        [Parameter]
        public string? MySrc
        {
            get => _mysrc; 
            set => SetMySrc(value);
        }

        private void SetMySrc(string? value)
        {
            _mysrc = value;
            Src = StaticCustomScheme.CustomPathToLocalPath(value);
        }
    }
}
