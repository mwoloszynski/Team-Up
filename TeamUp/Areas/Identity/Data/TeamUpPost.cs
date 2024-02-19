using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Identity.Data
{
    public class TeamUpPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Header { get; set; }

        [Column(TypeName = "text")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Author { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Category { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string MainImage { get; set; }

    }
}
