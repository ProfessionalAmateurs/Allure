using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class Estimates
    {
        public int QuestionID
        { get; set; }

        public string Question
        { get; set; }

        public int UserID
        { get; set; }

        public string Answer
        { get; set; }
    }
}