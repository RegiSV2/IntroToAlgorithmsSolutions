namespace VanEmdeBoasTree
{
    public interface IBoundedSet<TData>
    {
        void Insert(int key, TData data);
        bool Contains(int value);
        void Delete(int key);
        int? GetSuccessor(int key);
        int? GetPredecessor(int key);
        int Universe { get; }
        int? Min { get; }
        TData MinData { get; }
        int? Max { get; }
        TData MaxData { get; }
    }
}