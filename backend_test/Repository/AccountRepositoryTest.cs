﻿using System.Linq;
using backend.Models;
using backend.Models.DTOs.Accounts;
using backend.Services;
using Moq;
using NUnit.Framework;

namespace Repository
{
    internal class AccountRepositoryTest
    {
	    /// <summary>
	    ///     Registers user.
	    /// </summary>
	    [Test]
        public void Register()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var user = new RegisterDto();
            user.Email = "testemail";
            user.Name = "testUser";
            user.Password = "TestPass1";
            var registration = new Registration(user.Email, user.Name, user.Password);

            mock.Setup(AccountService => AccountService.Register(user)).Returns(registration);

            // Act
            var result = mock.Object.Register(user);

            // Assert
            Assert.AreEqual(user.Name, result.Name);
        }

        [Test]
        public void Login()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var user = new LoginDto();
            user.Name = "testUser";
            user.Password = "TestPass1";
            var token = new JwtToken("token");
            mock.Setup(AccountService => AccountService.Login(user.Name)).Returns(token);

            // Act
            var result = mock.Object.Login(user.Name);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetById()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var user = new AccountDto();
            user.Id = 1;
            mock.Setup(AccountService => AccountService.GetById(user.Id)).Returns(user);

            // Act
            var result = mock.Object.GetById(user.Id);

            // Assert
            Assert.AreEqual(user.Id, result.Id);
        }

        [Test]
        public void GetByName()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var user = new AccountDto();
            user.Name = "test";
            mock.Setup(AccountService => AccountService.GetByName(user.Name)).Returns(user);

            // Act
            var result = mock.Object.GetByName(user.Name);

            // Assert
            Assert.AreEqual(user.Name, result.Name);
        }

        [Test]
        public void CheckNameExists()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var name = "test";
            mock.Setup(AccountService => AccountService.CheckNameExists(name)).Returns(false);

            // Act
            var result = mock.Object.CheckNameExists(name);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckEmailExists()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var email = "test";
            mock.Setup(AccountService => AccountService.CheckEmailExists(email)).Returns(false);

            // Act
            var result = mock.Object.CheckEmailExists(email);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetAll()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            mock.Setup(AccountService => AccountService.GetAll());

            // Act
            var accounts = mock.Object.GetAll();

            // Assert
            Assert.AreEqual(0, accounts.Count());
        }

        [Test]
        public void Update()
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            var user = new EditAccountDto();
            user.Email = "testemail";
            user.Name = "testUser";
            user.Password = "TestPass1";
            mock.Setup(AccountService => AccountService.Update(user)).Returns(user);

            // Act
            var result = mock.Object.Update(user);

            // Assert
            Assert.AreEqual(user, result);
        }
    }
}