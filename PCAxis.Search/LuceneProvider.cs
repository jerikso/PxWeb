using PCAxis.Search;

namespace PcAxis.Search
{
    public class LuceneProvider : IPxSearchProvider
    {
        public IIndexer GetIndexer(string indexDirectory, GetMenuDelegate menuMethod, string database, string language)
        {
            return new LuceneIndexer(indexDirectory, menuMethod, database, language);
        }

        public ISearcher GetSearcher(string indexDirectory)
        {
            return new LuceneSearcher(indexDirectory);
        }
    }
}
