using Cartify.Application.Mappings;
using Cartify.Application.Services.Implementation;
using Cartify.Application.Services.Implementation.Authentication;
using Cartify.Application.Services.Implementation.Category;
using Cartify.Application.Services.Interfaces;
using Cartify.Application.Services.Interfaces.Authentication;
using Cartify.Domain.Interfaces.Repositories;
using Cartify.Domain.Models;
using Cartify.Infrastructure.Implementation.Repository;
using Cartify.Infrastructure.Implementation.Services;
using Cartify.Infrastructure.Implementation.Services.Helper;
using Cartify.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					policy =>
					{
						policy.AllowAnyOrigin()   
							  .AllowAnyMethod()   
							  .AllowAnyHeader();
					});
			});
			builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddIdentityCore<TblUser>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
			
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<ILoginService, LoginService>();
			builder.Services.AddScoped<IRegisterService, RegisterService>();
			builder.Services.AddScoped<ICreateJWTToken,CreateJWTToken>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();

			builder.Services.AddScoped<IProductServices, ProductServices>();

			builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddScoped<IEmailSender,EmailSender>();
			builder.Services.AddScoped<IResetPassword, ResetPassword>();
			builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

			builder.Services.AddOpenApi();
			builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Jwt"));
			builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("Smtp"));

			builder.Services.AddHttpContextAccessor();

			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			builder.Services.AddOpenApi();
			builder.Services.AddEndpointsApiExplorer();

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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
					ClockSkew=TimeSpan.Zero
				};
			});

			builder.Services.AddSwaggerGen(option => {
				var filepath = Path.Combine(System.AppContext.BaseDirectory, "Cartify.API.xml");
				option.IncludeXmlComments(filepath);
				option.SwaggerDoc("v1",
				   new OpenApiInfo
				   {
					   Title = "Cartify API",
					   Version = "v1",
					   Description = " ASP.NET Core WebAPI for Ecommerce ",
					   TermsOfService = new Uri("http://tempuri.org/terms"),
					   Contact = new OpenApiContact
					   {
						   Name = "Taqeyy Eldeen",
						   Email = "atakieeldeen@gmail.com",
					   },
				   });
				option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
				{
					Name="Authorization",
					In=ParameterLocation.Header,
					Type=SecuritySchemeType.ApiKey,
					Scheme=JwtBearerDefaults.AuthenticationScheme
				});

				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference=new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id=JwtBearerDefaults.AuthenticationScheme

							},

							In=ParameterLocation.Header,
							Scheme="Oauth2",
							Name="Authorization"
							


						},new List<string>()
					}

				});
			});


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseCors("AllowAll");
				app.MapOpenApi();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
