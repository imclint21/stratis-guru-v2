using System.Threading.Tasks;
using MongoDB.Driver;

namespace Stratis.Guru.Models
{
    public class Settings : ISettings
    {
        private DatabaseContext _databaseContext;

        public Settings(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int GetIterator() => _databaseContext.Settings.Find(x => true).FirstOrDefault().PublicKeyIterator;


        public async Task InitAsync()
        {
            if(!_databaseContext.Settings.Find(x => true).Any())
            {
                await _databaseContext.Settings.InsertOneAsync(new Setting
                {
                    PublicKeyIterator = 0
                });
            }
        }
    }
}