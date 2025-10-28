using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        [HttpGet("GetWishlist/{userId}")]
        public async Task<IActionResult> GetWishlist(Guid userId)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] object item)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpDelete("Remove/{itemId}")]
        public async Task<IActionResult> Remove(Guid itemId)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpDelete("Clear/{userId}")]
        public async Task<IActionResult> Clear(Guid userId)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
