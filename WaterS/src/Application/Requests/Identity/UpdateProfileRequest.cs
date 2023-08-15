using System.ComponentModel.DataAnnotations;

namespace WaterS.Application.Requests.Identity
{
    public class UpdateProfileRequest
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}