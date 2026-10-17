using System.ComponentModel.DataAnnotations;

namespace LGYTech_demo.ViewModels
{
    public class OrderItemViewModel
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a item.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Bigger than one.")]
        public int Quantity { get; set; }
    }
}