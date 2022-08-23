using PCAxis.Paxiom;
using PX.SearchAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCAxis.Menu;
using PCAxis.Paxiom.Extensions;
using Nest;


namespace PX.ElasticProvider
{
    public class ElasticIndexer : IIndexer
    {
        private string _indexDirectory;
        private string _database;
        private ElasticClient _client;
        private bool _running;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="indexDirectory">Index directory</param>
        /// <param name="database">Database id</param>
        public ElasticIndexer(ElasticClient client, string indexDirectory, string database)
        {
            _indexDirectory = indexDirectory;
            _database = database;
            _client = client;

        }
        public void AddPaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta)
        {
            Document doc = GetDocument(database, id, path, table, title, published, meta);
            var status = _client.Index(doc, i => i.Index(_indexDirectory));
        }

        public void UpdatePaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta)
        {
            _client.Indices.Delete(path);
            AddPaxiomDocument(database, id, path, table, title, published, meta);

        }

        public void Create(bool createIndex)
        {
        }

        public void Dispose()
        {
        }

        public void End()
        {
            _running = false;
        }

        /// <summary>
        /// Get Document object representing the table
        /// </summary>
        /// <param name="database">Database id</param>
        /// <param name="id">Id of document (table)</param>
        /// <param name="path">Path to table within database</param>
        /// <param name="path">Table</param>
        /// <param name="meta">PXMeta object</param>
        /// <returns>Document object representing the table</returns>
        private Document GetDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta)
        {
            Document doc = new Document();

            if (meta != null)
            {
                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(table) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(meta.Title) || string.IsNullOrEmpty(meta.Matrix) || meta.Variables.Count == 0)
                {
                    return doc;
                }

                doc.docID = id;
                doc.searchID = id;
                doc.Path = path;
                doc.Table = table;
                doc.Database = database;
                doc.Published = published.DateTimeToPxDateString();
                doc.Matrix = meta.Matrix;
                doc.Title = title;
                doc.Variables = string.Join(" ", (from v in meta.Variables select v.Name).ToArray());
                doc.Period = meta.GetTimeValues();
                doc.Values = meta.GetAllValues();
                doc.Codes = meta.GetAllCodes();
                doc.Groupings = meta.GetAllGroupings();
                doc.Valuesets = meta.GetAllValuesets();
                doc.Valuesetcodes = meta.GetAllValuesetCodes();
                doc.TableID = meta.TableID == null ? meta.Matrix : meta.TableID;

                if (!string.IsNullOrEmpty(meta.Synonyms))
                {
                    doc.Synonyms = meta.Synonyms;
                }

            }

            return doc;
        }
    }
}
