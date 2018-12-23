using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stratis.Guru.Services
{
    public class LotterySetting
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int PublicKeyIterator { get; set; }
    }
}