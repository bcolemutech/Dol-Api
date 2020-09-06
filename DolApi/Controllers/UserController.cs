using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    public interface IUserController
    {
    }

    [Authorize]
    [Route("[controller]")]
    public class UserController : IUserController
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PlayerRequest playerRequest)
        {
            Console.WriteLine($"Player email = {playerRequest.Email}");
            UserRecord userRecord = null;
            bool newExists;
            try
            {
                userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync("email");
                newExists = false;
                Console.WriteLine($"Player ID is {userRecord.Uid}");
            }
            catch (Exception e)
            {
                if (e is FirebaseAuthException)
                {
                    Console.WriteLine($"Player does not exist");
                    newExists = true;
                }
                else
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            if (newExists)
            {
                var user = new UserRecordArgs
                {
                    Email = playerRequest.Email,
                    Password = Guid.NewGuid().ToString(),
                    Disabled = false
                };
                userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(user);
                Console.WriteLine($"New player ID is {userRecord.Uid}");
            }
            Console.WriteLine($"Authority set to {playerRequest.Authority}");
            var claims = new Dictionary<string, object>
            {
                { "Authority", playerRequest.Authority },
            };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);

            return new CreatedResult("", userRecord);
        }
        
        public class PlayerRequest
        {
            public string Email { get; set; }
            public string Authority { get; set; }
        }
    }
}
