using System.Collections.Generic;

namespace WaterS.Application.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}