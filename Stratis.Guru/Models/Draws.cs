using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Stratis.Guru;
using Stratis.Guru.Services;

namespace Stratis.Guru.Models
{
    public class Draws : IDraws
    {
        private DatabaseContext _databaseContext;

        public Draws(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task InitDrawAsync(long nextDrawTimestamp)
        {
            if(!_databaseContext.Draws.Find(x => x.DrawDate.Equals(nextDrawTimestamp)).Any())
            {
                await _databaseContext.Draws.InsertOneAsync(new Draw()
                {
                    DrawDate = nextDrawTimestamp,
                    Passed = false
                });
            }
        }
    }
}