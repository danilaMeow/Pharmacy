using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Api.Data;
using Pharmacy.Api.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Pharmacy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDatabase _cache;
    private const string CacheKey = "medications_list";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public MedicationsController(AppDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _cache = redis.GetDatabase();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Medication>>> GetMedications()
    {
        var cached = await _cache.StringGetAsync(CacheKey);
        if (cached.HasValue)
            return JsonSerializer.Deserialize<List<Medication>>(cached!)!;

        var medications = await _context.Medications
            .OrderBy(m => m.Id)
            .ToListAsync();

        await _cache.StringSetAsync(CacheKey,
            JsonSerializer.Serialize(medications), CacheTtl);
        return medications;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Medication>> GetById(int id)
    {
        var key = $"medication_{id}";
        var cached = await _cache.StringGetAsync(key);
        if (cached.HasValue)
            return JsonSerializer.Deserialize<Medication>(cached!)!;

        var med = await _context.Medications.FindAsync(id);
        if (med == null) return NotFound();

        await _cache.StringSetAsync(key, JsonSerializer.Serialize(med), CacheTtl);
        return med;
    }

    [HttpPost]
    public async Task<ActionResult<Medication>> Create(Medication med)
    {
        _context.Medications.Add(med);
        await _context.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey); // инвалидация
        return CreatedAtAction(nameof(GetById), new { id = med.Id }, med);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Medication med)
    {
        if (id != med.Id) return BadRequest();
        _context.Entry(med).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey);
        await _cache.KeyDeleteAsync($"medication_{id}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var med = await _context.Medications.FindAsync(id);
        if (med == null) return NotFound();
        _context.Medications.Remove(med);
        await _context.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey);
        await _cache.KeyDeleteAsync($"medication_{id}");
        return NoContent();
    }
}