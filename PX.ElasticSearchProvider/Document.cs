using System;
using System.Collections.Generic;
using System.Text;

namespace PX.ElasticProvider
{
    internal class Document
    {
        internal string docID;
        internal string searchID;

        public string Path { get; set; }
        public string Title { get; set; }
        public float Score { get; set; }
        public string Table { get; set; }
        public string Published { get; set; }
        public string Database { get; internal set; }
        public string Matrix { get; internal set; }
        public string Variables { get; internal set; }
        public string Period { get; internal set; }
        public string Values { get; internal set; }
        public string Codes { get; internal set; }
        public string Groupings { get; internal set; }
        public string Valuesets { get; internal set; }
        public object Valuesetcodes { get; internal set; }
        public string TableID { get; internal set; }
        public string Synonyms { get; internal set; }
    }
}
