//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYPAUtOMATION
{
    using System;
    using System.Collections.Generic;
    
    public partial class Grade
    {
        public int Id { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string FirstHalf_marks { get; set; }
        public string SecondHalf_marks { get; set; }
        public string ThirdHalf_marks { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<System.DateTime> Date_Created { get; set; }
    }
}