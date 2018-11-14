using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Jobs
    {
        public virtual int JobID { get; set; }
        public virtual int UserID { get; set; }
        public virtual string JobDescription { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual decimal Estimate { get; set; }
        public virtual string City { get; set; }
        public virtual string Status { get; set; }
    }
}