using System.Collections.Generic;
using Stratis.Guru.Models;

namespace Stratis.Guru.Modules
{
    public class Ask : IAsk
    {
        private readonly Queue<Vanity> _vanities = new Queue<Vanity>();
        
        public void NewVanity(Vanity vanity)
        {
            _vanities.Enqueue(vanity);
        }

        public Queue<Vanity> GetVanities()
        {
            return _vanities;
        }
    }
}