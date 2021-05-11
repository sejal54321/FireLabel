using Firelabel.Database;
using Firelabel.Models;
using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Firelabel.Controllers
{
    public class HomeController : Controller
    {
        FirelabelModel FirelabelModel = new FirelabelModel();

        [HttpGet]
        public ActionResult Index()
        {
            OrderVM orderVM = new OrderVM();
            if (TempData["OrderVM"] != null)
            {
                orderVM = TempData["OrderVM"] as OrderVM;
            }

            var result = (from f in FirelabelModel.Orders
                          select new
                          {
                              LabelType = f.LabelType

                          }).Distinct().ToList();

            ViewBag.LabelTypelist = new SelectList(result.ToList(), "LabelType", "LabelType");
            return View(orderVM);
        }


        [HttpPost]
        public ActionResult Index(OrderVM orderVM, string command)
        {
            if (command.ToUpper().Contains("SEARCH"))
            {
                orderVM = SearchData(orderVM);

                return View(orderVM);
            }
            if (command.ToUpper().Contains("EXPORT"))
            {
                orderVM = ExportData(orderVM);

                string html = GenerateHTML(orderVM);
                byte[] pdf = GenerateRuntimePDF(html);

                Response.Clear();
                MemoryStream ms = new MemoryStream(pdf);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Orders.pdf");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
            }

            return View(orderVM);
        }

        public OrderVM SearchData(OrderVM orderVM)
        {
            List<Order> orders = FirelabelModel.Orders.ToList();
            List<OrderInvoiceRange> orderInvoiceRanges = FirelabelModel.OrderInvoiceRanges.ToList();

            if (!string.IsNullOrEmpty(orderVM.SO))
            {
                orderVM.SO = orderVM.SO;
                orders = orders.Where(p => p.SO.Contains(orderVM.SO)).ToList();
            }
            if (orderVM.Qty > 0)
            {
                orderVM.Qty = orderVM.Qty;
                orders = orders.Where(p => p.Qty == orderVM.Qty).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.Product))
            {
                orderVM.Product = orderVM.Product;
                orders = orders.Where(p => p.Product.Contains(orderVM.Product.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.LabelType))
            {
                orderVM.LabelType = orderVM.LabelType;
                orders = orders.Where(p => p.LabelType.Contains(orderVM.LabelType.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.LabelTypeId))
            {
                orderVM.LabelTypeId = orderVM.LabelTypeId;
                orderVM.LabelType = orderVM.LabelTypeId;
                orders = orders.Where(p => p.LabelType.Contains(orderVM.LabelType.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.StartOrderDate))
            {
                orderVM.StartOrderDate = orderVM.StartOrderDate;
                string[] d = orderVM.StartOrderDate.Split('/');
                DateTime date = new DateTime(int.Parse(d[2]), int.Parse(d[0]), int.Parse(d[1]));
                orders = orders.Where(p => p.OrderDate >= date).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.EndOrderDate))
            {
                orderVM.EndOrderDate = orderVM.EndOrderDate;
                string[] d = orderVM.EndOrderDate.Split('/');
                DateTime date = new DateTime(int.Parse(d[2]), int.Parse(d[0]), int.Parse(d[1]));
                orders = orders.Where(p => p.OrderDate <= date).ToList();
            }
            orderVM.Orders = orders;
            orderVM.OrderInvoiceRanges = orderInvoiceRanges;


            var result = (from f in FirelabelModel.Orders
                          select new
                          {
                              LabelType = f.LabelType

                          }).Distinct().ToList();

            ViewBag.LabelTypelist = new SelectList(result.ToList(), "LabelType", "LabelType");

            return orderVM;
        }

        public OrderVM ExportData(OrderVM orderVM)
        {
            List<Order> orders = FirelabelModel.Orders.ToList();
            List<OrderInvoiceRange> orderInvoiceRanges = FirelabelModel.OrderInvoiceRanges.ToList();
            List<string> orderIds = FirelabelModel.OrderInvoiceRanges.Select(p => p.SO).ToList();

            if (!string.IsNullOrEmpty(orderVM.StartOrderDate))
            {
                string[] d = orderVM.StartOrderDate.Split('/');
                DateTime date = new DateTime(int.Parse(d[2]), int.Parse(d[0]), int.Parse(d[1]));
                orders = orders.Where(p => p.OrderDate >= date).ToList();
            }
            if (!string.IsNullOrEmpty(orderVM.EndOrderDate))
            {
                string[] d = orderVM.EndOrderDate.Split('/');
                DateTime date = new DateTime(int.Parse(d[2]), int.Parse(d[0]), int.Parse(d[1]));
                orders = orders.Where(p => p.OrderDate <= date).ToList();
            }
            orderVM.Orders = orders.Where(p => orderIds.Contains(p.SO)).ToList();
            orderVM.OrderInvoiceRanges = orderInvoiceRanges;

            return orderVM;
        }
        [HttpGet]
        public ActionResult EditLableNumber(string id)
        {
            OrderVM orderVM = new OrderVM();
            List<OrderInvoiceRange> orderInvoiceRanges = FirelabelModel.OrderInvoiceRanges.Where(x => x.SO == id).ToList();
            orderVM.OrderInvoiceRanges = orderInvoiceRanges;
            orderVM.OrderInvoiceSONO = id;
            return PartialView("~/Views/Home/EditLableNumber.cshtml", orderVM);
        }
        [HttpPost]
        public ActionResult EditLableNumber(OrderVM obj)
        {
            if(Request.Params["hiddenSearch"] != null)
            {
                obj.hiddenSearch = Request.Params["hiddenSearch"];
            }
            if (Request.Params["hiddenType"] != null)
            {
                obj.hiddenType = Request.Params["hiddenType"];
            }
            if (obj != null)
            {
                if (obj.OrderInvoiceRanges != null)
                {
                    var result = FirelabelModel.OrderInvoiceRanges.Where(p => p.SO == obj.OrderInvoiceSONO).ToList();
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            FirelabelModel.OrderInvoiceRanges.Remove(item);
                            FirelabelModel.SaveChanges();

                        }
                    }
                    foreach (var item in obj.OrderInvoiceRanges)
                    {
                        OrderInvoiceRange Inv = new OrderInvoiceRange();
                        Inv.SO = obj.OrderInvoiceSONO;
                        Inv.BeginInvoice = item.BeginInvoice;
                        Inv.EndInvoice = item.EndInvoice;

                        FirelabelModel.OrderInvoiceRanges.Add(Inv);
                        FirelabelModel.SaveChanges();


                    }

                }
            }
            obj = SearchData(obj);
            TempData["OrderVM"] = null;
            TempData["OrderVM"] = obj;
            //OrderInvoiceRange orderInvoiceRange = new OrderInvoiceRange();
            //if (orderVM.OrderInvoiceRangeId != 0)
            //{
            //    orderInvoiceRange = FirelabelModel.OrderInvoiceRanges.Where(p => p.OrderInvoiceRangeId == orderVM.OrderInvoiceRangeId).FirstOrDefault();
            //    if (orderInvoiceRange != null)
            //    {
            //        orderInvoiceRange.BeginInvoice = orderVM.Begin;
            //        orderInvoiceRange.EndInvoice = orderVM.End;
            //        FirelabelModel.SaveChanges();
            //    }
            //}
            //orderVM = SearchData(orderVM);

            //TempData["OrderVM"] = orderVM;

            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult AddLableNumber(string[] Begin, string[] End, string addSoNumber, OrderVM orderVM)
        {
            if (Request.Params["hiddenSearch"] != null)
            {
                orderVM.hiddenSearch = Request.Params["hiddenSearch"];
            }
            if (Request.Params["hiddenType"] != null)
            {
                orderVM.hiddenType = Request.Params["hiddenType"];
            }
            for (int i = 0; i < Begin.Length; i++)
            {
                OrderInvoiceRange orderInvoiceRange = new OrderInvoiceRange();
                orderInvoiceRange.SO = addSoNumber;
                orderInvoiceRange.BeginInvoice = Begin[i];
                orderInvoiceRange.EndInvoice = End[i];
                FirelabelModel.OrderInvoiceRanges.Add(orderInvoiceRange);
                FirelabelModel.SaveChanges();
            }
            orderVM = SearchData(orderVM);
            TempData["OrderVM"] = null;
            TempData["OrderVM"] = orderVM;

            return RedirectToAction("Index");
        }


        public ActionResult DeleteRange(int id,string SO,string Prdt,int Qty,string Ltype,string Sdate,string Edate,string Search,string LableType, int PageNo = 0)
        {
                OrderVM obj = new OrderVM();
                obj.SO = SO;
                obj.Product = Prdt;
                obj.Qty = Qty;
                obj.LabelType = Ltype;
                obj.StartOrderDate = Sdate;
                obj.EndOrderDate = Edate;
                obj.hiddenSearch = Search;
                obj.hiddenType = LableType;
                obj.PageNo = PageNo;
            var Result = FirelabelModel.OrderInvoiceRanges.Find(id);
                if (Result != null)
                {
                    FirelabelModel.OrderInvoiceRanges.Remove(Result);
                    FirelabelModel.SaveChanges();              
                }          
         
            obj = SearchData(obj);
            TempData["OrderVM"] = null;
            TempData["OrderVM"] = obj;

            return RedirectToAction("Index");
        }

        public string GenerateHTML(OrderVM orderVM)
        {

            string html = "";
            string head = @"<html>
<head>
	<title></title>

<style>
	.table_wrap { font-size: 14px; line-height: 20px; }
	.table_wrap label { background-color: #87ceeb; padding: 7px 10px; width: 17.85%; display: inline-block; font-weight: 700; margin-right: 10px; }
	.table_wrap .table-box { width: 100%; padding-top: 30px; }
	.table_wrap table { width: 100%; border-collapse:collapse; }
	.table_wrap table th { background-color: #d0cdcd; color: #000; font-weight: 700; text-transform: capitalize; padding: 10px 10px; border: 0; }
    .table_wrap table th:not(:last-child) {border-right: 1px solid #d0cdcd;}
	.table_wrap table tr td { border: 1px solid #d0cdcd;margin: 0; padding: 6px 10px; text-align: center; }
</style>
</head>
<body>";
            string footer = "</div></body></html>";

            string DateLable = @"<div class='table_wrap'><label>SO Date Range</label>" + orderVM.StartOrderDate + " To " + orderVM.EndOrderDate + "";
            var labelgroup = orderVM.Orders.GroupBy(x => x.LabelType).ToList();

            string thtag = "<table><th>Sales Order</th><th>SO Date</th><th>Product</th><th>Qty</th><th>Begin Label</th><th>End Label</th>";
            string Mainbody = "";

            foreach (var item in labelgroup)
            {
                string LableType = "";
                string tablebody = "";
                var LableObj = item.FirstOrDefault();
                LableType += "<div class='table-box'><label>Label Type</label>  " + LableObj.LabelType + "";
                LableType += thtag;
                var GroupWiseRecords = orderVM.Orders.Where(x => x.LabelType.Contains(LableObj.LabelType)).ToList();
                foreach (var Rec in GroupWiseRecords)
                {
                    var childlable = orderVM.OrderInvoiceRanges.Where(x => x.SO == Rec.SO).ToList();
                    if (childlable.Count > 0)
                    {
                        foreach (var child in childlable)
                        {
                            tablebody += "<tr><td>" + Rec.SO + "</td><td>" + Rec.OrderDate.Date.ToString("MM/dd/yyyy") + "</td><td>" + Rec.Product + "</td><td>" + Rec.Qty + "</td><td>" + child.BeginInvoice + "</td><td>" + child.EndInvoice + "</td></tr>";
                        }
                    }
                    else
                    {
                        tablebody += "<tr><td>" + Rec.SO + "</td><td>" + Rec.OrderDate.Date.ToString("MM/dd/yyyy") + "</td><td>" + Rec.Product + "</td><td>" + Rec.Qty + "</td><td></td><td></td></tr>";
                    }
                }
                Mainbody += LableType + tablebody + "</table>";
            }
            html = head + DateLable + Mainbody + footer;
            return html;
        }

        public static byte[] GenerateRuntimePDF(string html)
        {
            HtmlToPdfConverter nRecohtmltoPdfObj = new HtmlToPdfConverter();
            return nRecohtmltoPdfObj.GeneratePdf(html);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}