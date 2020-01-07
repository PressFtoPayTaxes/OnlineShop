using System;
using System.Collections.Generic;

namespace OnlineShop.Domain
{
    public class Cart : Entity
    {
        public virtual ICollection<ProductInCart> ProductsInCart { get; set; } = new List<ProductInCart>();
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}