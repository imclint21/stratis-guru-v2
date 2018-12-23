using System;

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

        public void StoreParticipation(string ticket, string nickname, string address)
        {
            _databaseContext.Participations.InsertOne(new Participation
            {
                CreationDate = DateTime.Now,
                Ticket = ticket,
                Nickname = nickname,
                WithdrawAddress = address,
                Draw = _draws.GetLastDraw()
            });
        }
    }
}