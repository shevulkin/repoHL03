using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers
{
    [ApiController]           // Автоматизує валідацію DTO і повертає 400 Bad Request при помилках
    [Route("api/[controller]")] // Attribute Routing -> доступ за адресою api/orders
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        // Впровадження залежності (DI) - ЖОДНИХ "new OrderService()"
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id:int}")] // Обмеження типу: id має бути числом
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Замовлення не знайдено." });
            }
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderDto orderDto)
        {
            // Якщо ProductName буде порожнім або Quantity < 1, [ApiController] сам поверне 400
            var createdOrder = await _orderService.CreateOrderAsync(orderDto);

            // Повертає статус 201 Created і посилання на створений ресурс
            return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
        }

        // PUT: api/orders/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderDto orderDto)
        {
            var isUpdated = await _orderService.UpdateOrderAsync(id, orderDto);
            if (!isUpdated)
            {
                return NotFound(new { message = "Замовлення для оновлення не знайдено." });
            }
            return NoContent(); // Стандартна відповідь 204 при успішному PUT
        }

        // DELETE: api/orders/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _orderService.DeleteOrderAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = "Замовлення для видалення не знайдено." });
            }
            return NoContent(); // Стандартна відповідь 204 при успішному DELETE
        }
    }
}