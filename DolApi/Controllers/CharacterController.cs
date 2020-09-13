using System.Threading.Tasks;
using DolApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Put(string name)
        {
            var character = await _characterRepo.Add(_username, name);

            return new CreatedResult($"/Character/{character.Name}", character);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var characters = await _characterRepo.RetrieveAll(_username);
            
            return new OkObjectResult(characters);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var character = await _characterRepo.Retrieve(_username, name);
            
            return new OkObjectResult(character);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string name)
        {
            await _characterRepo.Remove(_username, name);
            
            return new NoContentResult();
        }
    }
}
