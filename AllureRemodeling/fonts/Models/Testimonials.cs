using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Testimonials
    {
        public virtual int TestimonialsID { get; set; }
        public virtual int UserID { get; set; }
        public virtual string Testimonial { get; set; }
        public virtual DateTime Date { get; set; }
    }
}