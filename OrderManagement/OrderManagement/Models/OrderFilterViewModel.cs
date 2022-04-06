using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace OrderManagement.Models
{
    public class OrderFilterViewModel
    {
        [Display(Name ="Store Name")]
        public string StoreName { get; set; }
        [Display(Name = "Manager Id")]
        public int ManagerOrderId { get; set; }
        [Display(Name = "Customer Id")]
        public int CustomerOrderId { get; set; }
        [Display(Name = "Seller Id")]
        public int SellerOrderId { get; set; }
        [Display(Name = "Item Id")]
        public int ItemOrderId { get; set; }

        public int CurrentPage { get; set; } = 1;
        public string SortDirection { get; set; } = "ASC";

        public IPagedList<Order> Orders;
    }
}
