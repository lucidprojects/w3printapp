/*
 * Class Config
 * Works with application configuration (queries, subqueries, settings).
 */

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    /*
     * Class Query
     * Works with query data.
     */
    public class Query
    {
        private readonly string _path;

        public Query(string aTitle, string aPath)
        {
            Title = aTitle;
            _path = aPath;
        }

        public string Title { get; }

        public string Text => File.ReadAllText(_path);

        public List<Query> SubqueryList { get; } = new List<Query>();

        public void addSubquery(Query subquery)
        {
            SubqueryList.Add(subquery);
        }
    }

    public class Config
    {
        private readonly Settings _settings = new Settings();

        public Config()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            loadQueries();
        }

        public List<Query> QueryList { get; } = new List<Query>();

        public string LabelServiceUrl => _settings.LabelServiceUrl;

        public string UnshipSinglePackageDataQuery =>
            File.ReadAllText(Application.StartupPath + "\\" + _settings.UnshipSinglePackageDataQuery);

        public string UnshipBatchPackageDataQuery =>
            File.ReadAllText(Application.StartupPath + "\\" + _settings.UnshipBatchPackageDataQuery);

        public string UnshippedQuery => File.ReadAllText(Application.StartupPath + "\\" + _settings.UnshippedQuery);

        public string ReprintSinglePackageDataQuery =>
            File.ReadAllText(Application.StartupPath + "\\" + _settings.ReprintSinglePackageDataQuery);

        public string ReprintBatchPackageDataQuery =>
            File.ReadAllText(Application.StartupPath + "\\" + _settings.ReprintBatchPackageDataQuery);

        public string RepairQuery => File.ReadAllText(Application.StartupPath + "\\" + _settings.RepairQuery);

        public string LastBatchesQuery => File.ReadAllText(Application.StartupPath + "\\" + _settings.LastBatchesQuery);

        private void loadQueries()
        {
            var readerSettings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            var reader = XmlReader.Create(Application.StartupPath + "\\queries.xml", readerSettings);

            //reader.MoveToContent();

            Query query = null;
            var queriesDir = "";
            var subqueriesDir = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "queries") queriesDir = reader.GetAttribute("dir");

                        if (reader.Name == "query")
                            query = new Query(reader.GetAttribute("title"),
                                Application.StartupPath + (queriesDir.Length == 0 ? "" : "\\") + queriesDir + "\\" +
                                reader.GetAttribute("file"));

                        if (reader.Name == "subqueries") subqueriesDir = reader.GetAttribute("dir");

                        if (reader.Name == "subquery")
                            query.addSubquery(new Query(reader.GetAttribute("title"),
                                Application.StartupPath + (queriesDir.Length == 0 ? "" : "\\") + queriesDir +
                                (subqueriesDir.Length == 0 ? "" : "\\") + subqueriesDir + "\\" +
                                reader.GetAttribute("file")));
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == "query") QueryList.Add(query);
                        break;
                }
            }
        }
    }
}