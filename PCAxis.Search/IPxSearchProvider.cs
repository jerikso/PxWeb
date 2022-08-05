using PCAxis.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCAxis.Search
{
    public interface IPxSearchProvider
    {
        ISearcher GetSearcher();
        IIndexer GetIndexer();
    }

}
