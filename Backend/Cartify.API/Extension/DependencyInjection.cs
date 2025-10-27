using Cartify.Application.Mappings;
using Cartify.Application.Services.Implementation;
using Cartify.Application.Services.Implementation.Authentication;
using Cartify.Application.Services.Implementation.Merchant;
using Cartify.Application.Services.Implementation.Profile;
using Cartify.Application.Services.Interfaces.Authentication;
using Cartify.Application.Services.Interfaces.Merchant;
using Cartify.Application.Services.Interfaces.Product;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using Cartify.Infrastructure.Implementation.Services;
using Cartify.Infrastructure.Implementation.Services.Helper;
using Cartify.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Cartify.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCartifyServices(this IServiceCollection services)
        {
            // 🧱 Infrastructure Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 🧠 Authentication
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<ICreateJWTToken, CreateJWTToken>();
            services.AddScoped<IResetPassword, ResetPassword>();

            // 📧 Email + Helper
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICreateMerchantProfile, CreateMerchantProfile>();

            // 👤 Profile
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IProfileServices, ProfileServices>();

            // 🛍️ Merchant Services
            services.AddScoped<IMerchantProductServices, MerchantProductServices>();
            services.AddScoped<IMerchantCategoryServices, MerchantCategoryServices>();
            services.AddScoped<IMerchantCustomerServices, MerchantCustomerServices>();
            services.AddScoped<IMerchantInventoryServices, MerchantInventoryServices>();
            services.AddScoped<IMerchantOrderServices, MerchantOrderServices>();
            services.AddScoped<IMerchantTransactionServices, MerchantTransactionServices>();
            services.AddScoped<IMerchantProfileServices, MerchantProfileServices>();

            // 🧩 Mapping Profiles
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
