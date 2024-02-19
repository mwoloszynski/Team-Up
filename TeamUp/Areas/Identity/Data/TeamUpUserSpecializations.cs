using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpUserSpecializations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [PersonalData]
        [Column(TypeName = "int")]
        public int SpecId { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        [PersonalData]
        [Column(TypeName = "int")]
        public int Stars { get; set; }
    }
}
