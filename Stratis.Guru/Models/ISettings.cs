

using System.Threading.Tasks;

namespace Stratis.Guru.Models
{
    public interface ISettings
    {
        Task InitAsync();
        uint GetIterator();
        void IncrementIterator();
    }
}