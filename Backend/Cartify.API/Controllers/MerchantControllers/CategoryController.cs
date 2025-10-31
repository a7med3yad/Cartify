using Cartify.Application.Contracts.CategoryDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cartify.API.Controllers.MerchantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMerchantCategoryServices _categoryServices;
        public CategoryController(IMerchantCategoryServices _categoryServices)
        {
            this._categoryServices = _categoryServices;
        }


        // =========================================================
        // 🔹 CREATE CATEGORY
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryServices.CreateCategoryAsync(dto);
            if (!result)
                return BadRequest("Failed to create category (already exists or invalid data).");

            return Ok(new { message = "Category created successfully ✅" });
        }

        // =========================================================
        // 🔹 UPDATE CATEGORY
        // =========================================================
        [HttpPut("{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int categoryId, [FromForm] CreateCategoryDto dto)
        {
            var result = await _categoryServices.UpdateCategoryAsync(categoryId, dto);
            if (!result)
                return NotFound(new { message = "Category not found or update failed ❌" });

            return Ok(new { message = "Category updated successfully ✅" });
        }

        // =========================================================
        // 🔹 DELETE CATEGORY
        // =========================================================
        [HttpDelete("{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
        {
            var result = await _categoryServices.DeleteCategoryAsync(categoryId);
            if (!result)
                return NotFound(new { message = "Category not found or already deleted ❌" });

            return Ok(new { message = "Category deleted successfully ✅" });
        }

        // =========================================================
        // 🔹 GET ALL CATEGORIES (Paged)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoryServices.GetAllCategoriesAsync(page, pageSize);
            return Ok(result);
        }

        // =========================================================
        // 🔹 GET CATEGORY BY ID
        // =========================================================
        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
        {
            var category = await _categoryServices.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return NotFound(new { message = "Category not found ❌" });

            return Ok(category);
        }

        // =========================================================
        // 🔹 GET PRODUCT COUNT BY CATEGORY
        // =========================================================
        [HttpGet("{categoryId:int}/products/count")]
        public async Task<IActionResult> GetProductCountByCategoryId([FromRoute] int categoryId)
        {
            var count = await _categoryServices.GetProductCountByCategoryIdAsync(categoryId);
            return Ok(new { categoryId, productCount = count });
        }

        // =========================================================
        // 🔹 GET PRODUCTS BY CATEGORY
        // =========================================================
        [HttpGet("{categoryId:int}/products")]
        public async Task<IActionResult> GetProductsByCategory([FromRoute] int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _categoryServices.GetProductsByCategoryIdAsync(categoryId, page, pageSize);
            return Ok(products);
        }

        // =========================================================
        // 🔹 GET PRODUCTS BY SUBCATEGORY
        // =========================================================
        [HttpGet("subcategory/{subCategoryId:int}/products")]
        public async Task<IActionResult> GetProductsBySubCategory([FromRoute] int subCategoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _categoryServices.GetProductsBySubCategoryIdAsync(subCategoryId, page, pageSize);
            return Ok(products);
        }

    }
}
