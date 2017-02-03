//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYPNFCWineSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.ActiveWines = new HashSet<ActiveWine>();
        }
    
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> WID { get; set; }
        public Nullable<int> SupplyID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public string ProjectDescription { get; set; }
        public System.DateTime ProjectStartDate { get; set; }
        public Nullable<System.DateTime> ProjectEndDate { get; set; }
        public int ProjectStatusID { get; set; }
    
        public virtual ICollection<ActiveWine> ActiveWines { get; set; }
        public virtual Group Group { get; set; }
        public virtual ProjectStatu ProjectStatu { get; set; }
        public virtual SupplyChain SupplyChain { get; set; }
    }
}