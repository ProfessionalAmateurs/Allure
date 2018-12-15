using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Estimates
    {
        public int QuestionID
        { get; set;}

        public string Question
        { get; set; }

        //public int UserID
        //{ get; set; }

        [Required(ErrorMessage = "Please enter your response")]
        public string Answer
        { get; set; }

        //public string estimateEmailBody
        //{ get; set; }

    }
}