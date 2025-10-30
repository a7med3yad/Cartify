using Cartify.Application.Contracts;
using Cartify.Application.Contracts.CategoryDtos;
using Cartify.Infrastructure.Implementation.Repository;
using System.Threading.Tasks;

namespace Cartify.Application.Services.Interfaces.Merchant
{
    public interface IMerchantCategoryServices
    {
        Task<PagedResult<CategoryDto>> GetAllCategoriesAsync(int page = 1, int pageSize = 10);
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<bool> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<int> GetProductCountByCategoryIdAsync(int categoryId);
        Task<PagedResult<ProductDto>> GetProductsByCategoryIdAsync(int categoryIint ,int page = 1, int pageSize = 10);
        Task<PagedResult<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId, int page = 1, int pageSize = 10);
    }
}
