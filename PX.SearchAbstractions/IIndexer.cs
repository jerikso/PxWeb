using PCAxis.Paxiom;
using System;

namespace PX.SearchAbstractions
{
    public interface IIndexer : IDisposable
    {
        void Create(bool createIndex);
        void AddPaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta);
        void UpdatePaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta);
        void End();
    }
}
