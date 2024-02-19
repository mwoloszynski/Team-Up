using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Models.EmailConfig
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
