using AutoMapper;
using Cartify.Application.Contracts.PromotionDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantPromotionService : IMerchantPromotionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MerchantPromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddPromotionAsync(CreatePromotionDto dto)
        {
            var promotion = new LkpPromotion
            {
                PromotionName = dto.PromotionName,
                DiscountPercentage = dto.DiscountPercentage,
                ImgUrl = dto.ImgUrl,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _unitOfWork.PromotionsRepository.CreateAsync(promotion);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
        {
            var promotions = await _unitOfWork.PromotionsRepository
                .GetAllIncluding2()
                .AsNoTracking()
                .Select(p => new PromotionDto
                {
                    PromotionName = p.PromotionName,
                    DiscountPercentage = p.DiscountPercentage
                })
                .ToListAsync();

            return promotions;
        }

        public async Task<PromotionDto?> GetPromotionByProductDetailIdAsync(int productDetailId)
        {
            var promotion = await _unitOfWork.PromotionsRepository
                .GetAllIncluding2()
                .Include(p => p.ProductDetails)
                .AsNoTracking()
                .Where(p => p.ProductDetails.Any(pd => pd.ProductDetailId == productDetailId))
                .Select(p => new PromotionDto
                {
                    PromotionName = p.PromotionName,
                    DiscountPercentage = p.DiscountPercentage
                })
                .FirstOrDefaultAsync();

            return promotion;
        }

        public async Task<IEnumerable<PromotionDto>> GetPromotionsBySubCategoryIdAsync(int subCategoryId)
        {
            var promotions = await _unitOfWork.PromotionsRepository
                .GetAllIncluding2()
                .Include(p => p.ProductDetails)
                .ThenInclude(pd => pd.Product)
                .Where(p => p.ProductDetails.Any(pd => pd.Product.TypeId == subCategoryId))
                .AsNoTracking()
                .Select(p => new PromotionDto
                {
                    PromotionName = p.PromotionName,
                    DiscountPercentage = p.DiscountPercentage
                })
                .ToListAsync();

            return promotions;
        }

        public async Task<bool> UpdatePromotionAsync(int promotionId, UpdatePromotionDto dto)
        {
            var promotion = await _unitOfWork.PromotionsRepository.ReadByIdAsync(promotionId);
            if (promotion == null) return false;

            promotion.PromotionName = dto.PromotionName ?? promotion.PromotionName;
            promotion.DiscountPercentage = dto.DiscountPercentage ?? promotion.DiscountPercentage;
            promotion.ImgUrl = dto.ImgUrl ?? promotion.ImgUrl;
            promotion.StartDate = dto.StartDate ?? promotion.StartDate;
            promotion.EndDate = dto.EndDate ?? promotion.EndDate;

            _unitOfWork.PromotionsRepository.Update(promotion);
            return await _unitOfWork.SaveChanges() > 0;
        }
        public async Task<bool> DeletePromotionAsync(int promotionId)
        {
            var promotion = await _unitOfWork.PromotionsRepository.ReadByIdAsync(promotionId);
            if (promotion == null || promotion.IsDeleted)
                return false;
            promotion.IsDeleted = true;
            promotion.DeletedDate = DateTime.UtcNow;
            _unitOfWork.PromotionsRepository.Update(promotion);
            return await _unitOfWork.SaveChanges() > 0;
        }

    }
}
