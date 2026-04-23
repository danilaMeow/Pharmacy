using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Api.Data;
using Pharmacy.Api.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Pharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDatabase _cache;

    public SalesController(AppDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _cache = redis.GetDatabase();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetAll()
    {
        return await _context.Sales
            .Include(s => s.Medication)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetById(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Medication)
            .FirstOrDefaultAsync(s => s.Id == id);
        return sale == null ? NotFound() : sale;
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Create(Sale sale)
    {
        var med = await _context.Medications.FindAsync(sale.MedicationId);
        if (med == null) return NotFound("Препарат не найден");
        if (med.StockQuantity < sale.Quantity)
            return BadRequest("Недостаточно товара на складе");

        med.StockQuantity -= sale.Quantity;
        sale.SaleDate = DateTime.UtcNow;
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // Инвалидируем кэш препаратов — остаток изменился
        await _cache.KeyDeleteAsync("medications_list");
        await _cache.KeyDeleteAsync($"medication_{sale.MedicationId}");

        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return NotFound();
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}