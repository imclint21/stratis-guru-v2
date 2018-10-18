using System.Collections.Generic;
using Stratis.Guru.Models;

namespace Stratis.Guru.Modules
{
    public interface IAsk
    {
        void NewVanity(Vanity vanity);
        Queue<Vanity> GetVanities();
    }
}