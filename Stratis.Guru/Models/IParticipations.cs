using System.Collections.Generic;

namespace Stratis.Guru.Models
{
    public interface IParticipation
    {
        void StoreParticipation(string ticket, string nickname, string address, double amount);
        List<Participation> GetPlayers(string draw);
    }
}