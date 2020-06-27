using CsvHelper;
using CsvHelper.Configuration;
using Otamajakushi;
using ShellProgressBar;
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
            int totalTicks = dicwords.Count;
            var progressBarOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '.',
                ProgressCharacter = '#',
            };
            var childProgressBarOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Green,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '.',
                ProgressCharacter = '#',
                CollapseWhenFinished = false,
            };
            using (var pbar = new ProgressBar(5, "Total Progress", progressBarOptions))
            using (var pbar5 = pbar.Spawn(1, "Output to ./output.json", childProgressBarOptions))
            using (var pbar4 = pbar.Spawn(1, "Relation ID completion", childProgressBarOptions))
            using (var pbar3 = pbar.Spawn(totalTicks, "Register cases", childProgressBarOptions))
            using (var pbar2 = pbar.Spawn(totalTicks, "Register subheadings", childProgressBarOptions))
            using (var pbar1 = pbar.Spawn(totalTicks, "Register words", childProgressBarOptions))
            {
                foreach (var dicword in dicwords)
                {
                    var wc = new WordConverter { DicWord = dicword, DicWords = dicwords };
                    var word = wc.Convert();
                    dictionary.AddWord(word);
                    var subheadings = wc.AddSubheading(dictionary);
                    pbar1.Tick($"Register \"{word.Entry.Form}\" in the dictionary");
                    foreach (var subheading in subheadings)
                    {
                        if (dictionary.Words.All(w => w.Entry.Id != subheading.Entry.Id))
                            dictionary.AddWord(subheading);
                    }
                    pbar2.Tick($"Register \"{word.Entry.Form}\"'s subheadings in the dictionary");
                }
                pbar.Tick("Register words");
                pbar.Tick("Register subheadings");
                pbar3.MaxTicks = dictionary.Words.Count;
                var casegen = new CaseGenerator();
                foreach (var word in dictionary.Words)
                {
                    casegen.Word = word;
                    casegen.AddCase();
                    pbar3.Tick($"Register \"{word.Entry.Form}\"'s cases in the dictionary");
                }
                pbar.Tick("Register cases");
                dictionary.RelationIdCompletion();
                pbar4.Tick("Relation ID completion");
                pbar.Tick("Relation ID completion");
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true,
                };
                var json = OneToManyJsonSerializer.Serialize(dictionary, options);
                File.WriteAllText(@"output.json", json);
                pbar5.Tick("Output to ./output.json");
                pbar.Tick("Output to ./output.json");
            }
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
