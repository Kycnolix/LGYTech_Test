using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http.Results;

namespace LGYTech_demo.Models
{
    public class Order
    {

        //Id, CustomerName, CreatedAt,Total

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string CustomerName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }

        public virtual ICollection<OrderLines> OrderLines
        { get; set; } = new HashSet<OrderLines>();

    }
}