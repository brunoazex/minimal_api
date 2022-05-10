using System.Text.Json.Serialization;

namespace RestApi.Models
{
    public class AccountEvent
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Account? Origin { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Account? Destination { get; }

        private AccountEvent(Account? destination, Account? origin)
        {
            Destination = destination;
            Origin = origin;
        }


        public static AccountEvent FromOrigin(Account origin)
            => new(destination: null, origin: origin);

        public static AccountEvent FromDestination(Account destination)
            => new(destination: destination, origin: null);

        public static AccountEvent From(Account origin, Account destination)
            => new(destination: destination, origin: origin);

    }
}
