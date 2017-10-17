StandardAnalyzer analyzer = new StandardAnalyzer();

Directory directory = new RAMDirectory();

IndexWriterConfig config = new IndexWriterConfig(Version.LATEST,
analyzer);

IndexWriter indexWriter = new IndexWriter(directory, config);

FieldType groupEndFieldType = new FieldType();

groupEndFieldType.setStored(false);

groupEndFieldType.setTokenized(false);

groupEndFieldType.setIndexed(true);

groupEndFieldType.setIndexOptions(FieldInfo.IndexOptions.DOCS_ONLY);

groupEndFieldType.setOmitNorms(true);

Field groupEndField = new Field("groupEnd", "x", groupEndFieldType);

List<Document> documentList = new ArrayList();

Document doc = new Document();

doc.add(new StringField("BookId", "B1", Field.Store.YES));

doc.add(new StringField("Category", "Cat 1", Field.Store.YES));

documentList.add(doc);

doc = new Document();

doc.add(new StringField("BookId", "B2", Field.Store.YES));

doc.add(new StringField("Category", "Cat 1", Field.Store.YES));

documentList.add(doc);

doc.add(groupEndField);

indexWriter.addDocuments(documentList);

documentList = new ArrayList();

doc = new Document();

doc.add(new StringField("BookId", "B3", Field.Store.YES));

doc.add(new StringField("Category", "Cat 2", Field.Store.YES));

documentList.add(doc);

doc.add(groupEndField);

indexWriter.addDocuments(documentList);

indexWriter.commit();
