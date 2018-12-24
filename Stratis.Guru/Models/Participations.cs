using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Stratis.Guru.Models
{
    public class Participations : IParticipation
    {
        private DatabaseContext _databaseContext;
        private IDraws _draws;

        public Participations(DatabaseContext databaseContext, IDraws draws)
        {
            _databaseContext = databaseContext;
            _draws = draws;
        }

        public List<Participation> GetPlayers(string draw) => _databaseContext.Participations.Find(x => x.Draw.Equals(draw)).ToList().OrderByDescending(x => x.CreationDate).ToList();

        public void StoreParticipation(string ticket, string nickname, string address, double amount)
        {
            _databaseContext.Participations.InsertOne(new Participation
            {
                CreationDate = DateTime.Now,
                Ticket = ticket,
                Nickname = nickname,
                WithdrawAddress = address,
                Draw = _draws.GetLastDraw(),
                Amount = amount
            });
        }
    }
}