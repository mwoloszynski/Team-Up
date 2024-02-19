using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TeamUp.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the TeamUpUser class
    public class TeamUpUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [PersonalData]
        [Column(TypeName = "datetime")]
        public DateTime BirthDate { get; set; }

        [PersonalData]
        [Column(TypeName = "image")]
        public byte[] ProfilePicture { get; set; }

        [PersonalData]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        [PersonalData]
        [Column(TypeName = "bit")]
        public bool isAdmin { get; set; }

        [PersonalData]
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string Localization { get; set; }
    }
}
