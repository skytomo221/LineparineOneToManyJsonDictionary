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

        CaseGenerator AddNominative(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "主格", Form = word + "'s", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "主格", Form = word + BufferSound(word) + "'s", });
            }
            return this;
        }

        CaseGenerator AddAccusation(string word, string option)
        {
            Word.Variations.Add(new Variation { Title = option + "対格", Form = word + "'i", });
            return this;
        }

        CaseGenerator AddGenitive(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "f", "r", "n", "l", "l", "v", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "属格", Form = word + "'d", });
            }
            if ((new string[] { "p", "f", "t", "c", "x", "k", "m", "n", "l", "j", "d", "s", "g", "ch", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "属格", Form = word + BufferSound(word) + "'d", });
            }
            return this;
        }

        CaseGenerator AddDignity(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "p", "r", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "与格", Form = word + "'c", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "d", "s", "g", "dz", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "与格", Form = word + BufferSound(word) + "'c", });
            }
            return this;
        }

        CaseGenerator AddLocative(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "k", "r", "l", "j", "g", "ch", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "向格", Form = word + "'l", });
            }
            if ((new string[] { "p", "t", "c", "x", "k", "m", "n", "l", "j", "v", "d", "s", "ch", "th", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "向格", Form = word + BufferSound(word) + "'l", });
            }
            return this;
        }

        CaseGenerator AddVocative(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "呼格", Form = word + "sti", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "呼格", Form = word + BufferSound(word) + "sti", });
            }
            return this;
        }

        CaseGenerator AddAbessive(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "欠格", Form = word + "nerfe", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "欠格", Form = word + BufferSound(word) + "nerfe", });
            }
            return this;
        }

        CaseGenerator AddComitative(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "共格", Form = word + "'tj", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "共格", Form = word + BufferSound(word) + "'tj", });
            }
            return this;
        }

        CaseGenerator AddTopical(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "話題格", Form = word + "'sci", });
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                Word.Variations.Add(new Variation { Title = option + "話題格", Form = word + BufferSound(word) + "'sci", });
            }
            return this;
        }

        CaseGenerator AddSingularForm(string word, string option)
        {
            return AddNominative(word, option)
                  .AddAccusation(word, option)
                  .AddGenitive(word, option)
                  .AddDignity(word, option)
                  .AddLocative(word, option)
                  .AddVocative(word, option)
                  .AddAbessive(word, option)
                  .AddComitative(word, option)
                  .AddTopical(word, option);
        }

        CaseGenerator AddPluralForm(string word, string option)
        {
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                var plural = word + "ss";
                Word.Variations.Add(new Variation { Title = option + "複数形", Form = plural });
                AddSingularForm(plural, option + "複数形");
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                var plural = word + BufferSound(word) + "ss";
                Word.Variations.Add(new Variation { Title = option + "複数形", Form = plural });
                AddSingularForm(plural, option + "複数形");
            }
            return this;
        }

        CaseGenerator AddFinite(string word, string option)
        {
            {
                var finite = "gir" + word;
                Word.Variations.Add(new Variation { Title = option + "定性", Form = finite });
                AddSingularForm(finite, "定性");
                AddPluralForm(finite, "定性");
            }
            if ((new string[] { "i", "y", "u", "o", "e", "a", "r", "j", "rl", }).Contains(LastLetter(word)))
            {
                var finite = word + "stan";
                Word.Variations.Add(new Variation { Title = option + "定性", Form = finite });
                AddSingularForm(finite, "定性");
                var indefinite = word + "ste";
                Word.Variations.Add(new Variation { Title = option + "不定性", Form = indefinite });
                AddSingularForm(indefinite, "不定性");
                AddPluralForm(finite, "定性");
                AddPluralForm(indefinite, "不定性");
            }
            if ((new string[] { "p", "f", "t", "x", "k", "z", "m", "n", "l", "j", "v", "d", "s", "g", "dz", "ch", "rl", }).Contains(LastLetter(word)))
            {
                var finite = word + BufferSound(word) + "stan";
                Word.Variations.Add(new Variation { Title = option + "定性", Form = finite });
                AddSingularForm(finite, "定性");
                var indefinite = word + BufferSound(word) + "ste";
                Word.Variations.Add(new Variation { Title = option + "不定性", Form = indefinite });
                AddSingularForm(indefinite, "不定性");
                AddPluralForm(finite, "定性");
                AddPluralForm(indefinite, "不定性");
            }
            return this;
        }
        public Word AddCase()
        {
            if (Word.Translations.Any(t => t.Title == "名詞")
                && !Regex.IsMatch(Word.Entry.Form, @"\s|[A-Z\.]")
                && !string.IsNullOrEmpty(LastVowelLetter(Word.Entry.Form)))
                return AddSingularForm(Word.Entry.Form, "単数形")
                      .AddPluralForm(Word.Entry.Form, string.Empty)
                      .AddFinite(Word.Entry.Form, string.Empty)
                      .Word;
            else
                return Word;
        }
    }
}
