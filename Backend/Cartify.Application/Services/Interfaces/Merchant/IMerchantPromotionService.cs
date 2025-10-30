using System.Collections.Generic;
using System.Threading.Tasks;
using Cartify.Application.Contracts.PromotionsDtos;

namespace Cartify.Application.Services.Interfaces.Merchant
{
    public interface IMerchantPromotionServices
    {
        Task<bool> AddPromotionAsync(CreatePromotionDto dto);

        Task<bool> UpdatePromotionAsync(int promotionId, UpdatePromotionDto dto);

        Task<bool> DeletePromotionAsync(int promotionId);

        Task<PromotionDto?> GetPromotionByProductDetailIdAsync(int productDetailId);

        Task<IEnumerable<PromotionDto>> GetPromotionsBySubCategoryIdAsync(int subCategoryId);

        Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync();
    }
}
