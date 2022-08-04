﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCAxis.Search
{
    public interface ISearcher
    {
        List<SearchResultItem> Search(string text, string filter, int resultListLength);
    }
}
