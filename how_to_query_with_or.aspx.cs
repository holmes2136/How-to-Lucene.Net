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

public partial class how_to_query_with_or : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BuildIndex();

        Search("ID:holmes ID:sherlock");
        //SearchByBQ("holmes");
    }


   public void SearchByBQ(string keyWord)
    {

        string indexPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\App_Data\\";
        DirectoryInfo dirInfo = new DirectoryInfo(indexPath);
        FSDirectory dir = FSDirectory.Open(dirInfo);
        IndexSearcher search = new IndexSearcher(dir, true);
        QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "DESC", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

        BooleanQuery booleanQuery = new BooleanQuery();
        Query query1 = new TermQuery(new Term("ID", "holmes"));
        Query query2 = new TermQuery(new Term("ID", "sherlock"));

        //Occur.Should 表示 Or , Must 表示 and 運算子
        booleanQuery.Add(query1,Occur.SHOULD);
        booleanQuery.Add(query2, Occur.SHOULD);
        Query query = parser.Parse(keyWord);

        // 開使搜尋
        var hits = search.Search(query, null, search.MaxDoc).ScoreDocs;

        foreach (var res in hits)
        {
            Response.Write(string.Format("ID:{0} / DESC{1}"
                                        , search.Doc(res.Doc).Get("ID").ToString()
                                        , search.Doc(res.Doc).Get("DESC").ToString() + "<BR>"));
        }
    }

   

    public void Search(string KeyWord) {

        string indexPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\App_Data\\";
        DirectoryInfo dirInfo = new DirectoryInfo(indexPath);
        FSDirectory dir = FSDirectory.Open(dirInfo);
        IndexSearcher search = new IndexSearcher(dir, true);
        // 針對 DESC 欄位進行搜尋
        QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "DESC", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        // 搜尋的關鍵字
        Query query = parser.Parse(KeyWord);
        // 開使搜尋
        var hits = search.Search(query, null, search.MaxDoc).ScoreDocs;

        foreach (var res in hits)
        {
            Response.Write(string.Format("ID:{0} / DESC{1}",search.Doc(res.Doc).Get("ID").ToString()
                                        ,search.Doc(res.Doc).Get("DESC").ToString() + "<BR>"));
        }

    }


    
    public void BuildIndex()
    {
        //從App_Data底下讀入Index檔案 , 若沒有會自動建立
        DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\App_Data");
        FSDirectory dir = FSDirectory.Open(dirInfo);
        IndexWriter iw = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

        //這裡將會寫進一份文件,而文件包含許多field(屬性),你可以決定這些屬性是否需要被索引

        Document doc = new Document();
        Field field = new Field("ID", "holmes", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
        Field field2 = new Field("DESC", "但是holmes是專業PG", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
        doc.Add(field);
        doc.Add(field2);
        iw.AddDocument(doc);

        Document doc1 = new Document();
        Field field3 = new Field("ID", "sherlock", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
        Field field4 = new Field("DESC", "但是Sherlock是專業PG", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
        doc1.Add(field3);
        doc1.Add(field4);
        iw.AddDocument(doc1);

        iw.Optimize();
        iw.Commit();

        //IndexWriter有實作IDisposable , 
        //表示握有外部資源,所以記的得Close
        iw.Close();

       

    }


}