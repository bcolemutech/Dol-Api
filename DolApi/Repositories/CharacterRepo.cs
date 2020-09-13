using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DolApi.POCOs;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace DolApi.Repositories
{
    public interface ICharacterRepo
    {
        Task<Character> Add(string user, string name);
        Task<IEnumerable<Character>> RetrieveAll(string user);
        Task<Character> Retrieve(string user, string name);
        Task Remove(string user, string name);
    }

    [ExcludeFromCodeCoverage]
    public class CharacterRepo : ICharacterRepo
    {
        private const string Players = "players";
        private const string Characters = "characters";
        private readonly FirestoreDb _db;

        public CharacterRepo(IConfiguration configuration)
        {
            _db = FirestoreDb.Create(configuration["PlayerId"]);
        }

        public async Task<Character> Add(string user, string name)
        {
            Console.WriteLine($"Adding character {name} to player {user}");
            var docRef = _db.Collection(Players).Document(user).Collection(Characters).Document($"{name.ToLower()}:");
            var character = new Character {Name = name};
            await docRef.SetAsync(character, SetOptions.MergeAll);

            return await Retrieve(user, name);
        }

        public async Task<IEnumerable<Character>> RetrieveAll(string user)
        {
            var query = _db.Collection(Players).Document(user).Collection(Characters).OrderBy("Name");
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(document => document.ConvertTo<Character>());
        }

        public async Task<Character> Retrieve(string user,string name)
        {
            var docRef = _db.Collection(Players).Document(user).Collection(Characters).Document($"{name.ToLower()}:");

            var snapshot = await docRef.GetSnapshotAsync();

            return snapshot.Exists ? snapshot.ConvertTo<Character>() : null;
        }

        public async Task Remove(string user,string name)
        {
            var docRef = _db.Collection(Players).Document(user).Collection(Characters).Document($"{name.ToLower()}:");
            await docRef.DeleteAsync();
        }
    }
}
