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
    
    public partial class Advisor
    {
        public int Id { get; set; }
        public string AdvisorsName { get; set; }
        public string Advisor_Description { get; set; }
        public string Advisor_Email { get; set; }
        public string Advisor_number { get; set; }
        public string Advisor_Uni_ID { get; set; }
        public Nullable<System.DateTime> Advisor_Requested_Date { get; set; }
        public Nullable<System.DateTime> Advisor_Accepted_Date { get; set; }
        public Nullable<bool> IsBlock { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Advisors_Slot { get; set; }
    }
}
