using System;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Lucene_Examples.Lesson_1
{
    public class LuceneTest
    {
        public LuceneTest()
        {
            #region Init

            Directory directory = new RAMDirectory();

			var analyzer = new StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);

			var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzer);

			IndexWriter indexWriter = new IndexWriter(directory, config);

			//indexWriter.Commit();

            #endregion

			#region Add Docs to Index

			Console.WriteLine("Document Contents:");

			Document doc1 = new Document();

			string text = "Lucene is an Information Retrieval library written in Java. Here's some extra document text.";

			Console.WriteLine(text);

			doc1.Add(new Int32Field("id", 1, Field.Store.YES));

			doc1.Add(new TextField("Content", text, Field.Store.YES));

			indexWriter.AddDocument(doc1);

			Document doc2 = new Document();

			var text2 = "This text belongs in document number 2.";

			Console.WriteLine(text2);

			doc1.Add(new Int32Field("id", 2, Field.Store.YES));

			doc2.Add(new TextField("Content", text2, Field.Store.YES));

			indexWriter.AddDocument(doc2);

			#endregion

			#region Close All the Things (This doesn't work in 4.8.0 on Nuget)

			indexWriter.Dispose();

			#endregion

			Console.WriteLine("Enter Search Term:");

			string search_term = Console.ReadLine();

			var indexReader = DirectoryReader.Open(directory, 100);

			var indexSearcher = new IndexSearcher(indexReader);

			QueryParser parser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "Content", analyzer);

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

		}
    }
}
