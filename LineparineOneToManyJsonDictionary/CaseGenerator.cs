using Otamajakushi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LineparineOneToManyJsonDictionary
{
    class CaseGenerator
    {
        public Word Word { get; set; }

        ReadOnlyCollection<string> Vowels = Array.AsReadOnly(new string[] { "i", "y", "u", "o", "e", "a" });
        ReadOnlyCollection<string> Consonants = Array.AsReadOnly(new string[] {
            "p", "fh", "f", "t", "c", "x",
            "k", "q", "h", "r", "z", "m",
            "n", "r", "l", "j", "w", "b",
            "vh", "v", "d", "s", "g", "dz",
            "ph", "ts", "ch", "ng", "sh",
            "th", "dh", "kh", "rkh", "rl",
        });

        static string FirstLetter(string word)
        {
            var letters = new List<string> {
                "fh", "vh", "dz", "ph", "ts", "ch", "ng", "sh", "th", "dh", "kh", "rkh", "rl",
                "i", "y", "u", "o", "e", "a",
                "p", "f", "t", "c", "x", "k", "q", "h", "r", "z", "m", "n", "r", "l", "j", "w", "b", "v", "d", "s", "g", };
            foreach (var letter in letters)
            {
                if (word.Replace("-", string.Empty).Replace("'", string.Empty).StartsWith(letter))
                {
                    return letter;
                }
            }
            return word;
        }

        static string LastLetter(string word)
        {
            var letters = new List<string> {
                "fh", "vh", "dz", "ph", "ts", "ch", "ng", "sh", "th", "dh", "kh", "rkh", "rl",
                "i", "y", "u", "o", "e", "a",
                "p", "f", "t", "c", "x", "k", "q", "h", "r", "z", "m", "n", "r", "l", "j", "w", "b", "v", "d", "s", "g", };
            foreach (var letter in letters)
            {
                if (word.Replace("-", string.Empty).Replace("'", string.Empty).EndsWith(letter))
                {
                    return letter;
                }
            }
            return word;
        }

        string LastVowelLetter(string word) => LastLetter(Regex.Replace(word, "[^iyuoea]", string.Empty));

        string BufferSound(string word)
        {
            var vowel = LastVowelLetter(word)[0];
            if (Consonants.Contains(LastLetter(word)))
                switch (vowel)
                {
                    case 'a':
                    case 'o':
                        return "a";
                    case 'e':
                    case 'i':
                        return "e";
                    case 'u':
                        return "u";
                    case 'y':
                        return "i";
                    default:
                        throw new Exception();
                }
            else
                switch (vowel)
                {
                    case 'a':
                    case 'o':
                        return "v";
                    case 'e':
                    case 'i':
                        return "rg";
                    case 'u':
                        return "m";
                    case 'y':
                        return "l";
                    default:
                        throw new Exception();
                }
        }

        CaseGenerator AddNominative()
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "主格", Form = Word.Entry.Form + "'s", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "主格", Form = Word.Entry.Form + BufferSound(Word.Entry.Form) + "'s", });
            }
            return this;
        }

        CaseGenerator AddAccusation()
        {
            Word.Variations.Add(new Variation { Title = "対格", Form = Word.Entry.Form + "'i", });
            return this;
        }

        CaseGenerator AddGenitive()
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "f", "r", "n", "l", "l", "v", "ch", "rl", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "属格", Form = Word.Entry.Form + "'d", });
            }
            if ((new string[] { "p", "f", "t", "c", "x", "k", "m", "n", "l", "j", "d", "s", "g", "ch", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "属格", Form = Word.Entry.Form + BufferSound(Word.Entry.Form) + "'d", });
            }
            return this;
        }

        CaseGenerator AddDignity()
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "p", "r", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "与格", Form = Word.Entry.Form + "'c", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "d", "s", "g", "dz", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "与格", Form = Word.Entry.Form + BufferSound(Word.Entry.Form) + "'c", });
            }
            return this;
        }

        CaseGenerator AddLocative()
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "k", "r", "l", "j", "g", "ch", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "方向格", Form = Word.Entry.Form + "'l", });
            }
            if ((new string[] { "p", "t", "c", "x", "k", "m", "n", "l", "j", "v", "d", "s", "ch", "th", "rl", }).Contains(LastLetter(Word.Entry.Form)))
            {
                Word.Variations.Add(new Variation { Title = "方向格", Form = Word.Entry.Form + BufferSound(Word.Entry.Form) + "'l", });
            }
            return this;
        }

        public Word AddCase()
        {
            if (Word.Translations.Any(t => t.Title == "名詞") && !string.IsNullOrEmpty(LastVowelLetter(Word.Entry.Form)))
                return AddNominative()
                      .AddAccusation()
                      .AddGenitive()
                      .AddDignity()
                      .AddLocative()
                      .Word;
            else
                return Word;
        }
    }
}
