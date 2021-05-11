using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Firelabel.Database
{
    public partial class OrderInvoiceRange
    {
        [Key]
        public int OrderInvoiceRangeId { get; set; }

        [StringLength(50)]
        public string SO { get; set; }

        public string BeginInvoice { get; set; }

        public string EndInvoice { get; set; }
    }
}