﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FYPNFCWineSystemEntities : DbContext
    {
        public FYPNFCWineSystemEntities()
            : base("name=FYPNFCWineSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ActiveWine> ActiveWines { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<InactiveWine> InactiveWines { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectStatu> ProjectStatus { get; set; }
        public DbSet<Rejection> Rejections { get; set; }
        public DbSet<SupplyChain> SupplyChains { get; set; }
        public DbSet<TrustStatu> TrustStatus { get; set; }
        public DbSet<WineCategory> WineCategories { get; set; }
        public DbSet<WineStatu> WineStatus { get; set; }
        public DbSet<TagUpdateTransaction> TagUpdateTransaction { get; set; }
        public DbSet<TagValueAchieve> TagValueAchieve { get; set; }
    }
}
