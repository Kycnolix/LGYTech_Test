using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Runtime.CompilerServices;

namespace LGYTech_demo.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        
        [Required, MaxLength(50), Index("IX_UniqueSKU", IsUnique = true)]
        public string Sku { get; set; }

        [Range(typeof(decimal),"0,01", "9999,00")]
        public decimal Price { get; set; }

        [Range(0,int.MaxValue)]
        public int Stock { get; set; }

        public virtual ICollection<OrderLines> OrderLines
        { get; set; } = new HashSet<OrderLines>();

        public virtual ICollection<StockMovements> StockMovements
        { get; set; } = new HashSet<StockMovements>();

    }
}