using System.Collections.Generic;
using System.Threading.Tasks;
using DolApi.Services;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace DolApi.Repositories
{
    public interface IPlayerRepo
    {
        Task Add(string userName);
    }

    public class PlayerRepo : IPlayerRepo
    {
        private const string Players = "players";
        private readonly FirestoreDb _db;
        public PlayerRepo(IConfiguration configuration)
        {
            _db = FirestoreDb.Create(configuration["PlayerId"]);
        }

        public async Task Add(string userName)
        {
            var docRef = _db.Collection(Players).Document(userName.ToLower());
            var user = new Dictionary<string, object>
            {
                { "UserName", userName }
            };
            await docRef.SetAsync(user);
        }
    }
}
