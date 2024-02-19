using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpPostContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "int")]
        public int PostId { get; set; }

        [Column(TypeName = "text")]
        public string Subtitle { get; set; }

        [Column(TypeName = "text")]
        public string Content { get; set; }
    }
}
