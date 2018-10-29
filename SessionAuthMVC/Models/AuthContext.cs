namespace SessionAuthMVC.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuthContext : DbContext
    {
        public AuthContext()
            : base("name=cstrAuth")
        {}
        public DbSet<UsersModel> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
