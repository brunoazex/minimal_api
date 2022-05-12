using RestApi.Models;

namespace RestApi.Services
{
    public interface IAccountService
    {
        ServiceResult GetBalance(string accountId);
        ServiceResult MakeOperation(NewEvent request);
        ServiceResult Reset();
        ServiceResult GetAccounts();
    }
}