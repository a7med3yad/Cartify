using Cartify.Application.Contracts.OrderDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Infrastructure.Implementation.Repository;
namespace Cartify.Application.Services.Implementation.Merchant
{

    public class MerchantOrderServices : IMerchantOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MerchantOrderServices(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }
        public Task<PagedResult<OrderDto>> FilterOrdersAsync(int storeId, DateTime? startDate, DateTime? endDate, string? status, int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDetailDto?> GetOrderByIdAsync(string orderId)
        {
            if (!int.TryParse(orderId, out int parsedId))
                return null;

            var order = await _unitOfWork.OrderRepository.ReadByIdAsync(parsedId);
            if (order == null)
                return null;

            var orderDetailDto = new OrderDetailDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                StatusId = order.OrderStatuesId,
                TotalAmount = order.GrantTotal,
                PaymentType = order.PaymentType?.Name,
                CustomerId = order.CustomerId,
                StoreName = order.UserStore?.StoreName,
                Items = order.TblOrderDetails?.Select(item => new OrderItemDto
                {
                    ProductName = item.Product?.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            };
            return orderDetailDto;
        }


        public async Task<PagedResult<OrderDetailDto>> GetOrdersByStoreIdAsync(int storeId, int page = 1, int pageSize = 10)
        {
            var ordersQuery = _unitOfWork.OrderRepository
                .GetAllIncluding2(o => o.StoreId == storeId)
                ?.AsQueryable();

            if (ordersQuery == null || !ordersQuery.Any())
                return new PagedResult<OrderDetailDto>(new List<OrderDetailDto>(), 0, page, pageSize);

            var totalOrders = ordersQuery.Count();

            var pagedOrders = ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDetailDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    StatusId = o.OrderStatuesId,
                    TotalAmount = o.GrantTotal,
                    CustomerId = o.CustomerId,
                    StoreName = o.UserStore != null ? o.UserStore.StoreName : null,
                    PaymentType = o.PaymentType != null ? o.PaymentType.Name : null
                })
                .ToList();

            var result = new PagedResult<OrderDetailDto>(pagedOrders, totalOrders, page, pageSize);
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            if (!int.TryParse(orderId, out int parsedId))
                return false;

            var order = await _unitOfWork.OrderRepository.ReadByIdAsync(parsedId);
            if (order == null)
                return false;

            var status = await _unitOfWork.OrderStatusRepository
                .GetAllIncluding(s => s.Name.ToLower() == newStatus.ToLower());

            var statusEntity = status?.FirstOrDefault();
            if (statusEntity == null)
                return false;

            order.OrderStatuesId = statusEntity.OrderStatuesId;
            order.UpdatedBy ??= 0;

            _unitOfWork.OrderRepository.Update(order);
            var affectedRows = await _unitOfWork.SaveChanges();

            return affectedRows > 0;
        }


    }

}
