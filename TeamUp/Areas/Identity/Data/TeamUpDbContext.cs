using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Data
{
    public class TeamUpDbContext : IdentityDbContext<TeamUpUser>
    {
        public DbSet<TeamUpTeam> Teams { get; set; }
        public DbSet<TeamUpSpecialization> Specializations { get; set; }
        public DbSet<TeamUpTeamSpecializations> TeamSpecializations { get; set; }
        public DbSet<TeamUpTeamUsers> TeamUsers { get; set; }
        public DbSet<TeamUpUserSpecializations> UserSpecializations { get; set; }
        public DbSet<TeamUpTeamPosts> TeamPosts { get; set; }
        public DbSet<TeamUpTeamApplications> TeamApplications { get; set; }
        public DbSet<TeamUpTeamChat> TeamChat { get; set; }
        public DbSet<TeamUpPost> Post { get; set; }
        public DbSet<TeamUpPostContent> PostContent { get; set; }
        public DbSet<TeamUpPostImage> PostImages { get; set; }
        public DbSet<TeamUpPostSource> PostSource { get; set; }

        public TeamUpDbContext(DbContextOptions<TeamUpDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
