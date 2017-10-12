using System;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Queries;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Lucene_Examples.Lesson_1
{
    public class LuceneTestUsing
    {
        public LuceneTestUsing()
        {
			Directory directory = new RAMDirectory();

			using (var analyzerU = new StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48))
			{
				var configU = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzerU);

				using (var indexWriterU = new IndexWriter(directory, configU))
				{
					Document doc1U = new Document();

					Document doc2U = new Document();

					string textU = "Lucene is an Information Retrieval library written in Java. Here's some extra document text.";

					doc1U.Add(new TextField("Content", textU, Field.Store.YES));

					indexWriterU.AddDocument(doc1U);

					string text2U = "Lucene is an Information Retrieval library written in Java. Here's some extra document text.";

					doc2U.Add(new TextField("Content", text2U, Field.Store.YES));

					indexWriterU.AddDocument(doc2U);

					indexWriterU.Dispose(true);
				}

				var indexReader = DirectoryReader.Open(directory);

				var indexSearcher = new IndexSearcher(indexReader);

                QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "Content", analyzerU);

                TopDocs docs = indexSearcher.Search(parser.Parse("text"), 10);
			}
        }
    }
}
