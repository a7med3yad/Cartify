using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpPageController : ControllerBase
    {
        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpGet("GetTopic/{id}")]
        public async Task<IActionResult> GetTopic(Guid id)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] object topic)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] object topic)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
