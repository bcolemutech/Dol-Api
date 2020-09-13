using DolApi.POCOs;
using DolApi.Repositories;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    [Authorize(Policy = "Players")]
    [Route("[controller]")]
    public class CharacterController
    {
        private readonly ICharacterRepo _characterRepo;
        private readonly string _username;

        public CharacterController(IHttpContextAccessor httpContextAccessor, ICharacterRepo characterRepo)
        {
            _characterRepo = characterRepo;
            _username = httpContextAccessor.HttpContext.User.Identity.Name;
        }

        [HttpPut]
        public IActionResult Put(string name)
        {
            var character = _characterRepo.Add(_username, name);

            return new CreatedResult($"/Character/{character.Name}", character);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var characters = _characterRepo.RetrieveAll(_username);
            
            return new OkObjectResult(characters);
        }

        [HttpGet]
        public IActionResult Get(string name)
        {
            var character = _characterRepo.Retrieve(_username, name);
            
            return new OkObjectResult(character);
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            _characterRepo.Remove(_username, name);
            
            return new NoContentResult();
        }
    }
}
