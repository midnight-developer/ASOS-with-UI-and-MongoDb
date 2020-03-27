using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASOS.Api.Models
{
  public class BaseObjectIdEntity
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
  }
}