using Cartify.Application.Contracts.CategoryDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Infrastructure.Implementation.Repository;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantCategoryServices : IMerchantCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MerchantCategoryServices(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public Task<bool> CreateCategoryAsync(CreateCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CategoryDto>> GetAllCategoriesAsync(int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetProductCountByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            throw new NotImplementedException();
        }
    }

}
