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
    
    public partial class Group
    {
        public Group()
        {
            this.Projects = new HashSet<Project>();
            this.SupplyChains = new HashSet<SupplyChain>();
        }
    
        public int GroupID { get; set; }
        public string GroupName { get; set; }
    
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<SupplyChain> SupplyChains { get; set; }
    }
}
