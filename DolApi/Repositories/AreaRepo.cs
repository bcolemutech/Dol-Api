namespace DolApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Google.Cloud.Firestore;
    using Microsoft.Extensions.Configuration;
    using POCOs;

    public interface IAreaRepo
    {
        Task<Area> Retrieve(int x, int y);
        Task Replace(int x, int y, Area area);
        Task<IEnumerable<Area>> RetrieveAll();
    }

    public class AreaRepo : IAreaRepo
    {
        private readonly FirestoreDb _db;

        public AreaRepo(IConfiguration configuration)
        {
            _db = FirestoreDb.Create(configuration["ProjectId"]);
        }

        public async Task<Area> Retrieve(int x, int y)
        {
            var docRef = _db.Collection("area").Document($"{x}-{y}");

            var snapshot = await docRef.GetSnapshotAsync();

            return snapshot.Exists ? snapshot.ConvertTo<Area>() : null;
        }

        public async Task Replace(int x, int y, Area area)
        {
            Console.WriteLine($"Add/replace area {x}-{y}");
            var docRef = _db.Collection("area").Document($"{x}-{y}");
            await docRef.SetAsync(area, SetOptions.MergeAll);
        }

        public async Task<IEnumerable<Area>> RetrieveAll()
        {
            var query = _db.Collection("area");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(document => document.ConvertTo<Area>());
        }
    }
}
