using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DolApi.Controllers
{
    [Authorize(Policy = "Players")]
    [Route("[controller]")]
    public class CharacterController
    {
        [HttpPut]
        public IActionResult Put(string name)
        {
            throw new System.NotImplementedException();
        }

        [HttpGet]
        public IActionResult Get()
        {
            throw new System.NotImplementedException();
        }

        [HttpGet]
        public IActionResult Get(string name)
        {
            throw new System.NotImplementedException();
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
