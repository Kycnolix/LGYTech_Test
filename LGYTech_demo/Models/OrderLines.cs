using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGYTech_demo.Models
{
    public class OrderLines
    {

        //Id, OrderId, ProductId, Quantity, UnitPrice -- UnitPrice, sipariş anındaki fiyat 

        public int Id { get; set; }
        public int OrderId  { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}