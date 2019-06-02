using System.Collections.Async;
using System.Threading.Tasks;

namespace LoliSetManager
{
    public abstract class ItemSetRipper
    {
        protected abstract Task Runner(AsyncEnumerator<ItemSetInstall>.Yield yield);
        public AsyncEnumerable<ItemSetInstall> Run()
        {
            return new AsyncEnumerable<ItemSetInstall>(Runner);
        }

    }
}
