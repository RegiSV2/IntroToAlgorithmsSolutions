using System.Collections.Generic;

namespace VanEmdeBoasTree
{
    public interface IVanEmdeBoasTree<TData>
    {
        uint Universe { get; }
        uint? Min { get; }
        TData MinData { get; }
        uint? Max { get; }
        TData MaxData { get; }
        IVanEmdeBoasTree<TData> Summary { get; }
        IEnumerable<IVanEmdeBoasTree<TData>> Clusters { get; }
        void Insert(uint value, TData data);
        bool Contains(uint value);
        uint? GetSuccessor(uint value);
        uint? GetPredecessor(uint value);
        void Delete(uint value);
    }
}