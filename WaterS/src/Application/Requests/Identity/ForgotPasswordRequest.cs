using System.ComponentModel.DataAnnotations;

namespace WaterS.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}