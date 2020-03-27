using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ASOS.Api.Models;
using MongoDB.Driver;

namespace ASOS.Api.Data
{
  public class UserRepository: IUserRepository
  {
    protected readonly IMongoCollection<User> Collection;

    public UserRepository(DataContext context)
    {
      Collection = context.Users;
    }

    public async Task<User> Get(string id)
    {
      return await Collection.Find(x => x.Id == id).FirstAsync();
    }

    public async Task<IEnumerable<User>> GetAll()
    {
      return await Collection.Find(_=>true).ToListAsync();
    }

    public async Task Create(User user, string password)
    {
      HashPassword(password, out var passwordHash, out var passwordSalt);

      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      await Collection.InsertOneAsync(user);
    }

    public async Task<long> Delete(string id)
    {
      var result = await Collection.DeleteOneAsync(x => x.Id == id);
      return result.DeletedCount;
    }

    public async Task<bool> UserExists(string username)
    {
      var userExists = await Collection.Find(x=>x.Username == username).FirstOrDefaultAsync() != null;
      return userExists;
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
      var user = await Collection.Find(x => x.Username == username).FirstAsync();
      if (user == null)
        return null;

      return !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
    }

    private void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
      }
    }
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using (var hmac = new HMACSHA512(passwordSalt))
      {
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        if (computedHash.Where((t, i) => t != passwordHash[i]).Any())
          return false;
      }
      return true;
    }
  }
}
