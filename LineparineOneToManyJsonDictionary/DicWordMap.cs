using CsvHelper.Configuration;

namespace LineparineOneToManyJsonDictionary
{
    public class DicWordMap : ClassMap<DicWord>
    {
        public DicWordMap()
        {
            Map(m => m.Word).Name("word");
            Map(m => m.Trans).Name("trans");
            Map(m => m.Exp).Name("exp");
            Map(m => m.Level).Name("level");
            Map(m => m.Memory).Name("memory");
            Map(m => m.Modify).Name("modify");
            Map(m => m.Pron).Name("pron");
            Map(m => m.Filelink).Name("filelink");
        }
    }
}
