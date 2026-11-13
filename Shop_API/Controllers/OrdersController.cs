using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_API.Dto;
using Shop_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Shop_API.Controllers
{
    // https://localhost:7487/orders
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // 1. Определяем контекст данных
        private readonly ShopDbContext _context;

        // 7.1. Создаём словарь сопоставлений (для статуса)
        private readonly Dictionary<string, string> statusMap = new Dictionary<string, string>()
        {
            ["new"] = "Новый",
            ["paid"] = "Оплачен",
            ["shipped"] = "Отправлен",
            ["done"] = "Завершён",
            ["cancelled"] = "Отменён"
        };

        // 2. Явно указываем конструктор класса OrdersController
        public OrdersController(ShopDbContext context)
        {
            // 3. Инициализируем контекст данных (присваиваем значение в контекст данных)
            _context = context;
        }

        // 4. Создаём эндпоинт (метод для выполнения действия над API) для получения данных
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string? status,
                                                   [FromQuery] string? userId)
        {
            // 5. Создаём объект для передачи данных (DTO):
            // 5.1. Создаём в проекте папку Dto
            // 5.2. Создаём в папке класс OrderDto

            // 7. Подготавливаем ответ
            try
            {
                // Исходный список (копия оригинального списка из таблицы Orders)
                List<Order> orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .OrderBy(o => o.Id) // заказы возвращаются в порядке возрастания номера заказа
                .ToList();

                // Обработка статуса 400 (указан несуществующий код состояния)
                if (!string.IsNullOrEmpty(status) && !statusMap.ContainsKey(status))
                {
                    return BadRequest(new { error = "Недопустимое значение параметра status" });
                }

                // 
                if (!string.IsNullOrWhiteSpace(status))
                {
                    orders = orders.Where(o => o.Status == status).ToList();
                }

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    if (int.TryParse(userId, out int id))
                    {
                        orders = orders.Where(o => o.UserId == id).ToList();
                    }
                    else
                    {
                        return BadRequest(new { error = "Недопустимое значение параметра user_id" });
                    }
                }

                List<OrderDto> ordersDto = orders.Select(o => new OrderDto()
                {
                    OrderId = o.Id,
                    Customer = $"{o.User.LastName} {o.User.FirstName} {o.User.MiddleName}".TrimEnd(),
                    Status = statusMap[o.Status],
                    CreatedAt = DateTime.Parse(o.CreatedAt).ToString("dd.MM.yyyy"),
                    Address = FormatAddress(o.User),
                    ItemsCount = o.OrderItems.Count,
                    Total = o.OrderItems.Sum(i => i.Quantity * i.Product.Price).ToString("F2", CultureInfo.InvariantCulture),
                    Items = o.OrderItems.Select(i => new OrderItemDto()
                    {
                        Title = i.Product.Title,
                        Quantity = i.Quantity,
                        Price = i.Product.Price.ToString("F2", CultureInfo.InvariantCulture) // CultureInfo.InvariantCulture делается для замены "," на "." т.е., 2770,00 => 2770.00
                    }).ToList()
                })
                .ToList();

                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка приложения: недоступна база данных" });
            }
        }
        private string FormatAddress(User user)
        {
            string address = $"г. {user.City}, ул. {user.Street}, д. {user.House}";

            if (!string.IsNullOrEmpty(user.Flat))
            {
                address += $", кв. {user.Flat}";
            }

            return address;
        }
    }
}
