using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Models
{
    public class PostModel
    {
        public TeamUpPost Post { get; set; }

        public List<TeamUpPostContent> PostContent { get; set; }

        public List<TeamUpPostImage> PostImage { get; set; }

        public List<TeamUpPostSource> PostSource { get; set; }

        public List<TeamUpPost> MorePosts { get; set; }
    }
}
