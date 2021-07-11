using System.Linq;
using System.Threading.Tasks;
using dol_sdk.POCOs;
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
        private readonly string _userId;

        public CharacterController(IHttpContextAccessor httpContextAccessor, ICharacterRepo characterRepo)
        {
            _characterRepo = characterRepo;
            var user = httpContextAccessor.HttpContext.User;
            _userId = user.Claims.First(c => c.Type == "user_id").Value;
        }

        [HttpPut]
        [Route("{name}")]
        public async Task<IActionResult> Put(string name)
        {
            var character = await _characterRepo.Add(_userId, name);

            return new CreatedResult($"/Character/{character.Name}", character);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var characters = await _characterRepo.RetrieveAll(_userId);
            
            return new OkObjectResult(characters);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var character = await _characterRepo.Retrieve(_userId, name);
            
            return new OkObjectResult(character);
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await _characterRepo.Remove(_userId, name);
            
            return new NoContentResult();
        }

        [HttpPut]
        [Route("{name}/move")]
        public async Task<IActionResult> PutMove(string name, [FromBody] IPosition move)
        {
            throw new System.NotImplementedException();
        }
    }
}
