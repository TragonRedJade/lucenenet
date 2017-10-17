public class LuceneTest {
  public static void main(String[] args) throws IOException, ParseException {
    Analyzer analyzer = new StandardAnalyzer();

    Directory directory = new RAMDirectory();

    IndexWriterConfig config = new

    IndexWriterConfig(Version.LATEST, analyzer);

    IndexWriter indexWriter = new IndexWriter(directory, config);

    Document doc = new Document();

    String text = "Lucene is an Information Retrieval library written in Java";

    doc.add(new TextField("Content", text, Field.Store.YES));

    indexWriter.addDocument(doc);

    indexWriter.close();

    IndexReader indexReader = DirectoryReader.open(directory);

    IndexSearcher indexSearcher = new

    IndexSearcher(indexReader);

    QueryParser parser = new QueryParser( "Content", analyzer);

    Query query = parser.parse("Lucene");

    int hitsPerPage = 10;

    TopDocs docs = indexSearcher.search(query, hitsPerPage);

    ScoreDoc[] hits = docs.scoreDocs;

    int end = Math.min(docs.totalHits, hitsPerPage);

    System.out.print("Total Hits: " + docs.totalHits);

    System.out.print("Results: ");

    for (int i = 0; i < end; i++) {
      Document d = indexSearcher.doc(hits[i].doc);
      System.out.println("Content: " + d.get("Content");
    }

    public List<Document> getPage(int from , int size){

      List<Document> documents = new ArraList<Document>();

      Query query = parser.parse(searchTerm);

      TopDocs hits = searcher.search(query, maxNumberOfResults);

      int end = Math.min(hits.totalHits, size);

      for (int i = from; i < end; i++) {
        int docId = hits.scoreDocs[i].doc;
        //load the document
        Document doc = searcher.doc(docId);
        documents.add(doc);
      }

      return documents;
    }
  }
}
