using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(450)")]
        public string AdminId { get; set; }

        [PersonalData]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string Localization { get; set; }

        [PersonalData]
        [Column(TypeName = "decimal(9,6)")]
        public float latitude { get; set; }

        [PersonalData]
        [Column(TypeName = "decimal(9,6)")]
        public float longtitude { get; set; }

        [PersonalData]
        [Column(TypeName = "bit")]
        public bool RemoteWork { get; set; }

        [PersonalData]
        [Column(TypeName = "image")]
        public byte[] TeamPicture { get; set; }
    }
}
