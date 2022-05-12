using RestApi.Models;
using System.Net;

namespace RestApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly List<Account> _repository;
        private readonly Dictionary<string, Func<NewEvent, ServiceResult>> _handlers;
        public AccountService()
        {
            _repository = new List<Account>();
            _handlers = new Dictionary<string, Func<NewEvent, ServiceResult>>
            {
                { "deposit", HandleDepositRequest },
                { "withdraw", HandleWithdrawRequest },
                { "transfer", HandleTransferRequest }
            };
        }
        private Account? GetById(string accountId)
            => _repository.SingleOrDefault(acc => acc.Id == accountId);

        private ServiceResult HandleDepositRequest(NewEvent request)
        {
            if (request.Destination is null)
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            var destination = GetById(request.Destination);
            if (destination == null)
            {
                destination = new Account(request.Destination);
                _repository.Add(destination);
            }

            destination.Deposit(request.Amount);
            return ServiceResult.Success(HttpStatusCode.Created, AccountEvent.FromDestination(destination));
        }

        private ServiceResult HandleWithdrawRequest(NewEvent request)
        {
            if (request.Origin is null)
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            var origin = GetById(request.Origin);
            if (origin == null)
                return ServiceResult.Error(HttpStatusCode.NotFound);

            if (!origin.HasEnoughFunds(request.Amount))
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            origin.Withdraw(request.Amount);
            return ServiceResult.Success(HttpStatusCode.Created, AccountEvent.FromOrigin(origin));
        }

        private ServiceResult HandleTransferRequest(NewEvent request)
        {
            if (request.Origin is null || request.Destination is null)
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            var origin = GetById(request.Origin);
            if (origin == null)
                return ServiceResult.Error(HttpStatusCode.NotFound);

            var destination = GetById(request.Destination);
            if (destination == null)
            {
                destination = new Account(request.Destination);
                _repository.Add(destination);
            }

            origin.Transfer(destination, request.Amount);
            return ServiceResult.Success(HttpStatusCode.Created, AccountEvent.From(origin, destination));
        }


        public ServiceResult MakeOperation(NewEvent request)
        {
            if (request.Amount <= 0)
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            if (!_handlers.TryGetValue(request.Type.ToLowerInvariant(), out var handler))
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            return handler.Invoke(request);
        }

        public ServiceResult GetBalance(string accountId)
        {
            if (accountId is null)
                return ServiceResult.Error(HttpStatusCode.UnprocessableEntity);

            var account = GetById(accountId);
            if (account == null)
                return ServiceResult.Error(HttpStatusCode.NotFound);
            return ServiceResult.Success(data: account.Balance);
        }

        public ServiceResult Reset()
        {
            _repository.Clear();
            return ServiceResult.Success();
        }

        public ServiceResult GetAccounts()
        {
            return ServiceResult.Success(data: _repository.ToList());
        }
    }
}
