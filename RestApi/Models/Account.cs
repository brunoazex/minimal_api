namespace RestApi.Models
{
    public class Account
    {
        /// <summary>
        /// Account's Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Account's current balance
        /// </summary>
        public decimal Balance { get; private set; }

        public Account(string id, decimal initialBalance)
        {
            Id = id;
            Balance = initialBalance;
        }

        public Account(string id)
            : this(id, 0) { }

        public bool HasEnoughFunds(decimal amount)
            => Balance >= amount;

        /// <summary>
        /// Makes a deposit
        /// </summary>
        /// <param name="amount">Amount to be deposited</param>
        public void Deposit(decimal amount)
            => Balance += amount;

        /// <summary>
        /// Makes a withdraw
        /// </summary>
        /// <param name="amount">Amount to be withdrawn</param>
        public void Withdraw(decimal amount)
            => Balance -= amount;

        /// <summary>
        /// Makes a transfer between a party and a counter party account
        /// </summary>
        /// <param name="destination">Who is receiving the amount</param>
        /// <param name="amount">Amount to be transfered</param>
        /// <exception cref="ArgumentNullException"> When destination is not specified</exception>
        public void Transfer(Account destination, decimal amount)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));
            Balance -= amount;
            destination.Balance += amount;
        }
    }
}
