using OrderManagementSystem.Services;
using OrderManagementSystem.Models;


/*
 * ДЗ 10. Система управління замовленнями
 Розробіть асинхронний RESTful API для системи управління замовленнями (Order Management System),
використовуючи сучасний стиль Top-level statements у Program.cs
для розділення фаз налаштування сервісів (Builder) та конвеєра обробки (App) .
Створіть контролер OrdersController, що наслідується від ControllerBase з атрибутом [ApiController],
який автоматично валідує вхідні DTO-моделі на основі Data Annotations (таких як [Required]
та [StringLength]) і повертає 400 Bad Request у разі помилок .
Реалізуйте бізнес-логіку через окремий сервіс IOrderService,
зареєстрований у DI-контейнері з життєвим циклом Scoped (один об'єкт на HTTP-запит),
та впровадьте його через конструктор контролера, уникаючи прямого створення об'єктів через new .
Забезпечте підтримку стандартних HTTP-методів (GET, POST, PUT, DELETE)
за допомогою Attribute Routing із використанням обмежень типів (наприклад, {id:int}),
 * 
 */


var builder = WebApplication.CreateBuilder(args);

// --- ФАЗА 1: Налаштування сервісів (Builder) ---
builder.Services.AddControllers();

// Налаштування Swagger
builder.Services.AddEndpointsApiExplorer(); // Обов'язково: допомагає Swagger знайти маршрути
builder.Services.AddSwaggerGen();           // Генерує документацію

// Реєстрація сервісу (Scoped)
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// --- ФАЗА 2: Конвеєр обробки (App) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Вмикає візуальний інтерфейс
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
//For testing, run the application and navigate to the Swagger UI at:
//https://localhost:7164/swagger