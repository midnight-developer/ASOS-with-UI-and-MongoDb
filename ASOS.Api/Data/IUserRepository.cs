using System.Collections.Generic;
using System.Threading.Tasks;
using ASOS.Api.Models;

namespace ASOS.Api.Data
{
  public interface IUserRepository
  {
    Task<User> Get(string id);
    Task<IEnumerable<User>> GetAll();
    Task Create(User user, string password);
    Task<long> Delete(string id);
    Task<bool> UserExists(string username);
    Task<User> AuthenticateUser(string username, string password);
  }
}