using Cartify.Application.Contracts;
using Cartify.Application.Contracts.AttributeDtos;
using Cartify.Application.Contracts.ProductDtos;
using Cartify.Application.Contracts.PromotionDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantProductServices : IMerchantProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public MerchantProductServices(IUnitOfWork _unitOfWork, IFileStorageService _fileStorageService)
        {
            this._unitOfWork = _unitOfWork;
            this._fileStorageService = _fileStorageService;
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
        public async Task<bool> AddProductImagesAsync(int productId, List<IFormFile> images)
        {
            var product = await _unitOfWork.ProductRepository.ReadByIdAsync(productId);
            if (product == null)
                return false;

            if (images == null || images.Count == 0)
                return false;

            product.TblProductImages ??= new List<TblProductImage>();

            var productImagesDto = new List<ProductImageDto>();

            foreach (var image in images)
            {
                var imageUrl = await _fileStorageService.UploadFileAsync(image, "products");

                var imageDto = new ProductImageDto
                {
                    ProductId = productId,
                    ImageURL = imageUrl,
                    IsDeleted = false,
                    CreatedBy = 1
                };
                productImagesDto.Add(imageDto);
                var productImageEntity = new TblProductImage
                {
                    ProductId = imageDto.ProductId,
                    ImageURL = imageDto.ImageURL,
                    IsDeleted = imageDto.IsDeleted,
                    CreatedBy = imageDto.CreatedBy
                };
                product.TblProductImages.Add(productImageEntity);
            }
            await _unitOfWork.SaveChanges();
            return true;
        }



        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.ReadByIdAsync(productId);
            if (product == null)
                return false;

            if (product.TblProductDetails != null && product.TblProductDetails.Any())
            {
                foreach (var detail in product.TblProductDetails)
                {
                    detail.IsDeleted = true;
                }
            }

            if (product.TblProductImages != null && product.TblProductImages.Any())
            {
                foreach (var image in product.TblProductImages)
                {
                    image.IsDeleted = true;
                }
            }

            product.IsDeleted = true;

            return await _unitOfWork.SaveChanges() > 0;
        }


        public async Task<PagedResult<ProductDto>> GetAllProductsByMerchantIdAsync(string merchantId, int page = 1, int pageSize = 10)
        {
            var store = await _unitOfWork.UserStorerepository.Search(s => s.UserId == merchantId && !s.IsDeleted);

            if (store == null)
                return new PagedResult<ProductDto>(Enumerable.Empty<ProductDto>(), 0, page, pageSize);

            var allProducts = await _unitOfWork.ProductRepository.GetAllIncluding(
                p => p.Type,
                p => p.Type.Category,
                p => p.TblProductImages
            );

            var productsQuery = allProducts
                .Where(p => p.UserStoreId == store.UserStorId && !p.IsDeleted)
                .AsQueryable();

            var totalCount = productsQuery.Count();

            var pagedProducts = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productDtos = pagedProducts.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                TypeName = p.Type?.TypeName ?? "",
                CategoryName = p.Type?.Category?.CategoryName ?? "",
                StoreId = p.UserStoreId,
                ImageUrl = p.TblProductImages != null && p.TblProductImages.Any()
                            ? p.TblProductImages.First().ImageURL
                            : null
            }).ToList();

            return new PagedResult<ProductDto>(productDtos, totalCount, page, pageSize);
        }


        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository
                .Search(p => p.ProductId == productId);

            if (product == null)
                return null;

            var type = product.Type;
            var category = type?.Category;
            var images = product.TblProductImages;

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                TypeName = type?.TypeName ?? "N/A",
                CategoryName = category?.CategoryName ?? "N/A",
                StoreId = product.UserStoreId,
                ImageUrl = images != null && images.Any()
                            ? images.First().ImageURL
                            : null
            };

            return productDto;
        }


        public async Task<PagedResult<ProductDto>> GetProductsByNameAsync(string name, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new PagedResult<ProductDto>(Enumerable.Empty<ProductDto>(), 0, page, pageSize);
            var allProducts = await _unitOfWork.ProductRepository.GetAllIncluding(
                p => p.Type,
                p => p.Type.Category,
                p => p.TblProductImages
            );
            var filteredProducts = allProducts
                .Where(p => p.ProductName.Contains(name) && !p.IsDeleted)
                .AsQueryable();
            var totalCount = filteredProducts.Count();

            var pagedProducts = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var productDtos = pagedProducts.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                TypeName = p.Type?.TypeName ?? "",
                CategoryName = p.Type?.Category?.CategoryName ?? "",
                StoreId = p.UserStoreId,
                ImageUrl = p.TblProductImages != null && p.TblProductImages.Any()
                            ? p.TblProductImages.First().ImageURL
                            : null
            }).ToList();
            return new PagedResult<ProductDto>(productDtos, totalCount, page, pageSize);
        }


        public async Task<PagedResult<ProductDto>> GetProductsByTypeIdAsync(int typeId, int page = 1, int pageSize = 10)
        {
            if (typeId <= 0)
                return new PagedResult<ProductDto>(Enumerable.Empty<ProductDto>(), 0, page, pageSize);

            var allProducts = await _unitOfWork.ProductRepository.GetAllIncluding(
                p => p.Type,
                p => p.Type.Category,
                p => p.TblProductImages
            );

            var filteredProducts = allProducts
                .Where(p => p.TypeId == typeId && !p.IsDeleted)
                .AsQueryable();

            var totalCount = filteredProducts.Count();

            var pagedProducts = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productDtos = pagedProducts.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                TypeName = p.Type?.TypeName ?? "",
                CategoryName = p.Type?.Category?.CategoryName ?? "",
                StoreId = p.UserStoreId,
                ImageUrl = p.TblProductImages != null && p.TblProductImages.Any()
                    ? p.TblProductImages.First().ImageURL
                    : null
            }).ToList();

            return new PagedResult<ProductDto>(productDtos, totalCount, page, pageSize);
        }


        public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto dto)
        {
            var product = await _unitOfWork.ProductRepository.ReadByIdAsync(productId);
            if (product == null || product.IsDeleted)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.ProductName))
                product.ProductName = dto.ProductName;

            if (!string.IsNullOrWhiteSpace(dto.ProductDescription))
                product.ProductDescription = dto.ProductDescription;

            if (dto.TypeId.HasValue)
                product.TypeId = dto.TypeId.Value;

            if (dto.NewImages != null && dto.NewImages.Any())
            {
                if (product.TblProductImages != null)
                {
                    foreach (var old in product.TblProductImages)
                        old.IsDeleted = true;
                }

                product.TblProductImages = new List<TblProductImage>();
                foreach (var image in dto.NewImages)
                {
                    var url = await _fileStorageService.UploadFileAsync(image, "products");
                    product.TblProductImages.Add(new TblProductImage
                    {
                        ProductId = productId,
                        ImageURL = url,
                        CreatedBy = dto.UpdatedBy ?? 1,
                        IsDeleted = false
                    });
                }
            }

            _unitOfWork.ProductRepository.Update(product);
            return await _unitOfWork.SaveChanges() > 0;
        }
        public async Task<ProductDetailDto> GetProductDetailAsync(int productDetailId)
        {
            var query = _unitOfWork.ProductDetailsRepository.GetAllIncluding2(
                p => p.Product,                               
                p => p.Product.TblProductImages,             
                p => p.Inventory,                             
                p => p.LkpProductDetailsAttributes,           
                p => p.LkpProductDetailsAttributes.Select(a => a.Attribute),
                p => p.LkpProductDetailsAttributes.Select(a => a.MeasureUnit),
                p => p.Promotions                            
            );

            var productDetail = await query
                .FirstOrDefaultAsync(p => p.ProductDetailId == productDetailId && !p.IsDeleted);

            if (productDetail == null)
                return null;

            var dto = new ProductDetailDto
            {
                ProductDetailId = productDetail.ProductDetailId,
                Description = productDetail.Description,
                Price = productDetail.Price,
                QuantityAvailable = productDetail.Inventory?.QuantityAvailable ?? 0,
                Images = productDetail.Product?.TblProductImages?
                    .Select(i => i.ImageURL)
                    .ToList() ?? new List<string>(),
                Attributes = productDetail.LkpProductDetailsAttributes?
                    .Select(a => new AttributeDto
                    {
                        Name = a.Attribute?.Name,
                        MeasureUnit = a.MeasureUnit?.Name
                    })
                    .ToList() ?? new List<AttributeDto>(),
                Promotions = productDetail.Promotions?
                    .Select(p => new PromotionDto
                    {
                        PromotionName = p.PromotionName,
                        DiscountPercentage = p.DiscountPercentage
                    })
                    .ToList() ?? new List<PromotionDto>()
            };

            if (dto.Promotions.Any())
            {
                var totalDiscount = dto.Promotions.Sum(p => p.DiscountPercentage);
                dto.PriceAfterDiscount = dto.Price - (dto.Price * totalDiscount / 100);
            }

            return dto;
        }

        public async Task<bool> AddProductDetailAsync(CreateProductDetailDto dto)
        {
            var productDetail = new TblProductDetail
            {
                ProductId = dto.ProductId,
                SerialNumber = dto.SerialNumber,
                Price = dto.Price,
                Description = dto.Description,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            var inventory = new TblInventory
            {
                InventoryId = Guid.NewGuid().GetHashCode(), 
                ProductDetail = productDetail,
                QuantityAvailable = dto.QuantityAvailable,
                QuantityReserved = 0,
                CreatedDate = DateTime.Now
            };

            productDetail.LkpProductDetailsAttributes = dto.Attributes.Select(a => new LkpProductDetailsAttribute
            {
                AttributeId = a.AttributeId,
                MeasureUnitId = a.MeasureUnitId
            }).ToList();

            await _unitOfWork.ProductDetailsRepository.CreateAsync(productDetail);
            await _unitOfWork.InventoryRepository.CreateAsync(inventory);


            return await _unitOfWork.SaveChanges() > 0;
        }
        public async Task<bool> UpdateProductDetailAsync(UpdateProductDetailDto dto)
        {
            var productDetail = await _unitOfWork.ProductDetailsRepository
                .ReadByIdAsync(dto.ProductDetailId);

            if (productDetail == null || productDetail.IsDeleted)
                return false;

            productDetail.Price = dto.Price;
            productDetail.Description = dto.Description;

            if (productDetail.Inventory != null)
            {
                productDetail.Inventory.QuantityAvailable = dto.QuantityAvailable;
            }

            productDetail.LkpProductDetailsAttributes?.Clear();
            productDetail.LkpProductDetailsAttributes = dto.Attributes.Select(a => new LkpProductDetailsAttribute
            {
                ProductDetailId = dto.ProductDetailId,
                AttributeId = a.AttributeId,
                MeasureUnitId = a.MeasureUnitId
            }).ToList();

            _unitOfWork.ProductDetailsRepository.Update(productDetail);

            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> DeleteProductDetailAsync(int productDetailId)
        {
            var productDetail = await _unitOfWork.ProductDetailsRepository
                .ReadByIdAsync(productDetailId);

            if (productDetail == null || productDetail.IsDeleted)
                return false;

            productDetail.IsDeleted = true;
            productDetail.DeletedDate = DateTime.Now;

            _unitOfWork.ProductDetailsRepository.Update(productDetail);

            return await _unitOfWork.SaveChanges() > 0;
        }




    }
}