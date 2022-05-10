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
            var destination = GetById(request.Destination);
            if (destination == null)
            {
                destination = new Account(request.Destination);
                _repository.Add(destination);
            }

            destination.Deposit(request.Amount);
            return new AccountEvent(destination);
        }

        private AccountEvent HandleWithdrawRequest(NewEvent request)
        {
            var destination = GetById(request.Destination);
            if (destination == null)
                throw new AccountServiceException("Conta de destino não encontrada", HttpStatusCode.NotFound);

            if (!destination.HasEnoughFunds(request.Amount))
                throw new AccountServiceException("Não há saldo suficiente para o saque", HttpStatusCode.Forbidden);

            destination.Withdraw(request.Amount);
            return new AccountEvent(destination);
        }

        private AccountEvent HandleTransferRequest(NewEvent request)
        {
            if (request.Origin is null)
                throw new AccountServiceException("Conta de origem não informada", HttpStatusCode.UnprocessableEntity);

            var origin = GetById(request.Origin);
            if (origin == null)
                throw new AccountServiceException("Conta de origem não encontrada", HttpStatusCode.NotFound);

            if (!origin.HasEnoughFunds(request.Amount))
                throw new AccountServiceException("Não há saldo suficiente para o saque", HttpStatusCode.Forbidden);

            var destination = GetById(request.Destination);
            if (destination == null)
                throw new AccountServiceException("Conta de destino não encontrada", HttpStatusCode.NotFound);

            origin.Transfer(destination, request.Amount);
            return new AccountEvent(origin, destination);
        }


        public AccountEvent MakeOperation(NewEvent request)
        {
            if (request.Amount <= 0)
                throw new AccountServiceException($"Valor inválido", HttpStatusCode.UnprocessableEntity);

            if (!_handlers.TryGetValue(request.Type.ToLowerInvariant(), out var handler))
                throw new AccountServiceException($"Operação {request.Type} desconhecida", HttpStatusCode.UnprocessableEntity);

            return handler.Invoke(request);
        }

        public decimal GetBalance(string accountId)
        {
            var account = GetById(accountId);
            if (account == null)
                throw new AccountServiceException("Conta não encontrada", HttpStatusCode.NotFound);
            return account.Balance;
        }

        public void Reset()
            => _repository.Clear();
    }
}
