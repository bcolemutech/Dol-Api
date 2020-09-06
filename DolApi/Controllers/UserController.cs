using System;
using System.Collections.Generic;
using DolApi.POCOs;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    public interface IUserController
    {
    }

    //[Authorize(Policy = "Admins")]
    [Route("[controller]")]
    public class UserController : IUserController
    {
        [HttpPost]
        public async void Post([FromBody]PlayerRequest playerRequest)
        {
            UserRecord userRecord = null;
            bool newExists;
            try
            {
                userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync("email");
                newExists = false;
            }
            catch (Exception e)
            {
                if (e is FirebaseAuthException)
                {
                    newExists = true;
                }
                else
                {
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
            }
            
            var claims = new Dictionary<string, object>
            {
                { "Authority", playerRequest.Authority.ToString() },
            };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
        }
        
        public struct PlayerRequest
        {
            public string Email { get; set; }
            public Authority Authority { get; set; }
        }
    }
}
