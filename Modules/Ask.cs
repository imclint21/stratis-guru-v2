using System.Collections.Generic;
using Stratis.Guru.Models;

namespace Stratis.Guru.Modules
{
    public class Ask : IAsk
    {
        private Queue<Vanity> VanityAsking = new Queue<Vanity>();
        
        public void NewVanity(Vanity vanity)
        {
            VanityAsking.Enqueue(vanity);
        }

        public Queue<Vanity> GetVanities()
        {
            return VanityAsking;
        }
    }
}