using Cartify.Application.Contracts.TransactionsDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Infrastructure.Implementation.Repository;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantTransactionServices : IMerchantTransactionServices
    {
        public Task<PagedResult<TransactionDto>> GetTransactionsByStoreIdAsync(int storeId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionSummaryDto> GetTransactionSummaryAsync(int storeId, string period = "monthly")
        {
            throw new NotImplementedException();
        }
    }
}