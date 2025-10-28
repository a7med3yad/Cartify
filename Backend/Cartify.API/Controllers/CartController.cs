using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet("GetCart/{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] object item)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPut("UpdateItemQuantity")]
        public async Task<IActionResult> UpdateItemQuantity([FromBody] object update)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpDelete("RemoveItem/{itemId}")]
        public async Task<IActionResult> RemoveItem(Guid itemId)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("Checkout/{userId}")]
        public async Task<IActionResult> Checkout(Guid userId)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
