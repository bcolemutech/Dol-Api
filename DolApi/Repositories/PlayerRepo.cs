using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace DolApi.Repositories
{
    using Wrappers;

    public interface IPlayerRepo
    {
        Task Add(string userId);
    }

    public class PlayerRepo : IPlayerRepo
    {
        private const string Players = "players";
        private readonly IFirestoreDb _db;

        public PlayerRepo(IConfiguration configuration, IFirestoreFactory firestoreFactory)
        {
            _db = firestoreFactory.Create(configuration["ProjectId"]);
        }

        public async Task Add(string userId)
        {
            var docRef = _db.Collection(Players).Document(userId);
            var user = new Dictionary<string, object>
            {
                { "UserId", userId }
            };
            var result = await docRef.SetAsync(user, SetOptions.MergeAll);
            
            Console.WriteLine($"New Player doc added at {result.UpdateTime}");
        }
    }
}
