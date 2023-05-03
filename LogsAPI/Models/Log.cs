using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Dynamic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace LogsAPI.Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
        public string? RenderedMessage { get; set; }
        public string? SourceContext { get; set; }
        public string? ActionId { get; set; }
        public string? ActionName { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPath { get; set; }
        public string? ConnectionId { get; set; }
        public string? UtcTimestamp { get; set; }
        [BsonExtraElements]
        public IDictionary<string, object>? Properties { get; set; }
    }
}
   
