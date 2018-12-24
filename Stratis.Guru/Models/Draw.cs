using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stratis.Guru.Models
{
    public class Draw
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public long DrawDate { get; set; }
        public bool Passed { get; set; }
        public uint BeginIterator { get; set; }
    }
}