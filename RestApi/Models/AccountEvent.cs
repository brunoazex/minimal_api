using System.Text.Json.Serialization;

namespace RestApi.Models
{
    public class AccountEvent
    {
        public Account Origin { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Account? Destination { get; }

        public AccountEvent(Account origin, Account? destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public AccountEvent(Account origin)
            : this(origin, null) { }
    }
}
