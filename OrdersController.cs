// Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using TubeShopBackend.Models;

namespace TubeShopBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static List<OrderRequest> _orders = new List<OrderRequest>();
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequest order)
        {
            try
            {
                // ���� ��� �� ������ 3 �����...
                if (order == null || order.Items == null || !order.Items.Any())
                {
                    return BadRequest(new { error = "����� �� ����� ���� ������" });
                }

                // ��������� ������ ���������...

                return Ok(new
                {
                    success = true,
                    orderId = order.OrderId,
                    message = "����� ������� ��������!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� �������� ������");
                return StatusCode(500, new { error = "��������� ������ ��� ��������� ������" });
            }
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok(_orders);
        }

        private bool IsValidInn(string inn)
        {
            if (string.IsNullOrWhiteSpace(inn)) return false;
            return (inn.Length == 10 || inn.Length == 12) && inn.All(char.IsDigit);
        }
    }
}