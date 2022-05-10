using RestApi.Models;
using System.Net;

namespace RestApi.Services
{
    public class AccountService
    {
        private readonly List<Account> _repository;
        private readonly Dictionary<string, Func<NewEvent, AccountEvent>> _handlers;
        public AccountService()
        {
            _repository = new List<Account>();
            _handlers = new Dictionary<string, Func<NewEvent, AccountEvent>>
            {
                { "deposit", HandleDepositRequest },
                { "withdraw", HandleWithdrawRequest },
                { "transfer", HandleTransferRequest }
            };
        }
        private Account? GetById(string accountId)
            => _repository.SingleOrDefault(acc => acc.Id == accountId);

        private AccountEvent HandleDepositRequest(NewEvent request)
        {
            var destination = GetById(request.Destination ?? throw new AccountServiceException());
            if (destination == null)
            {
                destination = new Account(request.Destination);
                _repository.Add(destination);
            }

            destination.Deposit(request.Amount);
            return AccountEvent.FromDestination(destination);
        }

        private AccountEvent HandleWithdrawRequest(NewEvent request)
        {
            var origin = GetById(request.Origin ?? throw new AccountServiceException());
            if (origin == null)
                throw new AccountServiceException(HttpStatusCode.NotFound);

            if (!origin.HasEnoughFunds(request.Amount))
                throw new AccountServiceException();

            origin.Withdraw(request.Amount);
            return AccountEvent.FromOrigin(origin);
        }

        private AccountEvent HandleTransferRequest(NewEvent request)
        {

            var origin = GetById(request.Origin ?? throw new AccountServiceException());
            if (origin == null)
                throw new AccountServiceException(HttpStatusCode.NotFound);

            if (!origin.HasEnoughFunds(request.Amount))
                throw new AccountServiceException();

            var destination = GetById(request.Destination ?? throw new AccountServiceException());
            if (destination == null)
            {
                destination = new Account(request.Destination);
                _repository.Add(destination);
            }

            origin.Transfer(destination, request.Amount);
            return AccountEvent.From(origin, destination);
        }


        public AccountEvent MakeOperation(NewEvent request)
        {
            if (request.Amount <= 0)
                throw new AccountServiceException();

            if (!_handlers.TryGetValue(request.Type.ToLowerInvariant(), out var handler))
                throw new AccountServiceException();

            return handler.Invoke(request);
        }

        public decimal GetBalance(string accountId)
        {
            var account = GetById(accountId ?? throw new AccountServiceException());
            if (account == null)
                throw new AccountServiceException(HttpStatusCode.NotFound);
            return account.Balance;
        }

        public void Reset()
            => _repository.Clear();
    }
}
