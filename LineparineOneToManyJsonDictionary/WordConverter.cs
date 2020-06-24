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

        public List<DicWord> DicWords { get; set; }

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
            var delete = new List<int>();
            DicWord.Trans = DicWord.Trans
                .Replace("【前置詞】(文語・詩語：【後置詞】)", "【口語前置詞・文語後置詞】")
                .Replace("【後置詞】(口語:【前置詞】)", "【口語前置詞・文語後置詞】");
            foreach (var (line, index) in DicWord.Trans.Split('\n').Select((line, index) => (line, index)))
            {
                var r = new Regex(@"【(.*)】(.*)($|\n)");
                MatchCollection mc = r.Matches(line);
                if (mc.Count == 0)
                {
                    break;
                }
                else
                {
                    foreach (Match m in mc)
                    {
                        Word.Translations.Add(new Translation
                        {
                            Title = m.Groups[1].Value.Replace("】【", "・"),
                            Forms = Regex.Split(m.Groups[2].Value, @"、|\s").ToList(),
                        });
                    }
                    delete.Add(index);
                }
            }
            DicWord.Trans =
                (DicWord.Trans.Split('\n').Length == delete.Count) ?
                string.Empty :
                DicWord.Trans.Split('\n')
                .Select((line, index) => (line, index))
                .Where((line, index) => !delete.Contains(index))
                .Select(taple => taple.line)
                .Aggregate((now, next) => now + "\n" + next);
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
            var wording = new Content { Title = "語法", Text = string.Empty };
            var delete = new List<int>();
            var flag = false;
            foreach (var (line, index) in DicWord.Trans.Split('\n').Select((line, index) => (line, index)))
            {
                if (line == "[語法]")
                    flag = true;
                else if (flag)
                    wording.Text = (wording.Text + "\n" + line).Trim();
                else if (string.IsNullOrEmpty(line))
                    flag = false;
                if (flag)
                    delete.Add(index);
            }
            DicWord.Trans =
                (DicWord.Trans.Split('\n').Length == delete.Count) ?
                string.Empty :
                DicWord.Trans.Split('\n')
                .Select((line, index) => (line, index))
                .TakeWhile((line, index) => !delete.Contains(index))
                .Select(taple => taple.line)
                .Aggregate((now, next) => now + "\n" + next);
            Word.Contents.Add(wording);
            wording = new Content { Title = "語法", Text = string.Empty };
            delete.Clear();
            flag = false;
            foreach (var (line, index) in DicWord.Exp.Split('\n').Select((line, index) => (line, index)))
            {
                if (line == "[語法]")
                    flag = true;
                else if (flag)
                    wording.Text = (wording.Text + "\n" + line).Trim();
                else if (string.IsNullOrEmpty(line))
                    flag = false;
                if (flag)
                    delete.Add(index);
            }
            DicWord.Exp =
                (DicWord.Exp.Split('\n').Length == delete.Count) ?
                string.Empty :
                DicWord.Exp.Split('\n')
                .Select((line, index) => (line, index))
                .TakeWhile((line, index) => !delete.Contains(index))
                .Select(taple => taple.line)
                .Aggregate((now, next) => now + "\n" + next);
            Word.Contents.Add(wording);
            return this;
        }

        protected WordConverter ConvertCulture()
        {
            var culture = new Content { Title = "文化", Text = string.Empty };
            var delete = new List<int>();
            var flag = false;
            foreach (var (line, index) in DicWord.Trans.Split('\n').Select((line, index) => (line, index)))
            {
                if (line == "[文化]")
                    flag = true;
                else if (flag)
                    culture.Text = (culture.Text + "\n" + line).Trim();
                else if (string.IsNullOrEmpty(line))
                    flag = false;
                if (flag)
                    delete.Add(index);
            }
            DicWord.Trans =
                (DicWord.Trans.Split('\n').Length == delete.Count) ?
                string.Empty :
                DicWord.Trans.Split('\n')
                .Select((line, index) => (line, index))
                .TakeWhile((line, index) => !delete.Contains(index))
                .Select(taple => taple.line)
                .Aggregate((now, next) => now + "\n" + next);
            Word.Contents.Add(culture);
            culture = new Content { Title = "文化", Text = string.Empty };
            delete.Clear();
            flag = false;
            foreach (var (line, index) in DicWord.Exp.Split('\n').Select((line, index) => (line, index)))
            {
                if (line == "[文化]")
                    flag = true;
                else if (flag)
                    culture.Text = (culture.Text + "\n" + line).Trim();
                else if (string.IsNullOrEmpty(line))
                    flag = false;
                if (flag)
                    delete.Add(index);
            }
            DicWord.Exp =
                (DicWord.Exp.Split('\n').Length == delete.Count) ?
                string.Empty :
                DicWord.Exp.Split('\n')
                .Select((line, index) => (line, index))
                .TakeWhile((line, index) => !delete.Contains(index))
                .Select(taple => taple.line)
                .Aggregate((now, next) => now + "\n" + next);
            Word.Contents.Add(culture);
            return this;
        }

        protected WordConverter ConvertRemarks()
        {
            Word.Contents.Add(new Content
            {
                Title = "例文",
                Text = DicWord.Exp.Trim(),
            });
            Word.Contents.Add(new Content
            {
                Title = "備考",
                Text = DicWord.Trans.Trim(),
            });
            return this;
        }

        public WordConverter ConvertRelations()
        {
            //{
            //    var relations = new List<Relation>();
            //    foreach (var translation in Word.Translations)
            //    {
            //        if (Regex.IsMatch(translation.Forms, @"cf\. [A-Za-z']+"))
            //        {
            //            var r = new Regex(@"cf\. ([A-Za-z']+)");
            //            MatchCollection mc = r.Matches(translation.Text);
            //            foreach (Match m in mc)
            //            {
            //                if (DicWords.Any(w => w.Word == m.Groups[1].Value))
            //                {
            //                    Word.Relations.Add(new Relation
            //                    {
            //                        Title = string.Empty,
            //                        Entry = new Entry { Form = m.Groups[1].Value },
            //                    });
            //                }
            //            }
            //        }
            //    }
            //}
            {
                var relations = new List<Relation>();
                foreach (var content in Word.Contents)
                {
                    if (Regex.IsMatch(content.Text, @"cf\. [A-Za-z']+"))
                    {
                        var r = new Regex(@"cf\. ([A-Za-z']+)");
                        MatchCollection mc = r.Matches(content.Text);
                        foreach (Match m in mc)
                        {
                            if (DicWords.Any(w => w.Word == m.Groups[1].Value))
                            {
                                Word.Relations.Add(new Relation
                                {
                                    Title = string.Empty,
                                    Entry = new Entry { Form = m.Groups[1].Value },
                                });
                            }
                        }
                    }
                }
            }
            {
                var relations = new List<Relation>();
                var remarks = Word.Contents.Find(c => c.Title == "備考").Text;
                if (Regex.IsMatch(remarks, @"^→[A-Za-z']+"))
                {
                    var r = new Regex(@"→([A-Za-z']+)");
                    MatchCollection mc = r.Matches(remarks);
                    foreach (Match m in mc)
                    {
                        if (DicWords.Any(w => w.Word == m.Groups[1].Value))
                        {
                            Word.Relations.Add(new Relation
                            {
                                Title = string.Empty,
                                Entry = new Entry { Form = m.Groups[1].Value },
                            });
                            Word.Contents.Remove(Word.Contents.Find(c => c.Title == "備考"));
                        }
                    }
                }
            }
            return this;
        }

        public WordConverter Initialization()
        {
            DicWord.Trans = DicWord.Trans.Replace("\r\n", "\n");
            DicWord.Exp = DicWord.Exp.Replace("\r\n", "\n");
            return this;
        }

        public WordConverter FinalAdjustment()
        {
            var delete = new List<Content>();
            foreach (var content in Word.Contents)
            {
                if (string.IsNullOrEmpty(content.Text))
                {
                    delete.Add(content);
                }
            }
            foreach (var content in delete)
            {
                Word.Contents.Remove(content);
            }
            return this;
        }

        public Word Convert()
        {
            return Initialization()
                .ConvertEntry()
                .ConvertTranslations()
                .ConvertTags()
                .ConvertWording()
                .ConvertCulture()
                .ConvertRemarks()
                .ConvertRelations()
                .FinalAdjustment()
                .Word;
        }

        public List<Word> AddSubheading()
        {
            var list = new List<Word>();
            foreach (var content in Word.Contents)
            {
                foreach (var (line, index) in content.Text.Split('\n').Select((line, index) => (line, index)))
                {
                    var r = new Regex(@"【(.*)】\s?([a-zA-Z\.\'\-]+)\s(.*)($|\n)");
                    MatchCollection mc = r.Matches(line);
                    if (mc.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            if (m.Groups[2].Value.Trim() == Word.Entry.Form)
                            {
                                Word.Translations.Add(new Translation
                                {
                                    Title = m.Groups[1].Value.Replace("】【", "・"),
                                    Forms = Regex.Split(m.Groups[3].Value, @"、|\s").ToList(),
                                });
                            }
                            else
                            {
                                var subheading = new Word
                                {
                                    Entry = new Entry
                                    {
                                        Form = m.Groups[2].Value.Trim(),
                                    },
                                    Tags = new List<string>
                                    {
                                        "小見出し",
                                    },
                                    Relations = new List<Relation>
                                    {
                                        new Relation
                                        {
                                            Title = "見出し語",
                                            Entry = new Entry
                                            {
                                                Form = Word.Entry.Form,
                                            }
                                        }
                                    }
                                };
                                subheading.Translations = new List<Translation>();
                                subheading.Translations.Add(new Translation
                                {
                                    Title = m.Groups[1].Value.Replace("】【", "・"),
                                    Forms = Regex.Split(m.Groups[3].Value, @"、|\s").ToList(),
                                });
                                //Word.Relations.Add(new Relation
                                //{
                                //    Title = "小見出し",
                                //    Entry = new Entry
                                //    {
                                //        Form = m.Groups[2].Value.Trim()
                                //    }
                                //});
                                list.Add(subheading);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
