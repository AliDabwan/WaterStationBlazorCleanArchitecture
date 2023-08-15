using System.ComponentModel.DataAnnotations;

namespace WaterS.Application.Requests.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public string KindName { get; set; }
        public string KindAdress { get; set; }
        public string KindRes { get; set; }

        public bool ActivateUser { get; set; } = false;
        public bool AutoConfirmEmail { get; set; } = false;
        [Required]
        public string KindType { get; set; }//Company/Station/Driver/Admin/Owner
        public string KindTypeAr { get; set; }//
        public int KindId { get; set; }//
    }
}