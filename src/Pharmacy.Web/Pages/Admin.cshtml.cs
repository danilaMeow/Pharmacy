using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pharmacy.Web.Models;
using Pharmacy.Web.Services;

namespace Pharmacy.Web.Pages;

public class AdminModel : PageModel
{
    private readonly IMedicationService _service;

    public AdminModel(IMedicationService service)
    {
        _service = service;
    }

    [BindProperty]
    public MedicationInput Input { get; set; } = new();

    [BindProperty]
    public EditInput Edit { get; set; } = new();

    public string? Message { get; set; }
    public bool IsError { get; set; }
    public IEnumerable<Medication> Medications { get; set; } = new List<Medication>();
    public List<Order> PendingOrders { get; set; } = new();

    public async Task OnGetAsync() => await LoadData();

    public async Task<IActionResult> OnPostCreateAsync()
    {
        try
        {
            await _service.CreateAsync(new
            {
                Name = Input.Name,
                Category = Input.Category,
                Price = Input.Price,
                StockQuantity = Input.Quantity
            });
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            Message = "Ошибка: " + ex.Message;
            IsError = true;
            await LoadData();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        try
        {
            await _service.UpdateAsync(Edit.Id, new
            {
                Id = Edit.Id,
                Name = Edit.Name,
                Category = Edit.Category,
                Price = Edit.Price,
                StockQuantity = Edit.StockQuantity
            });
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            Message = "Ошибка обновления: " + ex.Message;
            IsError = true;
            await LoadData();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try { await _service.DeleteAsync(id); } catch { }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        try { await _service.ApproveOrderAsync(id); }
        catch (Exception ex)
        {
            Message = "Ошибка одобрения: " + ex.Message;
            IsError = true;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id)
    {
        try { await _service.RejectOrderAsync(id, "Отклонено администратором"); }
        catch (Exception ex)
        {
            Message = "Ошибка отклонения: " + ex.Message;
            IsError = true;
        }
        return RedirectToPage();
    }

    private async Task LoadData()
    {
        try
        {
            Medications = await _service.GetAllAsync();
            PendingOrders = (await _service.GetAllOrdersAsync())
                .Where(o => o.Status == OrderStatus.Pending)
                .ToList();
        }
        catch
        {
            Medications = new List<Medication>();
            PendingOrders = new List<Order>();
        }
    }
}

public class MedicationInput
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class EditInput
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}