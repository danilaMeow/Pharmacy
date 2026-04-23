using System.Net.Http.Json;
using Pharmacy.Web.Models;

namespace Pharmacy.Web.Services;

public class PharmacyApiService : IMedicationService
{
    private readonly HttpClient _httpClient;

    public PharmacyApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Medication>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Medication>>("api/Medications")
               ?? new List<Medication>();
    }

    public async Task<IEnumerable<Sale>> GetSalesAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Sale>>("api/Sales")
               ?? new List<Sale>();
    }

    public async Task CreateAsync(object medication)
    {
        await _httpClient.PostAsJsonAsync("api/Medications", medication);
    }

    public async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/Medications/{id}");
    }

    public async Task CreateSaleAsync(int medicationId, int quantity)
    {
        var saleDto = new { MedicationId = medicationId, Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync("api/Sales", saleDto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(int id, object medication)
    {
        await _httpClient.PutAsJsonAsync($"api/Medications/{id}", medication);
    }
    public async Task<IEnumerable<Order>> GetMyOrdersAsync(string customerName)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Order>>
            ($"api/Orders/my/{customerName}") ?? new List<Order>();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Order>>
            ("api/Orders") ?? new List<Order>();
    }

    public async Task CreateOrderAsync(string customerName, int medicationId, int quantity)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Orders", new
        {
            CustomerName = customerName,
            MedicationId = medicationId,
            Quantity = quantity
        });
        response.EnsureSuccessStatusCode();
    }

    public async Task ApproveOrderAsync(int id)
    {
        await _httpClient.PostAsync($"api/Orders/{id}/approve", null);
    }

    public async Task RejectOrderAsync(int id, string? comment)
    {
        await _httpClient.PostAsJsonAsync($"api/Orders/{id}/reject", comment);
    }

}