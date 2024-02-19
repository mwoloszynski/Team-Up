using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpPostSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "int")]
        public int PostId { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Source { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string ShortLink { get; set; }
    }
}
