namespace Firelabel.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [Key]
        [StringLength(50)]
        public string SO { get; set; }

        [StringLength(50)]
        public string Product { get; set; }

        public int? Qty { get; set; }

        [StringLength(100)]
        public string LabelType { get; set; }

        [StringLength(50)]
        public string OrderStatus { get; set; }

        public int? OrderNumber { get; set; }

        [StringLength(50)]
        public string OrderSuffix { get; set; }

        public DateTime OrderDate { get; set; }

        //public int? BeginInvoice { get; set; }

        //public int? EndInvoice { get; set; }
    }
}
