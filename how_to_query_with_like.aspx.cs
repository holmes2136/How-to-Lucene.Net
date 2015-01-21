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

public partial class WildCard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BuildIndex();
        Search("*holm*");
    }

    /// <summary>
    /// 搜尋關鍵字
    /// </summary>
    /// <param name="keyWord"></param>
    public void Search(string keyWord) {

        string indexPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\App_Data\\";
        DirectoryInfo dirInfo = new DirectoryInfo(indexPath);
        FSDirectory dir = FSDirectory.Open(dirInfo);
        IndexSearcher search = new IndexSearcher(dir, true);
        // 針對 DESC 欄位進行搜尋
        QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "DESC", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        
        //開啟 leading wildcard 
        parser.AllowLeadingWildcard = true;

        // 搜尋的關鍵字
        Query query = parser.Parse(keyWord);

        // 開始搜尋
        var hits = search.Search(query, null, search.MaxDoc).ScoreDocs;

        foreach (var res in hits)
        {
            Response.Write(string.Format("ID:{0} / DESC{1}",search.Doc(res.Doc).Get("ID").ToString()
                                        ,search.Doc(res.Doc).Get("DESC").ToString().Replace(keyWord, "" + keyWord + "") + ""));
        }


    }


    /// <summary>
    /// 建立索引於 App_Data 資料夾下
    /// </summary>
    public void BuildIndex()
    {
        //從App_Data底下讀入Index檔案 , 若沒有會自動建立
        DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\App_Data");
        FSDirectory dir = FSDirectory.Open(dirInfo);
        IndexWriter iw = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

        //索引兩欄位 , ID 跟 DESC , 其值為 holmes2136 跟 但是Holmes是專業PG
        Document doc = new Document();
        Field field = new Field("ID", "holmes2136", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
        Field field2 = new Field("DESC", "但是Holmes是專業PG", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);

        doc.Add(field);
        doc.Add(field2);
        iw.AddDocument(doc);

        iw.Optimize();
        iw.Commit();
        iw.Close();

    }

}