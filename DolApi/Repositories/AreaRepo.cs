namespace DolApi.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Google.Cloud.Firestore;
    using Microsoft.Extensions.Configuration;
    using POCOs;

    public interface IAreaRepo
    {
        Task<Area> Retrieve(int x, int y);
        Task Replace(int x, int y, Area area);
        Task<Area[]> RetrieveAll();
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

        public Task<Area[]> RetrieveAll()
        {
            throw new NotImplementedException();
        }
    }
}
