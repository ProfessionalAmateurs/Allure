using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Testimonials
    {
        public  int TestimonialsID { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public  string Testimonial { get; set; }
        public  DateTime Date { get; set; }
    }
}