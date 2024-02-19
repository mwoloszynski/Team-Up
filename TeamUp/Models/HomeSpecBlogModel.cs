using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Models
{
    public class HomeSpecBlogModel
    {
        public TeamUpPost Post { get; set; }

        public List<TeamUpSpecialization> Specs { get; set; }
    }
}
