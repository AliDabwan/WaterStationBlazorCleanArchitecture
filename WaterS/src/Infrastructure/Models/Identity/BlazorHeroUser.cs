using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WaterS.Application.Interfaces.Chat;
using WaterS.Application.Models.Chat;
using WaterS.Domain.Contracts;

namespace WaterS.Infrastructure.Models.Identity
{
    public class BlazorHeroUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }

        public DateTime CreatedOn { get; set; }
        public int AccountId { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string KindType { get; set; }//Company/Station/Driver/Admin/Owner
        public string KindTypeAr { get; set; }//
        public int KindId { get; set; }//
        public int StationId { get; set; }//
        public int DriverId { get; set; }//
        public int CustomerId { get; set; }//

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryFromUsers { get; set; }
        public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryToUsers { get; set; }

        public BlazorHeroUser()
        {
            ChatHistoryFromUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
            ChatHistoryToUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
        }
    }
}