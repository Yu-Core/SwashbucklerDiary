using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwashbucklerDiary.IServices
{
    /// <summary>
    /// 为了解决MAUI与Blazor中作用域不同，导致获取的I18n实例不同
    /// </summary>
    public interface II18nService
    {
        event Action OnChanged;
        I18n I18n { get; protected set; }
        CultureInfo Culture { get; }
        void Initialize(I18n i18n);
        string T(string? key);
        void SetCulture(string culture);
        string ToWeek(DateTime? dateTime = null);
    }
}
