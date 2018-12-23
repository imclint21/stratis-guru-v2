using System;

namespace Stratis.Guru.Models
{
    public class Participations : IParticipation
    {
        private DatabaseContext _databaseContext;

        public Participations(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void StoreParticipation(string ticket, string nickname, string address)
        {
            //TODO: store lottery id
            _databaseContext.Participations.InsertOne(new Participation
            {
                CreationDate = DateTime.Now,
                Ticket = ticket,
                Nickname = nickname,
                WithdrawAddress = address
            });
        }
    }
}