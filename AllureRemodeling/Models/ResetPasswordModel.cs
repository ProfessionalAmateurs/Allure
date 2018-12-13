using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{

        public class ResetPasswordModel
        {
            [Required(AllowEmptyStrings = false, ErrorMessage = "New Password Required")]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string newPassword { get; set; }

            [Display(Name = "Confirm Password")]
            [DataType(DataType.Password)]
            [Compare("newPassword", ErrorMessage = "Passwords must match")]
            public string confirmPassword { get; set; }

            [Required]
            public string ResetCode { get; set; }
        }
    
}