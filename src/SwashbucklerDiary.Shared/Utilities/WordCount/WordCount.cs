using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwashbucklerDiary.Shared
{
    // from https://stackoverflow.com/questions/1633116/word-count-algorithm-in-c-sharp/23084797#23084797
    public class WordCount
    {
        public int NonAsianWordCount { get; set; }
        public int AsianWordCount { get; set; }
        public int TextLineCount { get; set; }
        public int TotalWordCount { get; set; }
        public int CharacterCount { get; set; }
        public int CharacterCountWithSpaces { get; set; }

        //public string Text { get; set; }

        public WordCount() { }

        ~WordCount() { }

        public void GetCountWords(string s)
        {
            #region Regular Expression Collection
            string asianExpression = @"[\u3001-\uFFFF]";
            string englishExpression = @"[\S]+";
            string LineCountExpression = @"[\r]+";
            #endregion


            #region Asian Character
            MatchCollection asiancollection = Regex.Matches(s, asianExpression);

            AsianWordCount = asiancollection.Count; //Asian Character Count

            s = Regex.Replace(s, asianExpression, " ");

            #endregion


            #region English Characters Count
            MatchCollection collection = Regex.Matches(s, englishExpression);
            NonAsianWordCount = collection.Count;
            #endregion

            #region Text Lines Count
            MatchCollection Lines = Regex.Matches(s, LineCountExpression);
            TextLineCount = Lines.Count;
            #endregion

            #region Total Character Count

            CharacterCount = AsianWordCount;
            CharacterCountWithSpaces = CharacterCount;

            foreach (Match word in collection)
            {
                CharacterCount += word.Value.Length;
                CharacterCountWithSpaces += word.Value.Length + 1;
            }

            #endregion

            #region Total Character Count
            TotalWordCount = AsianWordCount + NonAsianWordCount;
            #endregion
        }
    }
}
