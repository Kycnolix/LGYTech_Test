using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LGYTech_demo.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [MaxLength(100)]
        public string CustomerName { get; set; }

       
        public List<OrderItemViewModel> Items { get; set; }
            = new List<OrderItemViewModel>();
    }
}