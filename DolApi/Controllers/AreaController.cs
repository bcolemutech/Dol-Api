namespace DolApi.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using POCOs;
    using Repositories;

    [Authorize(Policy = "Players")]
    [Route("[controller]")]
    public class AreaController
    {
        public AreaController(IAreaRepo areaRepo)
        {
            throw new System.NotImplementedException();
        }

        [HttpGet]
        [Route("{x}/{y}")]
        public Task<IActionResult> Get(in int x, in int y)
        {
            throw new System.NotImplementedException();
        }
    }
}