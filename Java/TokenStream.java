Reader reader = new StringReader("Text to be passed");

Analyzer analyzer = new SimpleAnalyzer();

TokenStream tokenStream = analyzer.tokenStream("myField", reader);


StringReader reader = new StringReader("Lucene is mainly used for
information retrieval and you can read more about it at lucene.apache.
org.");
StandardAnalyzer wa = new StandardAnalyzer();
TokenStream ts = null;
try {
  ts = wa.tokenStream("field", reader);
  OffsetAttribute offsetAtt = ts.addAttribute(OffsetAttribute.
  class);
  CharTermAttribute termAtt = ts.addAttribute(CharTermAttribute.
  class);
  ts.reset();
  while (ts.incrementToken()) {
    String token = termAtt.toString();
    System.out.println("[" + token + "]");
    System.out.println("Token starting offset: " + offsetAtt.
    startOffset());
    System.out.println(" Token ending offset: " + offsetAtt.
    endOffset());
    System.out.println("");
  }
  ts.end();
} catch (IOException e) {
  e.printStackTrace();
} finally {
  ts.close();
  wa.close();
}