using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.Results;
using System.ComponentModel.DataAnnotations.Schema;

namespace LGYTech_demo.Models
{
    public class Order
    {

        //Id, CustomerName, CreatedAt,Total

        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string CustomerName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }


    
    }
}