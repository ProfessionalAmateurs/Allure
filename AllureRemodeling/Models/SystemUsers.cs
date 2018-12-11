using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class SystemUsers
    {
        public int SystemUserID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLogin { get; set; }

        public int NumberOfLogins { get; set; }

        public string GuidResetPassword{ get; set; }
    }
}


