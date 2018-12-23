using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stratis.Guru.Models
{
    public class Setting
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public uint PublicKeyIterator { get; set; }
    }
}