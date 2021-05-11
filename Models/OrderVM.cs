using Firelabel.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Firelabel.Models
{
    public class OrderVM
    {
        public List<Order> Orders { get; set; }
        public List<OrderInvoiceRange> OrderInvoiceRanges { get; set; }
        public string SO { get; set; }
        public string OrderInvoiceSONO { get; set; }
        public int Qty { get; set; }
        public string Product { get; set; }
        public string LabelType { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string StartOrderDate { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string EndOrderDate { get; set; }

        public int OrderInvoiceRangeId { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid Number")]
        public string Begin { get; set; }
        public string End { get; set; }
        public string SONumber { get; set; }
        public string hiddenType { get; set; }
        public string hiddenSearch { get; set; }
        public string LabelTypeId { get; set; }
        public int PageNo { get; set; }
    }
}