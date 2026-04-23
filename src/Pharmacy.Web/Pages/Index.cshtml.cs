using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pharmacy.Web.Models;
using Pharmacy.Web.Services;

namespace Pharmacy.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IMedicationService _apiService;

    public List<Medication> Medications { get; set; } = new();
    public List<Order> MyOrders { get; set; } = new();

    [BindProperty]
    public CartFormModel CartForm { get; set; } = new();

    public string Message { get; set; } = string.Empty;
    public bool IsError { get; set; }

    public IndexModel(IMedicationService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Medications = (await _apiService.GetAllAsync()).ToList();
            var name = User.Identity?.Name ?? "guest";
            MyOrders = (await _apiService.GetMyOrdersAsync(name)).ToList();
        }
        catch
        {
            IsError = true;
            Message = "Не удалось загрузить данные.";
        }
    }

    public async Task<IActionResult> OnPostAddToCartAsync()
    {
        try
        {
            var name = User.Identity?.Name ?? "guest";
            await _apiService.CreateOrderAsync(name, CartForm.MedicationId, CartForm.Quantity);
        }
        catch (Exception ex)
        {
            Message = "Ошибка: " + ex.Message;
        }
        return RedirectToPage();
    }
}

public class CartFormModel
{
    public int MedicationId { get; set; }
    public int Quantity { get; set; }
}