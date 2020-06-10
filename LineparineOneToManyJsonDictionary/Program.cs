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
        static void Main()
        {
            var path = @"lineparine.csv";
            var dicwords = Load(path);
            var dictionary = new OneToManyJson();
            foreach (var dicword in dicwords)
            {
                var word = (new WordConverter { DicWord = dicword, DicWords = dicwords }).Convert();
                dictionary.AddWord(word);
            }
            dictionary.RelationIdCompletion();
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
