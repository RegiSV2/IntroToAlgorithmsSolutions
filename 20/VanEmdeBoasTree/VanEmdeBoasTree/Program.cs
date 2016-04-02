using VanEmdeBoasTree.Tests;

namespace VanEmdeBoasTree
{
    class Program
    {
        static void Main(string[] args)
        {
            new RegularVanEmdeBoasTreeTests().RunAllTests();
            new RsVanEmdeBoasTreeTests().RunAllTests();
        }
    }
}
