using System.ComponentModel.DataAnnotations;

namespace LGYTech_demo.ViewModels
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Required a name.")]
        [MaxLength(50)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Can't be negative.")]
        public int Stock { get; set; }
    }
}