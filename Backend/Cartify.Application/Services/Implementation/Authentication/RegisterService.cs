﻿using AutoMapper;
using Cartify.Application.Contracts.AuthenticationDtos;
using Cartify.Application.Services.Implementation.Helper;
using Cartify.Application.Services.Interfaces.Authentication;
using Cartify.Domain.Models;

namespace Cartify.Application.Services.Implementation.Authentication
{	
	public class RegisterService : IRegisterService
	{
		private IMapper _mapper;
		private readonly ICreateJWTToken _createJWT;
		private readonly IUserService _userService;

		public RegisterService( IMapper mapper,ICreateJWTToken createJWT, IUserService userService)
		{
			_mapper = mapper;
			_createJWT = createJWT;
			_userService = userService;
		}

		public async Task<string> Register(dtoRegister register)
		{
			var user=_mapper.Map<TblUser>(register);
			var address = _mapper.Map<TblAddress>(register);
			var check = await _userService.GetByEmail(user.Email);
			if (check != null)
			{
				return ResultService.Failure("Email already exists!");
			}

			var check2 = await _userService.GetByUsername(user.UserName);
			if (check2 != null)
			{
				return ResultService.Failure("username already exists!");
			}
			user.TblAddresses.Add(address);
			var result = await _userService.CreateUserAsync(user, register.Password);

			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				return ResultService.Failure($"User creation failed! Reasons: {errors}");
			}

			await _userService.AddRoleToUserAsync(user, "User");
			return ResultService.Success();
		}
	}
}
