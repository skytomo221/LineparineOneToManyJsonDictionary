using CsvHelper;
using CsvHelper.Configuration;
using Otamajakushi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LineparineOneToManyJsonDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"lineparine.csv";
            var dicwords = Load(path);
            var dictionary = new OneToManyJson();
            foreach (var dicword in dicwords)
            {
                var translations = new List<Translation>();
                var r = new Regex(@"【(.*)】(.*)($|\n)");
                var tags = new List<string>();
                var transarray = dicword.Trans.Replace("\r\n", "\n").Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                transarray[0] = transarray[0].Replace("【前置詞】(文語・詩語：【後置詞】)", "【口語前置詞・文語後置詞】").Replace("【後置詞】(口語:【前置詞】)", "【口語前置詞・文語後置詞】");
                MatchCollection mc = r.Matches(transarray[0]);
                foreach (Match m in mc)
                {
                    var translation = new Translation
                    {
                        Title = m.Groups[1].Value.Replace("】【", "・"),
                        Forms = Regex.Split(m.Groups[2].Value, @"\s").ToList(),
                    };
                    translations.Add(translation);
                    if (Regex.IsMatch(m.Groups[2].Value, @"\[(古|口|略|国際理)語\]|\((古|口|略|国際理)語\)"))
                    {
                        tags.Add(Regex.Match(m.Groups[2].Value, @"\[(古|口|略|国際理)語\]|\((古|口|略|国際理)語\)").Value
                            .Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", ""));
                    }
                }
                var contents = new List<Content>();
                if (mc.Count == 0)
                {
                    var content = new Content
                    {
                        Title = "備考",
                        Text = transarray[0],
                    };
                    contents.Add(content);
                }
                foreach (var item in transarray.ToList().Skip(1))
                {
                    r = new Regex(@"\[(語法|文化)\]\n(.*?)(?=\[|\z)", RegexOptions.Singleline);
                    foreach (Match m in r.Matches(item))
                    {
                        contents.Add(new Content
                        {
                            Title = m.Groups[1].Value,
                            Text = m.Groups[2].Value,
                        });
                    }
                    var other = Regex.Replace(item, @"\[(語法|文化)\]\n(.*?)(?=\[|\z)", string.Empty, RegexOptions.Singleline).Trim();
                    if (!string.IsNullOrEmpty(other))
                    {
                        var content = new Content
                        {
                            Title = "備考",
                            Text = other,
                        };
                        contents.Add(content);
                    }
                }
                if (!string.IsNullOrEmpty(dicword.Exp))
                {
                    r = new Regex(@"\[(語法|文化)\]\n(.*?)(?=\[|\z)", RegexOptions.Singleline);
                    foreach (Match m in r.Matches(dicword.Exp))
                    {
                        contents.Add(new Content
                        {
                            Title = m.Groups[1].Value,
                            Text = m.Groups[2].Value,
                        });
                    }
                    contents.Add(new Content
                    {
                        Title = "例文",
                        Text = Regex.Replace(dicword.Exp, @"\[(語法|文化)\]\n(.*?)(?=\[|\z)", string.Empty, RegexOptions.Singleline).Trim(),
                    });
                }
                var word = new Word
                {
                    Entry = new Entry
                    {
                        Form = dicword.Word,
                    },
                    Translations = translations,
                    Tags = tags,
                    Contents = contents
                };
                if (word.Translations.Count >= 1 && word.Translations.All(t => t.Title == "名詞"))
                {
                    word.Variations.Add(new Variation { Title = "主格", Form = Buffer(word.Entry.Form, "'s") });
                    word.Variations.Add(new Variation { Title = "対格", Form = Buffer(word.Entry.Form, "'i") });
                    word.Variations.Add(new Variation { Title = "属格", Form = Buffer(word.Entry.Form, "'d") });
                    word.Variations.Add(new Variation { Title = "与格", Form = Buffer(word.Entry.Form, "'c") });
                    word.Variations.Add(new Variation { Title = "方向格", Form = Buffer(word.Entry.Form, "'l") });
                    word.Variations.Add(new Variation { Title = "部分格", Form = Buffer(word.Entry.Form, "'sta") });
                    word.Variations.Add(new Variation { Title = "呼格", Form = Buffer(word.Entry.Form, "'sti") });
                    word.Variations.Add(new Variation { Title = "話題格", Form = Buffer(word.Entry.Form, "'sci") });
                    word.Variations.Add(new Variation { Title = "共格", Form = Buffer(word.Entry.Form, "'tj") });
                    word.Variations.Add(new Variation { Title = "欠格", Form = Buffer(word.Entry.Form, "nerfe") });
                    word.Variations.Add(new Variation { Title = "処格", Form = word.Entry.Form + " io" });
                    word.Variations.Add(new Variation { Title = "奪格", Form = word.Entry.Form + " ler" });
                    word.Variations.Add(new Variation { Title = "定性", Form = Buffer(word.Entry.Form, "stan") });
                    word.Variations.Add(new Variation { Title = "不定性", Form = Buffer(word.Entry.Form, "ste") });
                    word.Variations.Add(new Variation { Title = "定性", Form = Buffer("gir", word.Entry.Form) });
                    if (word.Entry.Form + "'s" != Buffer(word.Entry.Form, "'s"))
                        word.Variations.Add(new Variation { Title = "主格", Form = word.Entry.Form + "'s" });
                    if (word.Entry.Form + "'i" != Buffer(word.Entry.Form, "'i"))
                        word.Variations.Add(new Variation { Title = "対格", Form = word.Entry.Form + "'i" });
                    if (word.Entry.Form + "'d" != Buffer(word.Entry.Form, "'d"))
                        word.Variations.Add(new Variation { Title = "属格", Form = word.Entry.Form + "'d" });
                    if (word.Entry.Form + "'c" != Buffer(word.Entry.Form, "'c"))
                        word.Variations.Add(new Variation { Title = "与格", Form = word.Entry.Form + "'c" });
                    if (word.Entry.Form + "'l" != Buffer(word.Entry.Form, "'l"))
                        word.Variations.Add(new Variation { Title = "方向格", Form = word.Entry.Form + "'l" });
                    if (word.Entry.Form + "'sta" != Buffer(word.Entry.Form, "'sta"))
                        word.Variations.Add(new Variation { Title = "部分格", Form = word.Entry.Form + "'sta" });
                    if (word.Entry.Form + "'sti" != Buffer(word.Entry.Form, "'sti"))
                        word.Variations.Add(new Variation { Title = "呼格", Form = word.Entry.Form + "'sti" });
                    if (word.Entry.Form + "'sci" != Buffer(word.Entry.Form, "'sci"))
                        word.Variations.Add(new Variation { Title = "話題格", Form = word.Entry.Form + "'sci" });
                    if (word.Entry.Form + "'tj" != Buffer(word.Entry.Form, "'tj"))
                        word.Variations.Add(new Variation { Title = "共格", Form = word.Entry.Form + "'tj" });
                    if (word.Entry.Form + "nerfe" != Buffer(word.Entry.Form, "nerfe"))
                        word.Variations.Add(new Variation { Title = "欠格", Form = word.Entry.Form + "nerfe" });
                    if (word.Entry.Form + "stan" != Buffer(word.Entry.Form, "stan"))
                        word.Variations.Add(new Variation { Title = "定性", Form = word.Entry.Form + "stan" });
                    if (word.Entry.Form + "ste" != Buffer(word.Entry.Form, "ste"))
                        word.Variations.Add(new Variation { Title = "不定性", Form = word.Entry.Form + "ste" });
                    if ("gir" + word.Entry.Form != Buffer("gir", word.Entry.Form))
                        word.Variations.Add(new Variation { Title = "定性", Form = "gir" + word.Entry.Form });
                }
                dictionary.AddWord(word);
            }
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            var json = OneToManyJsonSerializer.Serialize(dictionary, options);
            File.WriteAllText(@"output.json", json);
        }

        /// <summary>
        /// 緩衝母音とか緩衝子音の処理をします．
        /// </summary>
        /// <param name="front">前の単語</param>
        /// <param name="back">後ろの単語</param>
        /// <returns></returns>
        public static string Buffer(string front, string back)
        {
            var vowels = new List<char> { 'a', 'e', 'i', 'o', 'u', 'y' };
            var front_back = Regex.Replace(front, "[^a-zA-Z]+", string.Empty).Reverse().First(_ => true);
            var back_front = Regex.Replace(back, "[^a-zA-Z]+", string.Empty).First(_ => true);
            if (vowels.Contains(front_back) == vowels.Contains(back_front))
            {
                if (Regex.Replace(front, "[^aeiouy]+", string.Empty).Length == 0)
                    return front + back;
                var type = Regex.Replace(front, "[^aeiouy]+", string.Empty).Reverse().First(_ => true);
                if (vowels.Contains(front_back))
                    switch (type)
                    {
                        case 'a':
                        case 'o':
                            return front + "v" + back;
                        case 'e':
                        case 'i':
                            return front + "rg" + back;
                        case 'u':
                            return front + "m" + back;
                        case 'y':
                            return front + "l" + back;
                        default:
                            break;
                    }
                else
                    switch (type)
                    {
                        case 'a':
                        case 'o':
                            return front + "a" + back;
                        case 'e':
                        case 'i':
                            return front + "e" + back;
                        case 'u':
                            return front + "u" + back;
                        case 'y':
                            return front + "i" + back;
                        default:
                            break;
                    }
            }
            return front + back;
        }

        public static List<DicWord> Load(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<DicWordMap>();
                csv.Configuration.MissingFieldFound = null;
                return csv.GetRecords<DicWord>().ToList();
            }
        }
    }
}
