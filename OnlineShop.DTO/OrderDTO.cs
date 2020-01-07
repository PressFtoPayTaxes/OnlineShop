using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public string PaymentUrl { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderStatus { get; set; }
    }
}
