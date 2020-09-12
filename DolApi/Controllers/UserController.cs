using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DolApi.POCOs;
using DolApi.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("[controller]")]
    public class UserController
    {
        private readonly IAdminService _admin;
        public UserController(IAdminService adminService)
        {
            _admin = adminService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PlayerRequest playerRequest)
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
    }
}
