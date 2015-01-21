using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

public partial class Build_Index_By_RAMDirectory : System.Web.UI.Page
{

    static RAMDirectory dir = new RAMDirectory();


    protected void Page_Load(object sender, EventArgs e)
    {
        BuildIndex();
        Search("holm*");
    }


     public void Search(string KeyWord)
    {

        IndexSearcher search = new IndexSearcher(dir, true);

        QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Name", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

        Query query = parser.Parse(KeyWord);

        var hits = search.Search(query, null, search.MaxDoc).ScoreDocs;

        foreach (var res in hits)
        {
            Response.Write(string.Format("ID：{0} / DESC：{1}"
                                        , search.Doc(res.Doc).Get("ID").ToString()
                                        , search.Doc(res.Doc).Get("Name").ToString() + "<BR>"));
        }
        
    }


    private void BuildIndex()
    {

        IndexWriter iw = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

        for (int i = 1; i <= 1000; i++)
        {

            Document doc = new Document();
            Field field = new Field("ID", Guid.NewGuid().ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
            Field field2 = new Field("Name", "holmes" + i.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
            doc.Add(field);
            doc.Add(field2);
            iw.AddDocument(doc);
        }

        iw.Optimize();
        iw.Commit();
        iw.Close();
    }

}