using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamUp.Areas.Identity.Data;

namespace TeamUp.Models
{
    public class TeamUserModel
    {
        public TeamUpTeam Team { get; set; }

        public TeamUpUser Admin { get; set; }

        public List<TeamUpUser> Users { get; set; }

        public List<TeamUpTeamSpecializations> Slots { get; set; }

        public List<TeamUpTeamUsers> TeamUsers { get; set; }

        public List<TeamUpSpecialization> Specializations { get; set; }
    }
}
