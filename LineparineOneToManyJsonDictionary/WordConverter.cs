using Otamajakushi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LineparineOneToManyJsonDictionary
{
    class WordConverter
    {
        public Word Word { get; set; }

        public DicWord DicWord { get; set; }

        protected WordConverter ConvertEntry()
        {
            Word = new Word
            {
                Entry = new Entry
                {
                    Form = DicWord.Word,
                },
            };
            return this;
        }

        protected WordConverter ConvertTranslations()
        {
            Word.Translations = new List<Translation>();
            foreach (var (line, index) in DicWord.Trans.Split('\n').Select((line, index) => (line, index)))
            {
                var line2 =
                    line.Replace("【前置詞】(文語・詩語：【後置詞】)", "【口語前置詞・文語後置詞】")
                    .Replace("【後置詞】(口語:【前置詞】)", "【口語前置詞・文語後置詞】");
                var r = new Regex(@"【(.*)】(.*)($|\n)");
                MatchCollection mc = r.Matches(line2);
                if (mc.Count == 0)
                {
                    DicWord.Trans = DicWord.Trans.Split('\n').Skip(index).Aggregate((now, next) => now + "\n" + next);
                    break;
                }
                foreach (Match m in mc)
                {
                    Word.Translations.Add(new Translation
                    {
                        Title = m.Groups[1].Value.Replace("】【", "・"),
                        Forms = Regex.Split(m.Groups[2].Value, @"\s").ToList(),
                    });
                }
            }
            return this;
        }

        protected WordConverter ConvertTags()
        {
            Word.Tags = new List<string>();
            foreach (var translation in Word.Translations)
            {
                foreach (var form in translation.Forms)
                {
                    if (Regex.IsMatch(form, @"(\[|\()(古|口|略|国際理)語(\]|\))"))
                    {
                        Word.Tags.Add(Regex.Match(form, @"(古|口|略|国際理)語").Value);
                    }
                }
            }
            return this;
        }

        protected WordConverter ConvertWording()
        {
            var wording = new Content();
            foreach (var (line, index) in DicWord.Trans.Split('\n').Select((line, index) => (line, index)))
            {

            }
            return this;
        }


        protected WordConverter ConvertRemarks()
        {
            Word.Contents.Add(new Content
            {
                Title = "備考",
                Text = DicWord.Trans,
            });
            return this;
        }

        public WordConverter ConvertInit()
        {
            DicWord.Trans = DicWord.Trans.Replace("\r\n", "\n");
            return this;
        }

        public Word Convert()
        {
            return ConvertInit()
                .ConvertEntry()
                .ConvertTranslations()
                .ConvertTags()
                .ConvertRemarks()
                .Word;
        }
    }
}
