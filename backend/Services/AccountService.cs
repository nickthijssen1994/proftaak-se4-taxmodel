﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using backend.DAL.Repositories;
using backend.Helpers;
using backend.Models;
using backend.Models.DTOs.Accounts;
using backend.Security;
using Microsoft.Extensions.Options;

namespace backend.Services
{
	public class AccountService : IAccountService
	{
		private readonly AccountRepository repository;
		private readonly IMapper mapper;
		private TokenHandler tokenHandler;

		public AccountService(AccountRepository repository, IMapper mapper, IOptions<AppSettings> appSettings)
		{
			this.mapper = mapper;
			this.repository = repository;
			tokenHandler = new TokenHandler(appSettings);
		}

		public AccountDto GetById(long id)
		{
			Account account = repository.GetEntityById(id);
			return mapper.Map<AccountDto>(account);
		}

		public AccountDto GetByName(string name)
		{
			Account account = repository.GetEntities<Account>(a => a.Name == name).Single();
			return mapper.Map<AccountDto>(account);
		}

		public bool CheckNameExists(string name)
		{
			try
			{
				Account account = repository.GetEntities<Account>(a => a.Name == name).Single();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool CheckEmailExists(string email)
		{
			try
			{
				Account account = repository.GetEntities<Account>(a => a.Email == email).Single();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public Registration Register(RegisterDto registerDto)
		{
			// Generate jwt.
			string token = tokenHandler.GenerateToken(registerDto.Name);

			PasswordHasher hasher = new PasswordHasher();
			registerDto.Password = hasher.GenerateHash(registerDto.Password); // Hash password before registration.

			// Save to storage.
			Account account = mapper.Map<Account>(registerDto);
			repository.InsertEntity(account);
			repository.Save();

			return new Registration(registerDto.Email, registerDto.Name, token);
		}

		public JwtToken Login(string name)
		{
			// Generate jwt.
			return new JwtToken(tokenHandler.GenerateToken(name));
		}

		public void Delete(AccountDto dto)
		{
			Account account = mapper.Map<Account>(dto);
			repository.DeleteEntity(account);
			repository.Save();
		}

		public IEnumerable<AccountDto> GetAll()
		{
			IEnumerable<Account> accounts = repository.GetEntities<Account>();
			return mapper.Map<IEnumerable<AccountDto>>(accounts);
		}

		public EditAccountDto Update(EditAccountDto editAccountDto)
		{
			Account account = mapper.Map<Account>(editAccountDto);
			repository.UpdateEntity(account);
			repository.Save();
			return editAccountDto;
		}
	}
}
