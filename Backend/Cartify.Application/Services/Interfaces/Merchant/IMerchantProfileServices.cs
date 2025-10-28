using Cartify.Application.Contracts.ProfileDtos;
using System.Threading.Tasks;

namespace Cartify.Application.Services.Interfaces.Merchant
{
    public interface IMerchantProfileServices
    {
        Task<MerchantProfileDto?> GetProfileByUserIdAsync(string userId);
        Task<bool> UpdateProfileAsync(UpdateMerchantProfileDto dto);
        Task<bool> UpdateStoreInfoAsync(UpdateStoreInfoDto dto);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
}
