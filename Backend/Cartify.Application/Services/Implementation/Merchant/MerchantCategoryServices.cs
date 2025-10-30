using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartify.Application.Contracts;
using Cartify.Application.Contracts.CategoryDtos;
using Cartify.Application.Contracts.ProductDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantCategoryServices : IMerchantCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public MerchantCategoryServices(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<bool> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return false;

            var existingCategory = await _unitOfWork.CategoryRepository
                .Search(c => c.CategoryName == dto.CategoryName && !c.IsDeleted);

            if (existingCategory != null)
                return false; 

            string? imageUrl = null;
            if (dto.Image != null)
            {
                imageUrl = await _fileStorageService.UploadFileAsync(dto.Image, "categories");
            }

            var category = new TblCategory
            {
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDescription,
                ImageUrl = imageUrl, 
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.CategoryRepository.CreateAsync(category);

            await _unitOfWork.SaveChanges();

            return true;
        }


        public async Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.ReadByIdAsync(categoryId);
            if (category == null || category.IsDeleted)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
                category.CategoryName = dto.CategoryName;

            if (!string.IsNullOrWhiteSpace(dto.CategoryDescription))
                category.CategoryDescription = dto.CategoryDescription;

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                category.ImageUrl = dto.ImageUrl;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.ReadByIdAsync(categoryId);
            if (category == null)
                return false;

            category.IsDeleted = true;
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<PagedResult<CategoryDto>> GetAllCategoriesAsync(int page = 1, int pageSize = 10)
        {
            var allCategories = await _unitOfWork.CategoryRepository.ReadAsync();

            var filtered = allCategories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.CategoryName)
                .AsQueryable();

            var total = filtered.Count();

            var paged = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    ImageUrl = c.ImageUrl
                })
                .ToList();

            return new PagedResult<CategoryDto>(paged, total, page, pageSize);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.ReadByIdAsync(categoryId);
            if (category == null || category.IsDeleted)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                ImageUrl = category.ImageUrl
            };
        }

        public async Task<int> GetProductCountByCategoryIdAsync(int categoryId)
        {
            var allProducts = await _unitOfWork.ProductRepository.ReadAsync();
            return allProducts.Count(p => p.Type.CategoryId == categoryId && !p.IsDeleted);
        }

        public async Task<PagedResult<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            var allProducts = await _unitOfWork.ProductRepository.GetAllIncluding(
                p => p.Type,
                p => p.Type.Category,
                p => p.TblProductImages
            );

            var filtered = allProducts
                .Where(p => p.Type.CategoryId == categoryId && !p.IsDeleted)
                .AsQueryable();

            var total = filtered.Count();

            var paged = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    TypeName = p.Type.TypeName,
                    CategoryName = p.Type.Category.CategoryName,
                    StoreId = p.UserStoreId,
                    ImageUrl = p.TblProductImages != null && p.TblProductImages.Any()
                        ? p.TblProductImages.First().ImageURL
                        : null
                })
                .ToList();

            return new PagedResult<ProductDto>(paged, total, page, pageSize);
        }

        public async Task<PagedResult<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId, int page = 1, int pageSize = 10)
        {
            var allProducts = await _unitOfWork.ProductRepository.GetAllIncluding(
                p => p.Type,
                p => p.Type.Category,
                p => p.TblProductImages
            );

            var filtered = allProducts
                .Where(p => p.TypeId == subCategoryId && !p.IsDeleted)
                .AsQueryable();

            var total = filtered.Count();

            var paged = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    TypeName = p.Type.TypeName,
                    CategoryName = p.Type.Category.CategoryName,
                    StoreId = p.UserStoreId,
                    ImageUrl = p.TblProductImages != null && p.TblProductImages.Any()
                        ? p.TblProductImages.First().ImageURL
                        : null
                })
                .ToList();

            return new PagedResult<ProductDto>(paged, total, page, pageSize);
        }
    }
}
