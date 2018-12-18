using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Materials
    {
        public virtual int MaterialID { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Price { get; set; } 
        public string Vendor { get; set; }
    }
}