using Cartify.Application.Contracts.CustomerDtos;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using System.Linq;

namespace Cartify.Application.Services.Implementation.Merchant
{
    public class MerchantCustomerServices : IMerchantCustomerServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MerchantCustomerServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var user = await _unitOfWork.ProfileRepository.Search(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
                return null;

            return new CustomerDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<int> GetCustomerCountAsync(int storeId)
        {
            var allOrders = await _unitOfWork.OrderRepository.GetAllIncluding(o => o.CustomerId);
            return allOrders
                .Where(o => o.StoreId == storeId)
                .Select(o => o.CustomerId)
                .Distinct()
                .Count();
        }
        public async Task<PagedResult<CustomerDto>> GetCustomersByStoreIdAsync(int storeId, int page = 1, int pageSize = 10)
        {
            var allOrders = await _unitOfWork.OrderRepository.GetAllIncluding(
                o => o.UserStore
            );

            var customerIds = allOrders
                .Where(o => o.StoreId == storeId)
                .Select(o => o.CustomerId.ToString())
                .Distinct()
                .ToList();

            var allCustomers = await _unitOfWork.ProfileRepository.GetAllIncluding();

            var customers = allCustomers
                .Where(c => customerIds.Contains(c.Id))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = customerIds.Count;

            var customerDtos = customers.Select(c => new CustomerDto
            {
                UserId = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            }).ToList();

            return new PagedResult<CustomerDto>(customerDtos, totalCount, page, pageSize);
        }

     
    }
}
