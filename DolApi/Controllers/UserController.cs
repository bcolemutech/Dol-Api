namespace DolApi.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DolApi.Repositories;
using DolApi.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dol_sdk.POCOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;


[Route("[controller]")]
public class UserController
{
    private readonly IAdminService _admin;
    private readonly IPlayerRepo _playerRepo;
    private readonly string _userId;

    public UserController(IHttpContextAccessor httpContextAccessor,IAdminService adminService, IPlayerRepo playerRepo)
    {
        _admin = adminService;
        _playerRepo = playerRepo;
        
        var user = httpContextAccessor.HttpContext?.User;
        _userId = user?.Claims.First(c => c.Type == "user_id").Value;
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PlayerRequest playerRequest)
    {
        Console.WriteLine($"Player email = {playerRequest.Email}");
        var userId = "";
        bool newExists;
        try
        {
            userId = _admin.GetUserByEmailAsync(playerRequest.Email);
            newExists = false;
            Console.WriteLine("Player found");
        }
        catch (Exception)
        {
            newExists = true;
        }

        if (newExists)
        {
            Console.WriteLine("Creating new player");
            var user = new UserRecordArgs
            {
                Email = playerRequest.Email,
                Password = Guid.NewGuid().ToString(),
                Disabled = false
            };
            userId = _admin.CreateUserAsync(user);
            await _playerRepo.Add(userId, playerRequest);
            Console.WriteLine("New player created");
        }

        Console.WriteLine($"Setting authority to {playerRequest.Authority}");
        var claims = new Dictionary<string, object>
        {
            { "Authority", playerRequest.Authority },
        };
        await _admin.SetCustomUserClaimsAsync(userId, claims);

        return new OkResult();
    }

    [Authorize(Policy = "Player")]
    [HttpPatch]
    public async Task<IActionResult> Patch(JsonPatchDocument<User> doc)
    {
        var user = await _playerRepo.Get(_userId);

        if (user is null)
        {
            return new NotFoundResult();
        }

        var resetSession = doc.Operations.Any(x => x.path == nameof(User.CurrentCharacter));

        doc.ApplyTo(user);

        user.SessionId = resetSession ? string.Empty : user.SessionId;
        
        await _playerRepo.Update(_userId, user);

        return new OkResult();
    }
}
