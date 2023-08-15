﻿using System.ComponentModel.DataAnnotations;

namespace WaterS.Application.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }
       

        [Required]
        public string Password { get; set; }
    }
}