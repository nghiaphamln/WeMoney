using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WeMoney.Models.Constants;
using WeMoney.Models.Entities;

namespace WeMoney.Services;

public class UserService
{
    private readonly IMongoCollection<User> _userCollection;

    public UserService(IOptions<AppSettings> options) 
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _userCollection = mongoDatabase.GetCollection<User>(options.Value.UserCollectionName);
    }

    public async Task<User?> GetAsync(string email) =>
        await _userCollection
            .Find(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            .Limit(1)
            .SingleAsync();
}