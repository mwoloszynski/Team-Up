using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpTeamSpecializations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [PersonalData]
        [Column(TypeName = "int")]
        public int TeamId { get; set; }

        [PersonalData]
        [Column(TypeName = "int")]
        public int SpecId { get; set; }

        [PersonalData]
        [Column(TypeName = "bit")]
        public bool IsAvaible { get; set; }
    }
}
