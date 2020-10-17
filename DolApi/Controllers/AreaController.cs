namespace DolApi.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using POCOs;
    using Repositories;

    [Route("[controller]")]
    public class AreaController
    {
        private readonly IAreaRepo _areaRepo;
        public AreaController(IAreaRepo areaRepo)
        {
            _areaRepo = areaRepo;
        }

        [Authorize(Policy = "Players")]
        [HttpGet]
        [Route("{x}/{y}")]
        public async Task<IActionResult> Get(int x, int y)
        {
            var character = await _areaRepo.Retrieve(x, y);

            if (character == null)
            {
                return new NotFoundResult();
            }
            
            return new OkObjectResult(character);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("{x}/{y}")]
        public async Task<IActionResult> Put(int x, int y,[FromBody] Area area)
        {
            area.X = x;
            area.Y = y;
            
            await _areaRepo.Replace(x, y, area);

            return new OkResult();
        }
    }
}