using PCAxis.Paxiom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCAxis.Search
{
    public interface IIndexer : IDisposable
    {
        void Create(bool createIndex);
        void AddPaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta);
        void UpdatePaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta);
        void Rollback();
    }
}
