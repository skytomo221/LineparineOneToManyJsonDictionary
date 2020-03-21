using CsvHelper;
using CsvHelper.Configuration;
using Otamajakushi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            int id = 1;
            foreach (var dicword in dicwords)
            {
                var translations = new List<Translation>();
                var r = new Regex(@"【(.*)】(.*)($|\n)");
                var tags = new List<string>();
                var transarray = dicword.Trans.Replace("\r\n", "\n").Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                MatchCollection mc = r.Matches(transarray[0]);
                foreach (Match m in mc)
                {
                    var translation = new Translation
                    {
                        Title = m.Groups[1].Value,
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
                    if (other != string.Empty)
                    {
                        var content = new Content
                        {
                            Title = "備考",
                            Text = other,
                        };
                        contents.Add(content);
                    }
                }
                if (dicword.Exp != string.Empty)
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
                        Id = id++,
                    },
                    Translations = translations,
                    Tags = tags,
                    Contents = contents
                };
                dictionary.Words.Add(word);
            }
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            var json = OneToManyJsonSerializer.Serialize(dictionary, options);
            File.WriteAllText(@"output.json", json);
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
