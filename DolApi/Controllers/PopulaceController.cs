using DolApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    [Route("[controller]")]
    public class PopulaceController
    {
        private readonly IPopulaceRepo _populaceRepo;
        public PopulaceController(IPopulaceRepo repo)
        {
            _populaceRepo = repo;
        }

        [Authorize(Policy = "Players")]
        [HttpGet]
        [Route("{x}/{y}")]
        public OkObjectResult GetPopulaces(int x, int y)
        {
            throw new System.NotImplementedException();
        }
    }
}