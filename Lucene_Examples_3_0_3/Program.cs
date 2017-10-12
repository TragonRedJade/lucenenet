using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace Lucene_Examples_3_0_3
{
    class MainClass
    {
        public static void Main(string[] args)
        {
			#region Init

			Directory directory = new RAMDirectory();

			Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

			//Analyzer analyzer = new WhitespaceAnalyzer();

			//Analyzer analyzer = new Lucene.Net.Analysis.Snowball.SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "snowball analyzer");

			IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

			#endregion

			#region Add Docs to Index

			Console.WriteLine("Document Contents:");

			Document doc1 = new Document();

			var text = "Lucene is an Information Retrieval library written in Java. Here's some extra document text.";

			Console.WriteLine(text);

			//doc1.Add(new NumericField("id").SetIntValue(1));

			doc1.Add(new Field("Content", text, Field.Store.YES, Field.Index.ANALYZED));

			indexWriter.AddDocument(doc1);

			Document doc2 = new Document();

			var text2 = "This text belongs in document number 2.";

			Console.WriteLine(text2);

			//doc2.Add(new NumericField("id").SetIntValue(2));

			doc2.Add(new Field("Content", text2, Field.Store.YES, Field.Index.ANALYZED));

			indexWriter.AddDocument(doc2);

			#endregion

			#region Close All the Things

			//indexWriter.Optimize();

			//indexWriter.Flush(true, true, true);

			//indexWriter.Dispose(true);

			#endregion

			#region Search for a Search Term

			Console.WriteLine("Enter Search Term:");

			string search_term = Console.ReadLine();

			var indexReader = IndexReader.Open(directory, true);

			IndexSearcher indexSearcher = new IndexSearcher(indexReader);

			QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Content", analyzer);

			Query query = parser.Parse(search_term);

			int hitsPerPage = 10;

			TopDocs docs = indexSearcher.Search(query, hitsPerPage);

			ScoreDoc[] hits = docs.ScoreDocs;

			int end = Math.Min(docs.TotalHits, hitsPerPage);

			Console.WriteLine("Total Hits: " + docs.TotalHits);

			Console.WriteLine("Results: ");

			for (int i = 0; i < end; i++)
			{
				Document d = indexSearcher.Doc(hits[i].Doc);

				Console.WriteLine("Content: " + d.Get("Content"));
			}

			directory.Dispose();

			#endregion
		}
    }
}
