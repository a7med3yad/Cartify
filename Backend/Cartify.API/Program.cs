using Amazon.S3;
using Cartify.Application.Mappings;
using Cartify.Application.Services.Implementation;
using Cartify.Application.Services.Implementation.Authentication;
using Cartify.Application.Services.Implementation.Helper;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Cartify.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>() // أو <Startup> حسب الكلاس الأساسي عندك
    .AddEnvironmentVariables();

            // 🔧 Load configurations
            //builder.Configuration
            //   .AddJsonFile("appsettings.json", optional: false)
            //   .AddUserSecrets<Program>()
            //   .AddEnvironmentVariables();

            // 🧾 Controllers
            builder.Services.AddControllers();

            // 🌐 CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // 🧩 Database Context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 👤 Identity Configuration
            builder.Services.AddIdentityCore<TblUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // 👥 User Services
            builder.Services.AddScoped<IUserService, UserService>();



            // 🔐 Authentication Services
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IRegisterService, RegisterService>();
            builder.Services.AddScoped<ICreateJWTToken, CreateJWTToken>();
            builder.Services.AddScoped<IResetPassword, ResetPassword>();
            builder.Services.AddHttpContextAccessor();


            // ☁️ Amazon S3 Configuration
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();

            // 🧱 Infrastructure Repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 📧 Helpers
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ICreateMerchantProfile, CreateMerchantProfile>();

            // 👤 Profile Services
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IProfileServices, ProfileServices>();

            // 🛍️ Merchant Services
            builder.Services.AddScoped<IMerchantProductServices, MerchantProductServices>();
            builder.Services.AddScoped<IMerchantCategoryServices, MerchantCategoryServices>();
            builder.Services.AddScoped<IMerchantCustomerServices, MerchantCustomerServices>();
            builder.Services.AddScoped<IMerchantInventoryServices, MerchantInventoryServices>();
            builder.Services.AddScoped<IMerchantOrderServices, MerchantOrderServices>();
            builder.Services.AddScoped<IMerchantTransactionServices, MerchantTransactionServices>();
            builder.Services.AddScoped<IMerchantProfileServices, MerchantProfileServices>();

            // 🧭 Mapping + Configurations
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<GetUserServices>();


            // 🧾 Swagger + OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                var filepath = Path.Combine(AppContext.BaseDirectory, "Cartify.API.xml");
                option.IncludeXmlComments(filepath);
                option.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Cartify API",
                        Version = "v1",
                        Description = "ASP.NET Core WebAPI for Ecommerce",
                        Contact = new OpenApiContact
                        {
                            Name = "Taqeyy",
                            Email = "atakieeldeen@gmail.com",
                        },
                    });
                option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            In = ParameterLocation.Header,
                            Name = "Authorization"
                        },
                        new List<string>()
                    }
                });
            });

            // 🔑 JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            var app = builder.Build();

            // 🚀 HTTP Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowFrontend");
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
