using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Dtos;

public class PaymentRequestDto
{
    [Required]
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be between 14 and 19 numeric characters.")]
    public string CardNumber { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    public int ExpiryMonth { get; set; }

    [Required]
    [Range(2024, 9999, ErrorMessage = "Expiry year must be in the future.")]
    public int ExpiryYear { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
    public string Currency { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer representing the minor currency unit.")]
    public int Amount { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 numeric characters.")]
    public string CVV { get; set; }
}
