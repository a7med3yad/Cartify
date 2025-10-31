using System.Threading.Tasks;
using Cartify.Application.Contracts.OrderDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/merchant/orders")]
    [ApiController]
    [Authorize(Roles = "Merchant")]
    public class OrderController : ControllerBase
    {
        private readonly IMerchantOrderServices _orderServices;

        public OrderController(IMerchantOrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        // =========================================================
        // 🔹 GET ORDER BY ID
        // =========================================================
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById([FromRoute] string orderId)
        {
            var order = await _orderServices.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found ❌" });

            return Ok(order);
        }

        // =========================================================
        // 🔹 GET ORDERS BY STORE ID (Paged)
        // =========================================================
        [HttpGet("store/{storeId:int}")]
        public async Task<IActionResult> GetOrdersByStoreId(
            [FromRoute] int storeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderServices.GetOrdersByStoreIdAsync(storeId, page, pageSize);
            return Ok(orders);
        }

        // =========================================================
        // 🔹 UPDATE ORDER STATUS
        // =========================================================
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] string orderId, [FromQuery] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return BadRequest(new { message = "Status is required ❗" });

            var result = await _orderServices.UpdateOrderStatusAsync(orderId, newStatus);

            if (!result)
                return BadRequest(new { message = "Failed to update order status ❌" });

            return Ok(new { message = $"Order status updated to '{newStatus}' ✅" });
        }

        // =========================================================
        // 🔹 FILTER ORDERS (Optional - To Implement)
        // =========================================================
        [HttpGet("filter")]
        public async Task<IActionResult> FilterOrders(
            [FromQuery] int storeId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // ⚠️ This is a placeholder until the logic is implemented in the service
            var result = await _orderServices.FilterOrdersAsync(storeId, startDate, endDate, status, page, pageSize);
            return Ok(result);
        }
    }
}
