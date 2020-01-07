using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Controllers.Admin
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Guid> Products { get; set; }
        public string ImageUrl { get; set; }
    }
}
