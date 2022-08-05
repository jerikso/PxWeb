using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCAxis.Search
{
    public interface ISearcher
    {
        DateTime CreationTime { get; }
        List<SearchResultItem> Search(string text, string filter, int resultListLength, out SearchStatusType status);
        void SetDefaultOperator(DefaultOperator defaultOperator);
    }
}
