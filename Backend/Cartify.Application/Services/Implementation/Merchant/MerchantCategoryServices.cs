using Cartify.Application.Contracts;
using Cartify.Application.Contracts.CategoryDtos;
using Cartify.Application.Contracts.ProductDtos;
using Cartify.Application.Services.Implementation.Helper;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantCategoryServices : IMerchantCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly GetUserServices _getUserServices;

        public MerchantCategoryServices(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            GetUserServices getUserServices)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _getUserServices = getUserServices;
        }

        // =========================================================
        // 🔹 CREATE CATEGORY
        // =========================================================
        public async Task<bool> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return false;

            var existingCategory = await _unitOfWork.CategoryRepository
                .Search(c => c.CategoryName == dto.CategoryName && !c.IsDeleted);

            if (existingCategory != null)
                return false;

            // 🧩 Extract merchant info from Token
            var merchantId = _getUserServices.GetMerchantIdFromToken();
            var merchantName = _getUserServices.GetUserNameFromToken() ?? "merchant";

            // Upload image if provided
            string? imageUrl = null;
            if (dto.Image != null)
                imageUrl = await _fileStorageService.UploadFileAsync(dto.Image, $"categories/{merchantName}");

            var category = new TblCategory
            {
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDescription,
                ImageUrl = imageUrl,
                CreatedBy = int.TryParse(merchantId, out var id) ? id : 0,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.CategoryRepository.CreateAsync(category);
            return await _unitOfWork.SaveChanges() > 0;
        }

        // =========================================================
        // 🔹 UPDATE CATEGORY
        // =========================================================
        public async Task<bool> UpdateCategoryAsync(int categoryId, CreateCategoryDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.ReadByIdAsync(categoryId);
            if (category == null || category.IsDeleted)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
                category.CategoryName = dto.CategoryName;

            if (!string.IsNullOrWhiteSpace(dto.CategoryDescription))
                category.CategoryDescription = dto.CategoryDescription;

            // 🧩 Handle image update
            if (dto.Image != null)
            {
                var merchantName = _getUserServices.GetUserNameFromToken() ?? "merchant";
                var newUrl = await _fileStorageService.UploadFileAsync(dto.Image, $"categories/{merchantName}");
                category.ImageUrl = newUrl;
            }

            category.UpdatedBy = int.TryParse(_getUserServices.GetMerchantIdFromToken(), out var id) ? id : 0;

            _unitOfWork.CategoryRepository.Update(category);
            return await _unitOfWork.SaveChanges() > 0;
        }

        // =========================================================
        // 🔹 DELETE CATEGORY
        // =========================================================
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.ReadByIdAsync(categoryId);
            if (category == null)
                return false;

            category.IsDeleted = true;
            category.DeletedDate = DateTime.UtcNow;
            category.DeletedBy = int.TryParse(_getUserServices.GetMerchantIdFromToken(), out var id) ? id : 0;

            _unitOfWork.CategoryRepository.Update(category);
            return await _unitOfWork.SaveChanges() > 0;
        }

        // =========================================================
        // 🔹 GET ALL CATEGORIES (Paged)
        // =========================================================
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

        // =========================================================
        // 🔹 GET CATEGORY BY ID
        // =========================================================
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

        // =========================================================
        // 🔹 GET PRODUCT COUNT BY CATEGORY
        // =========================================================
        public async Task<int> GetProductCountByCategoryIdAsync(int categoryId)
        {
            var allProducts = await _unitOfWork.ProductRepository.ReadAsync();
            return allProducts.Count(p => p.Type.CategoryId == categoryId && !p.IsDeleted);
        }

        // =========================================================
        // 🔹 GET PRODUCTS BY CATEGORY
        // =========================================================
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

        // =========================================================
        // 🔹 GET PRODUCTS BY SUBCATEGORY
        // =========================================================
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
