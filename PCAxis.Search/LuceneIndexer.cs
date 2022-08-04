using PCAxis.Paxiom;
using PCAxis.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using PCAxis.Menu;
using Lucene.Net.Documents;
using PCAxis.Web.Core.Enums;
using PCAxis.Paxiom.Extensions;


namespace PcAxis.Search
{
    public class LuceneIndexer : IIndexer
    {
        private string _indexDirectory;
        private GetMenuDelegate _menuMethod;
        private string _database;
        private string _language;
        private static log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(Indexer));
        private IndexWriter _writer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="indexDirectory">Index directory</param>
        /// <param name="menuMethod">Delegate method to get the Menu</param>
        /// <param name="database">Database id</param>
        /// <param name="language">Language</param>
        public LuceneIndexer(string indexDirectory, GetMenuDelegate menuMethod, string database, string language)
        {
            _indexDirectory = indexDirectory;
            _menuMethod = menuMethod;
            _database = database;
            _language = language;

        }
        public void AddPaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta)
        {
            Document doc = GetDocument(database, id, path, table, title, published, meta);

            _writer.AddDocument(doc);
        }

        public void UpdatePaxiomDocument(string database, string id, string path, string table, string title, DateTime published, PXMeta meta)
        {
            Document doc = GetDocument(database, id, path, table, title, published, meta);
            _writer.UpdateDocument(new Term(SearchConstants.SEARCH_FIELD_DOCID, doc.Get(SearchConstants.SEARCH_FIELD_DOCID)), doc);
        }

        public void Create(bool createIndex)
        {
            _writer = CreateIndexWriter(createIndex);
            if (createIndex)
            {
                _writer.SetMaxFieldLength(int.MaxValue);
            }
        }

        public void Dispose()
        {
            _writer.Optimize();
            _writer.Dispose();
        }

        public void Rollback()
        {
            _writer.Rollback();
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

                doc.Add(new Field(SearchConstants.SEARCH_FIELD_DOCID, id, Field.Store.YES, Field.Index.NOT_ANALYZED)); // Used as id when updating a document - NOT searchable!!!
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_SEARCHID, id, Field.Store.NO, Field.Index.ANALYZED)); // Used for finding a document by id - will be used for generating URL from just the tableid - Searchable!!!
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_PATH, path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_TABLE, table, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_DATABASE, database, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_PUBLISHED, published.DateTimeToPxDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_MATRIX, meta.Matrix, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_TITLE, title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_VARIABLES, string.Join(" ", (from v in meta.Variables select v.Name).ToArray()), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_PERIOD, meta.GetTimeValues(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_VALUES, meta.GetAllValues(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_CODES, meta.GetAllCodes(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_GROUPINGS, meta.GetAllGroupings(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_GROUPINGCODES, meta.GetAllGroupingCodes(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_VALUESETS, meta.GetAllValuesets(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_VALUESETCODES, meta.GetAllValuesetCodes(), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field(SearchConstants.SEARCH_FIELD_TABLEID, meta.TableID == null ? meta.Matrix : meta.TableID, Field.Store.YES, Field.Index.ANALYZED));
                if (!string.IsNullOrEmpty(meta.Synonyms))
                {
                    doc.Add(new Field(SearchConstants.SEARCH_FIELD_SYNONYMS, meta.Synonyms, Field.Store.NO, Field.Index.ANALYZED));
                }

            }

            return doc;
        }

        /// <summary>
        /// Get Lucene.Net IndexWriter object 
        /// </summary>
        /// <param name="createIndex">If index shall be created (true) or updated (false)</param>
        /// <returns>IndexWriter object. If the Index directory is locked, null is returned</returns>
        private IndexWriter CreateIndexWriter(bool createIndex)
        {
            FSDirectory fsDir = FSDirectory.Open(_indexDirectory);

            if (IndexWriter.IsLocked(fsDir))
            {
                _logger.Error("Index directory " + _indexDirectory + " is locked - cannot write index");
                return null;
            }

            IndexWriter writer = new IndexWriter(fsDir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), createIndex, IndexWriter.MaxFieldLength.LIMITED);
            return writer;
        }
    }
}
