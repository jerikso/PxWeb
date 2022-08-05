namespace PX.SearchAbstractions
{
    public interface IPxSearchProvider
    {
        ISearcher GetSearcher();
        IIndexer GetIndexer();
    }

}
