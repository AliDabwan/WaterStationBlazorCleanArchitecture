using System;

namespace WaterS.Application.Responses.Identity
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserName { get; set; }
        public string UserRoll { get; set; }
        public string KindType { get; set; }
        public string KindTypeAr { get; set; }
      

    }
}