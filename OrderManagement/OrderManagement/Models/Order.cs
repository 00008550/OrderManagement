using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagement.Models
{
    public class Order
    {
        [DisplayName("Id")]
        public int? OrderId { get; set; }
        [DisplayName("Store Name")]
        public string StoreName { get; set; }
        [DisplayName("Manager Id")]
        public int? ManagerOrderId { get; set; }


        [DisplayName("Customer Id")]
        public int? CustomerOrderId { get; set; }


        [DisplayName("Seller Id")]
        public int? SellerOrderId { get; set; }


        [DisplayName("Item Id")]
        public int? ItemOrderId { get; set; }


        [DisplayName("Price")]
        public decimal Price { get; set; }
        [DisplayName("Date of Order"), DataType(DataType.Date)]
        public DateTime DateOfOrder { get; set; }
        [DisplayName("Image Url")]
        public string ImageUrl { get; set; }
        [DisplayName("Item Quantity")]
        public int ItemQuantity { get; set; }
        [DisplayName("Item Quality")]
        public string ItemQuality { get; set; }
        public bool Delivered { get; set; }

        
    }
}
