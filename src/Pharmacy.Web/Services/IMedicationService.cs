using Pharmacy.Web.Models;

namespace Pharmacy.Web.Services;

public interface IMedicationService
{
    Task<IEnumerable<Medication>> GetAllAsync();
    Task<IEnumerable<Sale>> GetSalesAsync();
    Task CreateAsync(object medication); 
    Task DeleteAsync(int id);

    Task UpdateAsync(int id, object medication);
    Task CreateSaleAsync(int medicationId, int quantity);
    Task<IEnumerable<Order>> GetMyOrdersAsync(string customerName);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task CreateOrderAsync(string customerName, int medicationId, int quantity);
    Task ApproveOrderAsync(int id);
    Task RejectOrderAsync(int id, string? comment);
}