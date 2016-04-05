using System.Collections.Generic;

namespace VanEmdeBoasTree
{
    public interface IVanEmdeBoasTree<TData> : IBoundedSet<TData>
    {
        IVanEmdeBoasTree<TData> Summary { get; }
        IEnumerable<IVanEmdeBoasTree<TData>> Clusters { get; }
    }
}