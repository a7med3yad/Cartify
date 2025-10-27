using Cartify.Application.Contracts;
using Cartify.Application.Contracts.ProductDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using Microsoft.AspNetCore.Http;
namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantProductServices : IMerchantProductServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MerchantProductServices(IUnitOfWork _unitOfWork)
        {
        this._unitOfWork = _unitOfWork;
        }

        public async Task<bool> AddProductAsync(CreateProductDto dto)
        {
            var _product = new TblProduct
            {
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                TypeId = dto.TypeId,
                UserStoreId = dto.StoreId,
            };
            if (_product == null)
            {
                return false;
            }
            await _unitOfWork.ProductRepository.CreateAsync(_product);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public Task<bool> AddProductImagesAsync(int productId, List<IFormFile> images)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProductDto>> GetAllProductsByMerchantIdAsync(string merchantId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProductDto>> GetProductsByNameAsync(string name, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ProductDto>> GetProductsByTypeIdAsync(int typeId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductAsync(int productId, CreateProductDto dto)
        {
            throw new NotImplementedException();
        }
    }
}