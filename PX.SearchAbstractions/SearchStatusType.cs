using System;
using System.Collections.Generic;
using System.Text;

namespace PX.SearchAbstractions
{
    /// <summary>
    /// Describes status of a search attempt
    /// </summary>
    public enum SearchStatusType
    {
        // A successful search has been made
        Successful,
        // No search index existed for the database/language
        NotIndexed
    }
}
