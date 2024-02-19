using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Models
{
    public class BlogUserModel
    {
        public TeamUpUser User { get; set; }

        public List<TeamUpPost> Posts { get; set; }

        public int currentPage { get; set; }

        public int numberOfPages { get; set; }
    }
}
