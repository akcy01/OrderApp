using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiparisApps.Models.ViewModels
{
    public class CartVM
    {
        public OrderProduct OrderProduct { get; set; }

        /* Sepetteki bilgilere de ihtiyacımız var.Birçok ürün olabilir diye liste şeklinde alıyorum. */
        public IEnumerable<Cart> ListCart { get; set; } 

    }
}
