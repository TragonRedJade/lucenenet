using System;
using System.Collections.Generic;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Search.Grouping;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;
using System.Text;

namespace Lucene_Examples.Lesson_9
{
    public class Grouping
    {
        public Grouping()
        {
            #region Init

            Directory directory = new RAMDirectory();

            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);

            IndexWriter indexWriter = new IndexWriter(directory, config);

            #endregion

            #region Add Docs to Index

            #region Setup Group End Field

            FieldType groupEndFieldType = new FieldType();

            groupEndFieldType.IsStored = false;

            groupEndFieldType.IsTokenized = false;

            groupEndFieldType.IsIndexed = true;

            groupEndFieldType.IndexOptions = IndexOptions.DOCS_ONLY;

            groupEndFieldType.OmitNorms = true;

            Field groupEndField = new Field("groupEnd", "x", groupEndFieldType);

            #endregion

            List<Document> documentList = new List<Document>();

            Document doc = new Document();

            doc.Add(new StringField("BookId", "B1", Field.Store.YES));

            doc.Add(new StringField("Category", "Cat 1", Field.Store.YES));

            doc.Add(new Int32Field("Repetition", 1, Field.Store.YES));

            documentList.Add(doc);

            doc = new Document();

            doc.Add(new StringField("BookId", "B2", Field.Store.YES));

            doc.Add(new StringField("Category", "Cat 1", Field.Store.YES));

			doc.Add(new Int32Field("Repetition", 1, Field.Store.YES));

			documentList.Add(doc);

            doc.Add(groupEndField);

            indexWriter.AddDocuments(documentList);

            documentList = new List<Document>();

            doc = new Document();

            doc.Add(new StringField("BookId", "B3", Field.Store.YES));

            doc.Add(new StringField("Category", "Cat 2", Field.Store.YES));

			doc.Add(new Int32Field("Repetition", 2, Field.Store.YES));

			documentList.Add(doc);

            doc.Add(groupEndField);

            indexWriter.AddDocuments(documentList);

            indexWriter.Dispose();

            #endregion

            //BasicFindRepByNumericRange(directory);

            //LookupGroupsByIntAlt(directory);

            TwoPassGroupingSearch(directory);

            directory.Dispose();
		}

		//Find Repetition using numeric range query
		private void BasicFindRepByNumericRange(Directory directory)
        {
			var indexReader = DirectoryReader.Open(directory);

			var indexSearcher = new IndexSearcher(indexReader);

            Query query = NumericRangeQuery.NewInt32Range("Repetition", 1, 2, true, false);

		    TopDocs topDocs = indexSearcher.Search(query, 10);

            Console.WriteLine("Total Hits: " + topDocs.TotalHits);

			Console.WriteLine("Results: ");

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                Document d = indexSearcher.Doc(scoreDoc.Doc);

		        Console.WriteLine("Book Id: " + d.Get("BookId"));
	        }

			indexReader.Dispose();
		}

		//Lookup by group string value
        private void LookupGroupsbyString(Directory directory)
        {
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
			        Document doc = indexSearcher.Doc(scoreDoc.Doc);

			        Console.WriteLine("Category: " + doc.GetField("Category").GetStringValue() + ", BookId: " + doc.GetField("BookId").GetStringValue());
			    }
			}

			indexReader.Dispose();
		}

		//Lookup by group string value (Alternative Syntax)
		private void LookupGroupsByStringAlt(Directory directory)
        {
			Filter groupEndDocs = new CachingWrapperFilter(new QueryWrapperFilter(new TermQuery(new Term("groupEnd", "x"))));

			IndexReader indexReader = DirectoryReader.Open(directory);

			IndexSearcher indexSearcher = new IndexSearcher(indexReader);

			GroupingSearch groupingSearch = new GroupingSearch(groupEndDocs);

			groupingSearch.SetGroupSort(new Sort());

			groupingSearch.SetIncludeScores(true);

			TermQuery query = new TermQuery(new Term("Category", "Cat 1"));

			var groupsResult = groupingSearch.Search(indexSearcher, query, 0, 10); //search(indexSearcher, query, groupOffset, groupLimit);

			indexReader.Dispose();
        }

		//Lookup by group int value (Alternative Syntax)
		private void LookupGroupsByIntAlt(Directory directory)
		{
			Filter groupEndDocs = new CachingWrapperFilter(new QueryWrapperFilter(new TermQuery(new Term("groupEnd", "x"))));

			IndexReader indexReader = DirectoryReader.Open(directory);

			IndexSearcher indexSearcher = new IndexSearcher(indexReader);

			GroupingSearch groupingSearch = new GroupingSearch(groupEndDocs);

			groupingSearch.SetGroupSort(new Sort());

			groupingSearch.SetIncludeScores(true);

			Query query = NumericRangeQuery.NewInt32Range("Repetition", 1, 2, true, false);

			var groupsResult = groupingSearch.Search(indexSearcher, query, 0, 10); //search(indexSearcher, query, groupOffset, groupLimit);

			indexReader.Dispose();
		}

        //Two-pass grouping search with cacheing (kinda working)
        private void TwoPassGroupingSearch(Directory directory)
        {
            var indexReader = DirectoryReader.Open(directory);

            var indexSearcher = new IndexSearcher(indexReader);

            //GroupingSearch groupingSearch = new GroupingSearch("Repetition");

			GroupingSearch groupingSearch = new GroupingSearch("Category");

            groupingSearch.SetGroupSort(new Sort());

            groupingSearch.SetFillSortFields(true);

			groupingSearch.SetCachingInMB(40.0, true);

			TermQuery query = new TermQuery(new Term("Category", "Cat 1"));

			//Query query = NumericRangeQuery.NewInt32Range("Repetition", 1, 2, true, false);

            var results = groupingSearch.Search(indexSearcher, query, 0, 10);



            //indexReader.Dispose();

            //int? total = results.TotalGroupCount;

            foreach (var groupDocs in results.Groups)
            {
                Console.WriteLine("Group: " + groupDocs.GroupValue);

                var biff = groupDocs.GroupValue;

                var biff_type = biff.GetType();

                var groupStringValue = Encoding.ASCII.GetChars(new byte[]{43, 61, 74, 20, 31});

                foreach (var scoreDoc in groupDocs.ScoreDocs)
                {
                    Document doc = indexSearcher.Doc(scoreDoc.Doc);

                    Console.WriteLine("Category: " 
                                      + doc.GetField("Category").GetStringValue() 
                                      + ", BookId: " 
                                      + doc.GetField("BookId").GetStringValue());
                }
            }

            indexReader.Dispose();
		}
	}
}
