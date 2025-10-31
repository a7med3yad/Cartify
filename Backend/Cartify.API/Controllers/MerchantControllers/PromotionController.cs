using Cartify.Application.Contracts.PromotionDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/merchant/promotions")]
    [ApiController]
    [Authorize(Roles = "Merchant")]
    public class PromotionController : ControllerBase
    {
        private readonly IMerchantPromotionServices _promotionServices;

        public PromotionController(IMerchantPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }

        // =========================================================
        // 🔹 CREATE PROMOTION
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> AddPromotion([FromBody] CreatePromotionDto dto)
        {
            var result = await _promotionServices.AddPromotionAsync(dto);
            if (!result)
                return BadRequest(new { message = "Failed to create promotion ❌" });

            return Ok(new { message = "Promotion created successfully ✅" });
        }

        // =========================================================
        // 🔹 UPDATE PROMOTION
        // =========================================================
        [HttpPut("{promotionId:int}")]
        public async Task<IActionResult> UpdatePromotion(
            [FromRoute] int promotionId,
            [FromBody] UpdatePromotionDto dto)
        {
            var result = await _promotionServices.UpdatePromotionAsync(promotionId, dto);
            if (!result)
                return NotFound(new { message = "Failed to update promotion ❌" });

            return Ok(new { message = "Promotion updated successfully ✅" });
        }

        // =========================================================
        // 🔹 DELETE PROMOTION
        // =========================================================
        [HttpDelete("{promotionId:int}")]
        public async Task<IActionResult> DeletePromotion([FromRoute] int promotionId)
        {
            var result = await _promotionServices.DeletePromotionAsync(promotionId);
            if (!result)
                return NotFound(new { message = "Promotion not found ❌" });

            return Ok(new { message = "Promotion deleted successfully ✅" });
        }

        // =========================================================
        // 🔹 GET ALL PROMOTIONS
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await _promotionServices.GetAllPromotionsAsync();
            return Ok(promotions);
        }

        // =========================================================
        // 🔹 GET PROMOTION BY PRODUCT DETAIL ID
        // =========================================================
        [HttpGet("product-detail/{productDetailId:int}")]
        public async Task<IActionResult> GetPromotionByProductDetailId([FromRoute] int productDetailId)
        {
            var promotion = await _promotionServices.GetPromotionByProductDetailIdAsync(productDetailId);
            if (promotion == null)
                return NotFound(new { message = "No promotion found for this product detail ❌" });

            return Ok(promotion);
        }

        // =========================================================
        // 🔹 GET PROMOTIONS BY SUBCATEGORY ID
        // =========================================================
        [HttpGet("subcategory/{subCategoryId:int}")]
        public async Task<IActionResult> GetPromotionsBySubCategoryId([FromRoute] int subCategoryId)
        {
            var promotions = await _promotionServices.GetPromotionsBySubCategoryIdAsync(subCategoryId);
            if (!promotions.Any())
                return NotFound(new { message = "No promotions found for this subcategory ❌" });

            return Ok(promotions);
        }
    }
}
