namespace BTree
{
    internal struct SearchResult<TData>
    {
        private SearchResult(bool isFound, TData data)
        {
            IsFound = isFound;
            Data = data;
        }

        public bool IsFound { get; }
        public TData Data { get; }

        public static SearchResult<TData> CreateFound(TData data)
        {
            return new SearchResult<TData>(true, data);
        }

        public static SearchResult<TData> CreateNotFound()
        {
            return new SearchResult<TData>(false, default(TData));
        }
    }
}