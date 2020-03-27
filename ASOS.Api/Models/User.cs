using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASOS.Api.Models
{
  public class User : BaseObjectIdEntity
  {
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
  }
}
