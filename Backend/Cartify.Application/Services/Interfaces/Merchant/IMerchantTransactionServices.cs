using Cartify.Application.Contracts.TransactionsDtos;
using Cartify.Infrastructure.Implementation.Repository;
using System.Threading.Tasks;

namespace Cartify.Application.Services.Interfaces.Merchant
{
    public interface IMerchantTransactionServices
    {
        Task<PagedResult<TransactionDto>> GetTransactionsByStoreIdAsync(int storeId, int page = 1, int pageSize = 10);
        Task<TransactionSummaryDto> GetTransactionSummaryAsync(int storeId, string period = "monthly");
    }
}
