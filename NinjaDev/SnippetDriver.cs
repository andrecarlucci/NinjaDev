namespace NinjaDev {
    public class SnippetDriver {
        private readonly string _snippet;
        private int _index;

        public SnippetDriver(string snippet) {
            _snippet = snippet;
        }

        public bool Available => _index < _snippet.Length;

        public char Next() {
            return _snippet[_index++];
        }
    }
}