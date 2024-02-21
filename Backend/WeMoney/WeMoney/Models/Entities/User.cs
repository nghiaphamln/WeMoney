using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeMoney.Models.Entities;

[Serializable]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Role { get; set; } = [];
    public string Password { get; set; } = null!;
    public string Avatar { get; set; } = "/images/no-avatar.jpeg";
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}