using Cartify.Application.Contracts.OrderDtos;
using Cartify.Application.Services.Interfaces.Merchant;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantOrderServices : IMerchantOrderServices
    {
        public Task<Infrastructure.Implementation.Repository.PagedResult<OrderDto>> FilterOrdersAsync(int storeId, DateTime? startDate, DateTime? endDate, string? status, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetailDto?> GetOrderByIdAsync(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<Infrastructure.Implementation.Repository.PagedResult<OrderDto>> GetOrdersByStoreIdAsync(int storeId, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            throw new NotImplementedException();
        }
    }

}
