﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace DolApi.Repositories
{
    public interface IPlayerRepo
    {
        Task Add(string userId);
    }

    public class PlayerRepo : IPlayerRepo
    {
        private const string Players = "players";
        private readonly FirestoreDb _db;

        public PlayerRepo(IConfiguration configuration)
        {
            _db = FirestoreDb.Create(configuration["ProjectId"]);
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
