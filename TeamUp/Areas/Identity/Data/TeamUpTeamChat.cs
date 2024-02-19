using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpTeamChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        [PersonalData]
        [Column(TypeName = "int")]
        public int TeamId { get; set; }

        [PersonalData]
        [Column(TypeName = "datetime")]
        public DateTime SendDate { get; set; }

        [PersonalData]
        [Column(TypeName = "text")]
        public string Message { get; set; }

    }
}
