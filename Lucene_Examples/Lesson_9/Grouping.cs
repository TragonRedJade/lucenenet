using System;
using System.Collections.Generic;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Search.Grouping;

namespace Lucene_Examples.Lesson_9
{
    public class Grouping
    {
        public Grouping()
        {
			#region Init

			Directory directory = new RAMDirectory();

			var analyzer = new StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);

			var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzer);

			IndexWriter indexWriter = new IndexWriter(directory, config);

            #endregion

            #region Add Docs to Index

			FieldType groupEndFieldType = new FieldType();

            groupEndFieldType.IsStored = false;

            groupEndFieldType.IsTokenized = false;

            groupEndFieldType.IsIndexed = true;

            groupEndFieldType.IndexOptions = IndexOptions.DOCS_ONLY;

            groupEndFieldType.OmitNorms = true;

            Field groupEndField = new Field("groupEnd", "x", groupEndFieldType);

            List<Document> documentList = new List<Document>();

			Document doc = new Document();
			
            doc.Add(new StringField("BookId", "B1", Field.Store.YES));
			
            doc.Add(new StringField("Category", "Cat 1", Field.Store.YES));
			
            documentList.Add(doc);
			
            doc = new Document();
			
            doc.Add(new StringField("BookId", "B2", Field.Store.YES));
			
            doc.Add(new StringField("Category", "Cat 1", Field.Store.YES));
			
            documentList.Add(doc);
			
            doc.Add(groupEndField);
			
            indexWriter.AddDocuments(documentList);

			documentList = new List<Document>();
			
            doc = new Document();
			
            doc.Add(new StringField("BookId", "B3", Field.Store.YES));
			
            doc.Add(new StringField("Category", "Cat 2", Field.Store.YES));
			
            documentList.Add(doc);
			
            doc.Add(groupEndField);
			
            indexWriter.AddDocuments(documentList);

            indexWriter.Commit();

			#endregion

			#region Lookup by group value

			Filter groupEndDocs = new CachingWrapperFilter(new QueryWrapperFilter(new TermQuery(new Term("groupEnd", "x"))));

			IndexReader indexReader = DirectoryReader.Open(directory);

			IndexSearcher indexSearcher = new IndexSearcher(indexReader);

			BlockGroupingCollector blockGroupingCollector = new BlockGroupingCollector(Sort.RELEVANCE, 10, true, groupEndDocs);

			indexSearcher.Search(new MatchAllDocsQuery(), null, blockGroupingCollector);

			var topGroups = blockGroupingCollector.GetTopGroups(Sort.RELEVANCE, 0, 0, 10, true);

			Console.WriteLine("Total group count: " + topGroups.TotalGroupCount);

			Console.WriteLine("Total group hit count: " + topGroups.TotalGroupedHitCount);

			foreach (var groupDocs in topGroups.Groups)
			{
				Console.WriteLine("Group: " + groupDocs.GroupValue);

				foreach (var scoreDoc in groupDocs.ScoreDocs)
				{
					doc = indexSearcher.Doc(scoreDoc.Doc);

					Console.WriteLine("Category: " + doc.GetField("Category").GetStringValue() + ", BookId: " + doc.GetField("BookId").GetStringValue());
				}
			}

            #endregion
        }
    }
}
