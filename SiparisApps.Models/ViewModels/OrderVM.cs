using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiparisApps.Models.ViewModels
{
    public class OrderVM
    {
        public OrderProduct OrderProduct { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; } //Bize liste şeklinde gelicek siparişler.
    }
}
