using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Api.Data;
using Pharmacy.Api.Models;
using StackExchange.Redis;
using System.Text.Json;


using Order = Pharmacy.Api.Models.Order;
using OrderStatus = Pharmacy.Api.Models.OrderStatus;

namespace Pharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly IDatabase _cache;

	public OrdersController(AppDbContext context, IConnectionMultiplexer redis)
	{
		_context = context;
		_cache = redis.GetDatabase();
	}

	// Все заказы (для админа)
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Order>>> GetAll()
	{
		return await _context.Orders
			.Include(o => o.Medication)
			.OrderByDescending(o => o.CreatedAt)
			.ToListAsync();
	}

	// Заказы конкретного пользователя
	[HttpGet("my/{customerName}")]
	public async Task<ActionResult<IEnumerable<Order>>> GetMy(string customerName)
	{
		return await _context.Orders
			.Include(o => o.Medication)
			.Where(o => o.CustomerName == customerName)
			.OrderByDescending(o => o.CreatedAt)
			.ToListAsync();
	}

	// Создать заказ (пользователь добавляет в корзину)
	[HttpPost]
	public async Task<ActionResult<Order>> Create(Order order)
	{
		var med = await _context.Medications.FindAsync(order.MedicationId);
		if (med == null) return NotFound("Препарат не найден");
		if (med.StockQuantity < order.Quantity)
			return BadRequest("Недостаточно товара на складе");

		order.Status = OrderStatus.Pending;
		order.CreatedAt = DateTime.UtcNow;
		_context.Orders.Add(order);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
	}

	// Одобрить заказ (админ)
	[HttpPost("{id}/approve")]
	public async Task<IActionResult> Approve(int id)
	{
		var order = await _context.Orders
			.Include(o => o.Medication)
			.FirstOrDefaultAsync(o => o.Id == id);
		if (order == null) return NotFound();

		var med = await _context.Medications.FindAsync(order.MedicationId);
		if (med == null) return NotFound("Препарат не найден");
		if (med.StockQuantity < order.Quantity)
			return BadRequest("Недостаточно товара на складе");

		// Списываем остаток только при одобрении
		med.StockQuantity -= order.Quantity;
		order.Status = OrderStatus.Approved;
		await _context.SaveChangesAsync();

		await _cache.KeyDeleteAsync("medications_list");
		await _cache.KeyDeleteAsync($"medication_{order.MedicationId}");

		return Ok();
	}

	[HttpPost("{id}/reject")]
	public async Task<IActionResult> Reject(int id, [FromBody] string? comment)
	{
		var order = await _context.Orders.FindAsync(id);
		if (order == null) return NotFound();

		order.Status = OrderStatus.Rejected;
		order.AdminComment = comment;
		await _context.SaveChangesAsync();
		return Ok();
	}
}