using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ISettings _settings;

        public Draws(DatabaseContext databaseContext, ISettings settings)
        {
            _databaseContext = databaseContext;
            _settings = settings;
        }

        public string GetLastDraw()
        {
            return _databaseContext.Draws.Find(x => true).ToList().OrderByDescending(x => x.DrawDate).FirstOrDefault().Id.ToString();
        }

        public async Task InitDrawAsync(long nextDrawTimestamp)
        {
            if(!_databaseContext.Draws.Find(x => x.DrawDate.Equals(nextDrawTimestamp)).Any())
            {
                await _databaseContext.Draws.InsertOneAsync(new Draw()
                {
                    DrawDate = nextDrawTimestamp,
                    BeginIterator = _settings.GetIterator(),
                    Passed = false
                });
            }
        }
    }
}