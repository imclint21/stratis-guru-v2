

using System.Threading.Tasks;

namespace Stratis.Guru.Models
{
    public interface IDraws
    {
        Task InitDrawAsync(long nextDrawTimestamp); 
        string GetLastDraw();
    }
}