using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LGYTech_demo.Models
{
    public class StockMovements
    {

        //Id, ProductId, Change, Reason, CreatedAt -- Change örneği: -2; Reason örneği: “Sipariş #12” 
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Change {  get; set; }

        [Required, MaxLength(300)]
        public string Reason { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}