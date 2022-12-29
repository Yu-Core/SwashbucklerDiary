using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageWrite
    {
        private readonly List<string> _weathers = new List<string>()
    {
        "晴","阴","小雨","中雨","大雨","小雪","中雪","大雪","雾",
    };
        private string _value;
        private List<string> _weathers2 = new List<string>();

        private void Remove(string item)
        {
            var index = _weathers2.IndexOf(item);
            if (index >= 0)
            {
                _weathers2.RemoveAt(index);
            }
        }
    }
}
