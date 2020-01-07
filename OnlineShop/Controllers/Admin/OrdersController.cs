using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using OnlineShop.Domain;
using OnlineShop.DTO;

namespace OnlineShop.Web.Controllers.Admin
{
    [Route("admin/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineShopContext context;

        public OrdersController(OnlineShopContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await context.Orders.Where(order => order.DeletedDate == null).ToListAsync();
            return Ok(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await context.Orders.SingleOrDefaultAsync(ord => ord.Id == id);
            return Ok(order);
        }

        [HttpPut]
        public async Task<IActionResult> EditOrder(OrderDTO orderDTO)
        {
            var order = await context.Orders.SingleOrDefaultAsync(ord => ord.Id == orderDTO.Id);

            if (orderDTO.PaymentUrl != null && orderDTO.PaymentUrl != "")
                order.PaymentUrl = orderDTO.PaymentUrl;
            if (orderDTO.DeliveryAddress != null && orderDTO.DeliveryAddress != "")
                order.DeliveryAddress = orderDTO.DeliveryAddress;
            if (orderDTO.OrderStatus != null && orderDTO.OrderStatus != "")
                order.OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), orderDTO.OrderStatus);

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await context.Orders.SingleOrDefaultAsync(ord => ord.Id == id);
            order.DeletedDate = DateTime.Now;
            return Ok();
        }
    }
}