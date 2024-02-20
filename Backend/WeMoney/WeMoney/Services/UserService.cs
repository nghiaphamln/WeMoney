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

    public async Task<User?> GetByEmailAsync(string email) =>
        await _userCollection
            .Find(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            .Limit(1)
            .SingleOrDefaultAsync();


    public async Task<User?> GetByIdAsync(string id) =>
        await _userCollection
            .Find(user => user.Email.Equals(id))
            .Limit(1)
            .SingleOrDefaultAsync();

    public async Task CreateAsync(User user) =>
        await _userCollection.InsertOneAsync(user);

    public async Task UpdateAsync(User user) =>
        await _userCollection.ReplaceOneAsync(e => e.Id!.Equals(user.Id), user);
}