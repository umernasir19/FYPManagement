using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYPAUtOMATION.Models
{
    public class StudentGroupViewModel
    {

        public int std_Adv_req_id { get; set; }
        public int group_id { get; set; }
        [Required]
        public string Group_Name { get; set; }

        public string Student1_Name { get; set; }
        public string Student2_Name { get; set; }
        public string AdvisorName { get; set; }
    }
}