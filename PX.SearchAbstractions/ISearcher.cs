using System;
using System.Collections.Generic;

namespace PX.SearchAbstractions
{
    public interface ISearcher
    {
        DateTime CreationTime { get; }
        List<SearchResultItem> Search(string text, string filter, int resultListLength, out SearchStatusType status);
        void SetDefaultOperator(DefaultOperator defaultOperator);
    }
}
