using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Stratis.Guru.Models;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;

namespace Stratis.Guru
{
    public class DatabaseContext
    {
        private readonly IMongoDatabase _database;

        public DatabaseContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("DefaultConnection"));
            if (client != null)
            {
                _database = client.GetDatabase("stratis-guru");
            }
        }

        public IMongoCollection<Draw> Draws => _database.GetCollection<Draw>("draws");
        public IMongoCollection<Setting> Settings => _database.GetCollection<Setting>("lottery");
        public IMongoCollection<Participation> Participations => _database.GetCollection<Participation>("participations");
    }
}