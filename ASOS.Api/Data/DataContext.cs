using System;
using ASOS.Api.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ASOS.Api.Data
{
  public class DataContext
  {
    private readonly IMongoDatabase _database;

    public DataContext(IMongoClient client, string databaseName)
    {
      _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("User");
  }
}