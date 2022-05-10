using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class NewEvent
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }
    }
}
