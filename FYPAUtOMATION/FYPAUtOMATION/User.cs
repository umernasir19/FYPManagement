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
    
    public partial class User
    {
        public int Id { get; set; }
        public string User_Name { get; set; }
        public string User_Password { get; set; }
        public Nullable<System.DateTime> Date_Created { get; set; }
        public string Status { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<bool> Is_Block { get; set; }
        public Nullable<bool> Is_Pending { get; set; }
        public Nullable<int> Created_By { get; set; }
    }
}
