using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestApi.Models;
using RestApi.Services;
using System;

namespace RestApiTests
{

    [TestClass]
    public class AccountServiceTests
    {
        private readonly CompareLogic _comparison;

        public AccountServiceTests()
        {
            _comparison = new CompareLogic();
        }

        [TestMethod]
        public void GetBalance_ShouldActSuccessfully()
        {
            //Arrange
            var expectedAmount = 10m;
            var mockedDestination = "100";
            var service = new AccountService();
            var expectedResult = ServiceResult.Success(data: expectedAmount);
            //Act
            service.MakeOperation(new NewEvent { Amount = expectedAmount, Destination = mockedDestination, Type = "deposit" });
            var result = service.GetBalance(mockedDestination);

            //Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("100")]
        [DataRow("200")]
        public void GetBalance_ShouldRaiseException(string invalidAccount)
        {
            //Arrange
            var expectedResult = ServiceResult.Error(System.Net.HttpStatusCode.NotFound);
            var service = new AccountService();

            //Act
            var result = service.GetBalance(invalidAccount);

            // Assert

            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [TestMethod]
        public void MakeOperationDeposit_ShouldCreateSuccessfully()
        {
            //Arrange
            var expectedResult = ServiceResult.Success(System.Net.HttpStatusCode.Created, AccountEvent.FromDestination(new Account("100", 10)));
            var service = new AccountService();

            //Act
            var result = service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });

            //Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        public void MakeOperationDeposit_ShouldActSuccessfully()
        {
            //Arrange
            var expectedResult = ServiceResult.Success(System.Net.HttpStatusCode.Created, AccountEvent.FromDestination(new Account("100", 20)));
            var service = new AccountService();

            //Act
            service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });
            var result = service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });

            //Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [TestMethod]
        public void MakeOperationWithdraw_ShouldActSuccessfully()
        {
            //Arrange
            var mockedEvent = new NewEvent { Amount = 10, Origin = "100", Type = "withdraw" };
            var expectedResult = ServiceResult.Success(System.Net.HttpStatusCode.Created, AccountEvent.FromOrigin(new Account("100")));

            var service = new AccountService();
            service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });

            //Act
            var result = service.MakeOperation(mockedEvent);

            //Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [TestMethod]
        public void MakeOperationTransfer_ShouldActSuccessfully()
        {
            //Arrange
            var mockedEvent = new NewEvent { Amount = 10, Origin = "100", Destination = "200", Type = "transfer" };
            var expectedResult = ServiceResult.Success(System.Net.HttpStatusCode.Created, AccountEvent.From(new Account("100"), new Account("200", 20)));

            var service = new AccountService();
            service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });
            service.MakeOperation(new NewEvent { Amount = 10, Destination = "200", Type = "deposit" });

            //Act
            var result = service.MakeOperation(mockedEvent);

            //Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("invalid", 10.0)]
        [DataRow("deposit", 0.0)]
        [DataRow("transfer", -100.0)]
        [DataRow(null, 0.0)]
        public void MakeOperation_ShouldRaiseException(string type, double amount)
        {
            //Arrange
            var mockedRequest = new NewEvent { Amount = Convert.ToDecimal(amount), Destination = "100", Type = type };
            var service = new AccountService();
            service.MakeOperation(new NewEvent { Amount = 10, Destination = "100", Type = "deposit" });
            var expectedResult = ServiceResult.Error(System.Net.HttpStatusCode.UnprocessableEntity);

            //Act
            var result = service.MakeOperation(mockedRequest);

            // Assert
            _comparison.Compare(expectedResult, result).AreEqual.Should().BeTrue();
        }
    }
}