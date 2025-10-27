using Cartify.Application.Contracts.ProfileDtos;
using Cartify.Application.Services.Interfaces.Merchant;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantProfileServices : IMerchantProfileServices
    {
        public Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<MerchantProfileDto?> GetProfileByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProfileAsync(UpdateMerchantProfileDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStoreInfoAsync(UpdateStoreInfoDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
