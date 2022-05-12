using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using RestApi.Models;
using RestApi.Modules;
using RestApi.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace RestApiTests
{

    [TestClass]
    public class TransactionModuleTests
    {
        private readonly CompareLogic _comparison;

        public TransactionModuleTests()
        {
            _comparison = new CompareLogic();
        }

        [TestMethod]
        public void Test_HandleBalance()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedAccountId = "100";
            var expectedBalance = 100m;
            var expectedResult = Results.Json(expectedBalance, statusCode: 200);

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.GetBalance(It.Is<string>(current => _comparison.Compare(mockedAccountId, current).AreEqual)))
                .Returns(ServiceResult.Success(data: expectedBalance))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleBalance(mockedAccountId);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }

        [TestMethod]
        public void Test_HandleBalanceNotFound()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedAccountId = "100";
            var expectedResult = Results.Json(data: null, statusCode: 404);

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.GetBalance(It.Is<string>(current => _comparison.Compare(mockedAccountId, current).AreEqual)))
                .Returns(ServiceResult.Error(System.Net.HttpStatusCode.NotFound))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleBalance(mockedAccountId);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }

        [TestMethod]
        public void Test_HandleDeposit()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedAccount = new Account("100", 100m);

            var expectedResult = Results.Json(AccountEvent.FromDestination(mockedAccount), statusCode: 201);
            var mockedRequest = new NewEvent { Amount = mockedAccount.Balance, Destination = mockedAccount.Id, Type = "deposit" };

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.MakeOperation(It.Is<NewEvent>(current => _comparison.Compare(mockedRequest, current).AreEqual)))
                .Returns(ServiceResult.Success(System.Net.HttpStatusCode.Created, data: AccountEvent.FromDestination(mockedAccount)))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleEvent(mockedRequest);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }

        [TestMethod]
        public void Test_HandleWithdraw()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedAccount = new Account("100", 100m);

            var expectedResult = Results.Json(AccountEvent.FromOrigin(mockedAccount), statusCode: 201);
            var mockedRequest = new NewEvent { Amount = mockedAccount.Balance, Origin = mockedAccount.Id, Type = "withdraw" };

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.MakeOperation(It.Is<NewEvent>(current => _comparison.Compare(mockedRequest, current).AreEqual)))
                .Returns(ServiceResult.Success(System.Net.HttpStatusCode.Created, data: AccountEvent.FromOrigin(mockedAccount)))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleEvent(mockedRequest);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }

        [TestMethod]
        public void Test_HandleTransfer()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedOrigin = new Account("100", 100m);
            var mockedDestination = new Account("200", 100m);

            var expectedResult = Results.Json(AccountEvent.From(mockedOrigin, mockedDestination), statusCode: 201);
            var mockedRequest = new NewEvent { Amount = mockedOrigin.Balance, Origin = mockedOrigin.Id, Destination = mockedDestination.Id, Type = "transfer" };

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.MakeOperation(It.Is<NewEvent>(current => _comparison.Compare(mockedRequest, current).AreEqual)))
                .Returns(ServiceResult.Success(System.Net.HttpStatusCode.Created, data: AccountEvent.From(mockedOrigin, mockedDestination)))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleEvent(mockedRequest);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }

        [TestMethod]
        public void Test_HandleReset()
        {
            // Arrange
            var mock = new AutoMocker();
            var expectedResult = Results.StatusCode(200);

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.Reset())
                .Returns(ServiceResult.Success())
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleReset();

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();


        }
        [TestMethod]
        public void Test_HandleGetAccounts()
        {
            // Arrange
            var mock = new AutoMocker();
            var mockedAccounts = new List<Account>
            {
                new Account("100", 10),
                new Account("200", 20),
                new Account("300", 30),
            };

            var expectedResult = Results.Json(mockedAccounts, statusCode: 200);

            mock
                .GetMock<IAccountService>()
                .Setup(svc => svc.GetAccounts())
                .Returns(ServiceResult.Success(data: mockedAccounts))
                .Verifiable();

            var module = new TransactionModule(mock.GetMock<IAccountService>().Object);

            // Act
            var result = module.HandleGetAccounts();

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
            mock.Verify();

        }
    }
}
