using System;
using System.Linq;
using System.Threading.Tasks;
using dol_sdk.Enums;
using dol_sdk.POCOs;
using DolApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Action = dol_sdk.Enums.Action;

namespace DolApi.Controllers
{
    [Authorize(Policy = "Players")]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterRepo _characterRepo;
        private readonly IAreaRepo _areaRepo;
        private readonly string _userId;
        private readonly DolApi.POCOs.Position _startingPosition;

        private readonly Action[] allowedPositionActions = {Action.Idle, Action.Rest};

        public CharacterController(IHttpContextAccessor httpContextAccessor, ICharacterRepo characterRepo,
            IAreaRepo areaRepo, IConfiguration configuration)
        {
            _characterRepo = characterRepo;
            _areaRepo = areaRepo;
            _startingPosition = new POCOs.Position
            {
                X = Convert.ToInt32(configuration["StartPosition:X"]),
                Y = Convert.ToInt32(configuration["StartPosition:Y"]),
                Populace = configuration["StartPosition:Populace"]
            };
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.Claims.First(c => c.Type == "user_id").Value;
        }

        [HttpPut]
        [Route("{name}")]
        public async Task<IActionResult> Put(string name)
        {
            var character = await _characterRepo.Add(_userId, name, _startingPosition);

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
        [Route("{name}/position")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutPosition(string name, Position position)
        {
            var (validPosition, actionResult) = await TryValidatePosition(position, false);
            if (!validPosition) return actionResult;
            
            await _characterRepo.SetPosition(_userId, name, position);

            return Ok();
        }
        
        private async Task<Tuple<bool, IActionResult>> TryValidatePosition(IPosition position, bool moving)
        {
            const string objectInvalid = "Position object is not valid.";

            if (!moving && !allowedPositionActions.Contains(position.Action))
                return new Tuple<bool, IActionResult>(false,
                    UnprocessableEntity($"{objectInvalid} The {position.Action} action is not allowed for current position"));
            
            var area = await _areaRepo.Retrieve(position.X, position.Y);

            if (area is null)
                return new Tuple<bool, IActionResult>(false,
                    UnprocessableEntity($"{objectInvalid} Area {position.X},{position.Y} does not exist"));

            if (area.Navigation == Navigation.Impassable)
                return new Tuple<bool, IActionResult>(false,
                    UnprocessableEntity($"{objectInvalid} Area {position.X},{position.Y} is impassable"));

            return new Tuple<bool, IActionResult>(true, null);
        }
    }
}
