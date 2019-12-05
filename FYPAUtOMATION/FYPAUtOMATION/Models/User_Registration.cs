using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FYPAUtOMATION.Models
{
    public class User_Registration
    {
        public int Id { get; set; }
       
        public string User_Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }
        public Nullable<System.DateTime> Date_Created { get; set; }

        public int Registration_Type { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string university_Id { get; set; }
        [Required]
        public string Student_Email { get; set; }


        [NotMapped] // Does not effect with your database
        [Compare("User_Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        public string Status { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<bool> Is_Block { get; set; }
        public Nullable<bool> Is_Pending { get; set; }


    }
}