using PX.SearchAbstractions;
using System.IO;
using System.Web;
using System.Text;
using Nest;
using System;

namespace PX.ElasticProvider
{
    public class ElasticSearchProvider : IPxSearchProvider
    {
        private string _database;
        private string _language;
        private DirectoryInfo _databaseBaseDirectory;
        private ElasticClient _client;
        public ElasticSearchProvider(string databaseBaseDirectory, string database, string language) {
            _database = database;
            _language = language;
            _databaseBaseDirectory = GetDatabaseBaseDirectory(databaseBaseDirectory);

            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("default");
            _client = new ElasticClient(settings);


        }


        public IIndexer GetIndexer()
        {
            string path = GetIndexDirectoryPath();
            return new ElasticIndexer(_client, path, _database);
        }

        public ISearcher GetSearcher()
        {
            string path = GetIndexDirectoryPath();
            return new ElasticSearcher(_client, path);
        }

        /// <summary>
        /// Set the index base directory
        /// </summary>
        /// <param name="indexDirectory">Base directory for all search indexes</param>
        private DirectoryInfo GetDatabaseBaseDirectory(string databaseBaseDirectory)
        {
            if (System.IO.Directory.Exists(databaseBaseDirectory))
            {
                return new DirectoryInfo(databaseBaseDirectory);
            }
            return null;
        }

        /// <summary>
        /// Get path to the specified index directory 
        /// </summary>
        /// <param name="database">database</param>
        /// <param name="language">language</param>
        /// <returns></returns>
        private string GetIndexDirectoryPath()
        {
            StringBuilder dir = new StringBuilder("");

            dir.Append(_database.Replace("/","_"));
            dir.Append("_");
            dir.Append(_language);

            return dir.ToString().ToLower();
        }
    }
}
