namespace Pharmacy.Api.Models;

public enum OrderStatus
{
    Pending,   // ожидает решения админа
    Approved,  // одобрен
    Rejected   // отклонён
}

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CustomerName { get; set; } = string.Empty;
    public int MedicationId { get; set; }
    public Medication? Medication { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? AdminComment { get; set; }
}