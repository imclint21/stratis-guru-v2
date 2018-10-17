using System.Collections.Generic;
using Stratis.Guru.Models;

namespace Stratis.Guru.Modules
{
    public class Ask : IAsk
    {
        public Queue<Vanity> VanityAsking = new Queue<Vanity>();
    }
}