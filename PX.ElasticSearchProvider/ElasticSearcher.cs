using PX.SearchAbstractions;
using System;
using System.Collections.Generic;
using PCAxis.Paxiom.Extensions;
using Nest;

namespace PX.ElasticProvider
{
    public class ElasticSearcher : ISearcher
    {
        ElasticClient _client;
        DateTime _creationTime;
        Operator _defaultOperator;
        string _path;
        public ElasticSearcher(ElasticClient client, string path)
        {
            _client = client;
            _creationTime = DateTime.Now;
            _path = path;
            _defaultOperator = Operator.And;
        }
        public DateTime CreationTime { get { return _creationTime; } }

        public List<SearchResultItem> Search(string text, string filter, int resultListLength, out SearchStatusType status)
        {
            List<SearchResultItem> searchResult = new List<SearchResultItem>();
            string[] fields = GetSearchFields(filter);

            var searchResponse = _client.Search<Document>(s => s
                .From(0)
                .Size(resultListLength)
                .Index(_path)
                .Query(q => q
                    .QueryString(m => m
                        .Fields(fields)
                        .Query(text)
                        .DefaultOperator(_defaultOperator)
                    )
                    
                )
            );

            foreach (Document d in searchResponse.Documents)
            {
                searchResult.Add(new SearchResultItem()
                {
                    Path = d.Path,
                    Table = d.Table,
                    Title = d.Title,
                    Score = d.Score,
                    Published = GetPublished(d),
                });
            }

            status = SearchStatusType.Successful;
            return searchResult;
        }

        public void SetDefaultOperator(DefaultOperator defaultOperator)
        {
            if (defaultOperator == DefaultOperator.OR)
            {
                _defaultOperator = Operator.Or;
            }
            else
            {
                _defaultOperator = Operator.And;
            }
        }

        /// <summary>
        /// Get fields in index to search in
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string[] GetSearchFields(string filter)
        {
            string[] fields;


            if (string.IsNullOrEmpty(filter))
            {
                // Default fields
                fields = new[] { "searchid",
                                 "title",
                                 "values",
                                 "codes",
                                 "matrix",
                                 "variables",
                                 "period",
                                 "groupings",
                                 "groupingcodes",
                                 "valuesets",
                                 "valuesetcodes",
                                 "synonyms",
                };
            }
            else
            {
                // Get fields from filter
                fields = filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return fields;
        }
        private DateTime GetPublished(Document doc)
        {
            DateTime published = DateTime.MinValue;
            string publishedStr = doc.Published;

            if (!string.IsNullOrEmpty(publishedStr))
            {
                if (PxDate.IsPxDate(publishedStr))
                {
                    published = publishedStr.PxDateStringToDateTime();
                }
            }

            return published;
        }
    }
}