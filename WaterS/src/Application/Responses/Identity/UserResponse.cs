
namespace WaterS.Application.Responses.Identity
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureDataUrl { get; set; }



        public string KindName { get; set; }
        public string KindAdress { get; set; }
        public string KindRes { get; set; }

     
        public string KindType { get; set; }//Company/Station/Driver/Admin/Owner
        public string KindTypeAr { get; set; }//
        public int AccountId { get; set; }//

        public int KindId { get; set; }//
        public int StationId { get; set; }//
        public int DriverId { get; set; }//
        public int CustomerId { get; set; }//


    }
}