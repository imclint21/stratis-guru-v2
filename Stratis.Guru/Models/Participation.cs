using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stratis.Guru.Models
{
    public class Participation
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Ticket { get; set; }
        public string WithdrawAddress { get; set; }
        public BsonDateTime CreationDate { get; set; }
        public string Nickname { get; set; }
        public string Draw { get; set; }
        public double Amount { get; set; }
    }
}