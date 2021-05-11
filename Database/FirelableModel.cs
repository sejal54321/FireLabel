namespace Firelabel.Database
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FirelabelModel : DbContext
    {
        public FirelabelModel()
            : base("name=FirelabelDataModel")
        {
        }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderInvoiceRange> OrderInvoiceRanges { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
