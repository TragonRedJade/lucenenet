StandardAnalyzer analyzer = new StandardAnalyzer();

Directory directory = new RAMDirectory();

IndexWriterConfig config = new IndexWriterConfig(Version.LATEST, analyzer);

IndexWriter indexWriter = new IndexWriter(directory, config);

Document doc = new Document();

doc.add(new StringField("BookId", "B1", Field.Store.YES));

doc.add(new StringField("Category", "Cat 1", Field.Store.YES));

indexWriter.addDocument(doc);

doc = new Document();

doc.add(new StringField("BookId", "B2", Field.Store.YES));

doc.add(new StringField("Category", "Cat 1", Field.Store.YES));

indexWriter.addDocument(doc);

doc = new Document();

doc.add(new StringField("BookId", "B3", Field.Store.YES));

doc.add(new StringField("Category", "Cat 2", Field.Store.YES));

indexWriter.addDocument(doc);

indexWriter.commit();

GroupingSearch groupingSearch = new GroupingSearch("Category");

groupingSearch.setAllGroups(true);

groupingSearch.setGroupDocsLimit(10);

IndexReader indexReader = DirectoryReader.open(directory);

IndexSearcher indexSearcher = new IndexSearcher(indexReader);

TopGroups topGroups = groupingSearch.search(indexSearcher, new MatchAllDocsQuery(), 0, 10);

System.out.println("Total group count: " + topGroups.totalGroupCount);

System.out.println("Total group hit count: " + topGroups.totalGroupedHitCount);

for (GroupDocs groupDocs : topGroups.groups)
{
  System.out.println("Group: " + ((BytesRef)groupDocs.groupValue).utf8ToString());

  for (ScoreDoc scoreDoc : groupDocs.scoreDocs)
  {
    doc = indexSearcher.doc(scoreDoc.doc);

    System.out.println("Category: " +

    doc.getField("Category").stringValue() + ", BookId: " +

    doc.getField("BookId").stringValue());
  }
}
