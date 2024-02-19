using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Models
{
    public class PostUserModel
    {
        public PostModel PostModel { get; set; }

        public int isAdmin { get; set; }
    }
}
