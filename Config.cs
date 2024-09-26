/*
 * Class Config
 * Works with application configuration (queries, subqueries, settings).
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace PrintInvoice
{
  /*
   * Class Query
   * Works with query data.
   */
  public class Query
  {
    private string title;
    private string path;
    private List<Query> subqueryList = new List<Query>();

    public Query(string aTitle, string aPath)
    {
      title = aTitle;
      path = aPath;
    }

    public string Title
    {
      get { return title; }
    }

    public string Text
    {
      get 
      {
        return (System.IO.File.ReadAllText(path));
      }
    }

    public void addSubquery(Query subquery)
    {
      subqueryList.Add(subquery);
    }

    public List<Query> SubqueryList
    {
      get { return subqueryList; }
    }
  }

  public class Config
  {
    private List<Query> queryList = new List<Query>();
    private Properties.Settings settings = new Properties.Settings();

    public Config()
    {
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      loadQueries();
    }

    public List<Query> QueryList
    {
      get { return queryList; }
    }

    private void loadQueries()
    {
      XmlReaderSettings readerSettings = new XmlReaderSettings();
      readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
      readerSettings.IgnoreWhitespace = true;
      readerSettings.IgnoreComments = true;
      XmlReader reader = XmlReader.Create(Application.StartupPath + "\\queries.xml", readerSettings);

      //reader.MoveToContent();

      Query query = null;
      string queriesDir = "";
      string subqueriesDir = "";
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            if (reader.Name == "queries")
            {
              queriesDir = reader.GetAttribute("dir");
            }

            if (reader.Name == "query")
            {
              query = new Query(reader.GetAttribute("title"), Application.StartupPath + (queriesDir.Length == 0 ? "" : "\\") + queriesDir + "\\" + reader.GetAttribute("file"));
            }

            if (reader.Name == "subqueries")
            {
              subqueriesDir = reader.GetAttribute("dir");
            }

            if (reader.Name == "subquery")
            {
              query.addSubquery(new Query(reader.GetAttribute("title"), Application.StartupPath + (queriesDir.Length == 0 ? "" : "\\") + queriesDir + (subqueriesDir.Length == 0 ? "" : "\\") + subqueriesDir + "\\" + reader.GetAttribute("file")));
            }
            break;

          case XmlNodeType.EndElement:
            if (reader.Name == "query")
            {
              queryList.Add(query);
            }
            break;
        }
      }
    }

    public string LabelServiceUrl
    {
      get { return settings.LabelServiceUrl; }
    }

    public string UnshipSinglePackageDataQuery
    {
      get {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.UnshipSinglePackageDataQuery));       
      }
    }

    public string UnshipBatchPackageDataQuery
    {
      get
      {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.UnshipBatchPackageDataQuery));
      }
    }

    public string UnshippedQuery
    {
      get { return (File.ReadAllText(Application.StartupPath + "\\" + settings.UnshippedQuery)); }
    }

    public string ReprintSinglePackageDataQuery
    {
      get
      {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.ReprintSinglePackageDataQuery));
      }
    }

    public string ReprintBatchPackageDataQuery
    {
      get
      {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.ReprintBatchPackageDataQuery));
      }
    }

    public string RepairQuery
    {
      get
      {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.RepairQuery));
      }
    }

    public string LastBatchesQuery
    {
      get
      {
        return (File.ReadAllText(Application.StartupPath + "\\" + settings.LastBatchesQuery));
      }
    }
  }
}
