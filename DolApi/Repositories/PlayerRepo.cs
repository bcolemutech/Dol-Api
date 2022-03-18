namespace DolApi.Repositories;

using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using dol_sdk.POCOs;

public interface IPlayerRepo
{
    Task Add(string userId, PlayerRequest playerRequest);
}

public class PlayerRepo : IPlayerRepo
{
    private const string Players = "players";
    private readonly FirestoreDb _db;

    public PlayerRepo(IConfiguration configuration)
    {
        _db = FirestoreDb.Create(configuration["ProjectId"]);
    }

    public async Task Add(string userId, PlayerRequest playerRequest)
    {
        var docRef = _db.Collection(Players).Document(userId);
        var snapshot = await docRef.GetSnapshotAsync();

        User user;
        if (snapshot.Exists)
        {
            user = snapshot.ConvertTo<User>();
            user.Authority = playerRequest.Authority;
        }
        else
        {
            user = new User
            {
                Authority = playerRequest.Authority,
                Email = playerRequest.Email,
                UserId = userId
            };
        }

        var result = await docRef.SetAsync(user, SetOptions.MergeAll);

        Console.WriteLine($"New Player doc added at {result.UpdateTime}");
    }
}
