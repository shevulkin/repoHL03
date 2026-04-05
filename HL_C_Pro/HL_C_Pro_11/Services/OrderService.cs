using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private static readonly List<OrderDto> _orders = new();
        private static int _nextId = 1;

        // РЕАЛІЗАЦІЯ НОВОГО МЕТОДУ (Повертає всю "базу даних")
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            await Task.Delay(10);
            return _orders;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            await Task.Delay(10);
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            await Task.Delay(10);
            orderDto.Id = _nextId++;
            _orders.Add(orderDto);
            return orderDto;
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderDto orderDto)
        {
            await Task.Delay(10);
            var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder == null) return false;

            existingOrder.ProductName = orderDto.ProductName;
            existingOrder.Quantity = orderDto.Quantity;
            existingOrder.Price = orderDto.Price;

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            await Task.Delay(10);
            var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder == null) return false;

            _orders.Remove(existingOrder);
            return true;
        }
    }
}