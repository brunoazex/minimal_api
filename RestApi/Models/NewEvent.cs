using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class NewEvent
    {
        public string? Origin { get; set; }
        [Required]
        public string Destination { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
