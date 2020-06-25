using CsvHelper;
using CsvHelper.Configuration;
using Otamajakushi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LineparineOneToManyJsonDictionary
{
    class Program
    {
        static void Main()
        {
            var path = @"lineparine.csv";
            var dicwords = Load(path);
            var dictionary = new OneToManyJson
            {
                Zpdic = new Zpdic
                {
                    AlphabetOrder = "",
                    WordOrderType = "UNICODE",
                    Punctuations = new List<string> { ",", "、" },
                    IgnoredTranslationRegex = @"\(.*\)|（.*）|\[.*\]",
                    PlainInformationTitles = null,
                    InformationTitleOrder = null,
                    FormFontFamily = null,
                    DefaultWord = new Word
                    {
                    },
                }
            };
            foreach (var dicword in dicwords)
            {
                var wc = new WordConverter { DicWord = dicword, DicWords = dicwords };
                var word = wc.Convert();
                dictionary.AddWord(word);
                var subheadings = wc.AddSubheading(dictionary);
                foreach (var subheading in subheadings)
                {
                    if (dictionary.Words.All(w => w.Entry.Id != subheading.Entry.Id))
                        dictionary.AddWord(subheading);
                }
            }
            dictionary.RelationIdCompletion();
            foreach (var item in
                from word in dictionary.Words
                where word.Translations.Count == 0
                select word.Entry.Form)
            {
                Console.WriteLine(item);
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
