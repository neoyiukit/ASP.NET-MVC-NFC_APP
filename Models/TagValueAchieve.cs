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
    
    public partial class TagValueAchieve
    {
        public int Id { get; set; }
        public string tag_value { get; set; }
        public int wine_id { get; set; }
    
        public virtual ActiveWine ActiveWine { get; set; }
    }
}
